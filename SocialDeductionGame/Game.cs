using SocialDeductionGame.Roles;
using SocialDeductionGame.Worlds;
using SocialDeductionGame.Actions;

namespace SocialDeductionGame
{
    public class Game
    {
        public GameConfiguration GameConfig = new GameConfiguration();
        public List<Player> Players { get; set; }

        
        private static Game _instance;
        private bool _gameFinished = false;

        //At the end of the game, if town has won, set to true
        //else the mafia has won
        //used for data collection
        public bool townWin = false;

        public int round = 0;

        private long startTime = 0;

        public List <int> correctVotes { get; set; }

        public static Game Instance 
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new Game();
                }
                return _instance;
            }
        }
        
        public void StartGame()
        {
            _gameFinished = false;
            Players = CreatePlayers();

            var curTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            List<World> allWorlds = WorldManager.LoadOrGenerateWorlds();
            Console.WriteLine($"Time taken to generate worlds: {DateTimeOffset.UtcNow.ToUnixTimeSeconds() - curTime}");

            Console.WriteLine("Moving worlds to player");
            WorldManager.MoveWorldsToPlayers(allWorlds);
            Console.WriteLine($"Time taken to move to player: {DateTimeOffset.UtcNow.ToUnixTimeSeconds() - curTime}");

            startTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            round = 0;

            correctVotes = new List<int>();
            for(int i = 0; i < Players.Count; i++)
            {
                correctVotes.Add(0);
            }
            
            while (!_gameFinished)
            {
                Console.WriteLine($"Round: {round++}");
                
                RunDayPhase();
                RunNightPhase();
                
                CheckIfFinished();

                if (_gameFinished)
                {
                    Console.WriteLine($"Game time taken: {DateTimeOffset.UtcNow.ToUnixTimeSeconds() - startTime}");
                    break;
                }
            }
        }
        
        private void CheckIfFinished()
        {
            bool townWins = !Players.Any(p => p.IsAlive && !p.Role.IsTown);
            bool mafiaWins = Players.Count(p => p.IsAlive && !p.Role.IsTown) >= Players.Count(p => p.IsAlive && p.Role.IsTown);

            if (townWins)
            {
                Console.WriteLine("Town wins!");
                townWin = true;
            }
                
            else if (mafiaWins)
            {
                Console.WriteLine("Mafia wins!");
                townWin = false;
            }
            _gameFinished = townWins || mafiaWins;
        }

        private List<Player> CreatePlayers()
        {
            Console.WriteLine("Creating Players");
            
            List<Player> playerList = new List<Player>();
            List<Role> availableRoles = GameConfig.GetRoleCounts();

            for (int i = 0; i < GameConfig.Players; i++)
            {
                Player newPlayer = new Player(
                    "Player " + (i + 1),
                    availableRoles[i]
                );
                
                playerList.Add(newPlayer);
                
            }
            
            // Players.Sort();
            // TODO Need to sort somehow

            // Console.WriteLine(playerList);
            return playerList;
        }

        private void RunDayPhase()
        {
            Console.WriteLine("Day Phase");
            
            // Preform day action before voting might need changing?
            foreach (Player player in Players.Where(player => player.IsAlive))
            {
                if (player is {Role: IRoleDayAction dayAction})
                {
                    dayAction.PerformDayAction(Players);
                }
            }

            foreach (Player player in Players.Where(player => player.IsAlive))
            {
                if (player.Role.blackmailed == null || player.Role.blackmailed == false)
                {
                    player.Communicate();
                }

                player.Role.blackmailed = false;
                
            }

            
            
            // Voting stuff
            List<VotingPlayer> votingPlayers = new List<VotingPlayer>();
            foreach (Player player in Players.Where (player => player.IsAlive == true))
            {
                VotingPlayer voting = new VotingPlayer(player, 0);
                votingPlayers.Add(voting);
            }
            int i = 0;
            foreach (Player player in Players.Where(player => player.IsAlive == true))
            {
                if (player.IsAlive)
                {
                    int MaxPossiblescore = Int32.MinValue;

                    //MaxPossible score
                    foreach (World world in player.PossibleWorlds.Where(world => world.IsActive == true))
                    {
                        if(MaxPossiblescore < world.Marks)
                        {
                            MaxPossiblescore = world.Marks;
                        }
                    }

                    //Generating a list of all equally most possible worlds
                    List<World> worldList = [.. player.PossibleWorlds.Where(world => world.Marks == MaxPossiblescore && world.IsActive == true)];

                    //Select at random which of the most likely worlds to choose
                    var random = new Random();
                    int index = random.Next(0, worldList.Count);

                    World SelectedWorld = worldList[index];

                    List<PossiblePlayer> playerList = new List<PossiblePlayer>();
                    if (player.Role.IsTown)
                    {
                        foreach (PossiblePlayer susPlayer in SelectedWorld.PossiblePlayers.Where(susPlayer => susPlayer.PossibleRole.IsTown == false))
                        {
                            playerList.Add(susPlayer);
                        }
                    }
                    else 
                    {
                        foreach(PossiblePlayer susPlayer in SelectedWorld.PossiblePlayers.Where(susPlayer => susPlayer.PossibleRole.IsTown == true))
                        {
                            playerList.Add(susPlayer);
                        }
                    }

                    index = random.Next(playerList.Count);
                    PossiblePlayer SelectedPlayer = playerList[index];

                    foreach (VotingPlayer votingPlayer in votingPlayers)
                    {
                        if (votingPlayer.VotedPlayer == SelectedPlayer.ActualPlayer)
                        {
                            votingPlayer.Votes++;

                            //Data stuff
                            break;
                        }
                    }

                    if ((player.Role.IsTown && !SelectedPlayer.ActualPlayer.Role.IsTown) ||
                                (!player.Role.IsTown && SelectedPlayer.ActualPlayer.Role.IsTown))
                    {
                        correctVotes[i]++;
                    }
                    Console.WriteLine("I " + player.Name + " am voting for " + SelectedPlayer.Name + " because I think they are a " + SelectedPlayer.PossibleRole.Name + "");
                }
                i++;
            }

            //MaxVotes
            int MaxVotes = 0;
            foreach (VotingPlayer votedPlayer in votingPlayers)
            {
                // Console.WriteLine(votedPlayer.VotedPlayer.Name + " " + votedPlayer.Votes.ToString());
                if (MaxVotes < votedPlayer.Votes)
                {
                   MaxVotes = votedPlayer.Votes;
                }
            }

            //Generating a list of all player that have gotten equal votes
            List<VotingPlayer> TrialList = [.. votingPlayers.Where(Vp => Vp.Votes == MaxVotes)];
            if (TrialList.Count == 1)
            {
                //Kill this player
                foreach (Player p in Players.Where(p => p.IsAlive))
                {
                    foreach (World world in p.PossibleWorlds)
                    {
                        foreach (PossiblePlayer possiblePlayer in world.PossiblePlayers.Where(possiblePlayer => possiblePlayer.Name == TrialList[0].VotedPlayer.Name))
                        {
                            possiblePlayer.IsAlive = false;
                        }
                    }
                }
                foreach (Player p in Players.Where(p => p.Name == TrialList[0].VotedPlayer.Name))
                {
                    p.IsAlive = false;
                }

            }
            //If a majority vote has not been cast, no one is killed

            // Console.WriteLine("Marks");
            // foreach (World pWorlds in Players[0].PossibleWorlds)
            // {
            //     Console.Write($" {pWorlds.Marks}");
            // }
        }
        
        private void RunNightPhase()
        {
            Console.WriteLine("Night Phase");

            List<Actions.Action> actions = new List<Actions.Action>();

            //Reset blackmail
            foreach (Player player in Players)
            {
                player.Role.blackmailed = false;
            }

            foreach (var player in Players)
            {
                if (player is { IsAlive: true, Role: IRoleNightAction nightAction })
                {
                    nightAction.PerformNightAction(player, actions); 
                    
                    
                }
            }

            ActionManager actionManager = new ActionManager(actions);

            actionManager.HandleActions(Players);
        }
    }
};

