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

            /*foreach (var world in allWorlds)
            {
                Console.Write("Possible: ");
                world.PrintPossible();
                Console.Write("\nActual: ");
                world.PrintActual();

                Console.WriteLine("\n");
            }*/
                
            // while (!GameFinished)
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine($"Round: {i}");
                
                RunDayPhase();
                RunNightPhase();

                if (_gameFinished)
                    break;
            }
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
                player.Communicate();
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

                    Console.WriteLine($"ay: {MaxPossiblescore}");

                    //Generating a list of all equally most possible worlds
                    List<World> worldList = new List<World>();
                    foreach(World world in player.PossibleWorlds.Where(world => world.Marks == MaxPossiblescore && world.IsActive == true))
                    {
                        worldList.Add(world);
                    }

                    //Select at random which of the most likely worlds to choose
                    var random = new Random();
                    int index = random.Next(0, worldList.Count);

                    Console.WriteLine($"WL: {worldList.Count}");

                    World SelectedWorld = worldList[index];
                    foreach (PossiblePlayer player1 in SelectedWorld.PossiblePlayers)
                    {
                        // Console.WriteLine(player1.ActualPlayer.Name + " " + player1.PossibleRole);
                    }

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

