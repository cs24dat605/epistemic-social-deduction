using SocialDeductionGame.Roles;
using SocialDeductionGame.Worlds;
using SocialDeductionGame.Actions;

namespace SocialDeductionGame
{
    public class Game
    {
        public GameConfiguration GameConfig = new GameConfiguration();
        public List<Player> Players { get; set; }
        
        private int _round = 0;

        public int Round => _round;

        private static Game _instance;
        private bool _gameFinished = false;

        private long startTime = 0;

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
        
        public void StartGame(int players = -1, int godfather = -1, int sheriffs = -1, int mafioso = -1, int escort = -1, int consort = -1)
        {
            /*if (players != -1)
            {
                GameConfig.Players = players;
                if (godfather != -1)
                    GameConfig.Godfather = godfather;
                if (sheriffs != -1)
                    GameConfig.Sheriffs = sheriffs;
                if (mafioso != -1)
                    GameConfig.Mafioso = mafioso;
                if (escort != -1)
                    GameConfig.Escort = escort;
                if (consort != -1)
                   GameConfig.Consort = consort;
            }*/
            
            Players = CreatePlayers();

            var curTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            List<World> allWorlds = WorldManager.LoadOrGenerateWorlds();
            Console.WriteLine($"Time taken to generate worlds: {DateTimeOffset.UtcNow.ToUnixTimeSeconds() - curTime}");

            Console.WriteLine("Moving worlds to player");
            WorldManager.MoveWorldsToPlayers(allWorlds);
            Console.WriteLine($"Time taken to move to player: {DateTimeOffset.UtcNow.ToUnixTimeSeconds() - curTime}");

            startTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            while (!_gameFinished)
            {
                Console.WriteLine($"Round: {Round}");
                
                RunDayPhase();
                RunNightPhase();
                
                CheckIfFinished();
                
                _round++;

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
                Console.WriteLine($"Town wins! Round:{_round}");
            else if (mafiaWins)
                Console.WriteLine($"Mafia wins! Round:{_round}");

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
                    i,
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
            
            Random random = new Random();

            // Allowing up to 3 communications per turn
            for (var i = 0; i < 2; i++)
            {
                foreach (Player player in Players.Where(player => player.IsAlive).OrderBy(_ => random.Next()))
                {
                        player.Communicate();
                }
            }
            
            // Voting stuff
            List<VotingPlayer> votingPlayers = new List<VotingPlayer>();
            foreach (Player player in Players.Where (player => player.IsAlive == true))
            {
                VotingPlayer voting = new VotingPlayer(player, 0);
                votingPlayers.Add(voting);
            }
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
                    List<World> worldList = new List<World>();
                    foreach(World world in player.PossibleWorlds.Where(world => world.Marks == MaxPossiblescore && world.IsActive == true))
                    {
                        worldList.Add(world);
                    }

                    //Select at random which of the most likely worlds to choose
                    random = new Random();
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
                        }
                    }
                    
                    Console.WriteLine("I " + player.Name + " am voting for " + SelectedPlayer.Name + " because I think they are a " + SelectedPlayer.PossibleRole.Name + "");
                }
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
            List<VotingPlayer> TrialList = new List<VotingPlayer>();
            if (TrialList.Count == 1)
            {
                //Kill this player
            }
            else
            {
                // Decide how to handle this
            }

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

