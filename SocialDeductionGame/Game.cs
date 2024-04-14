using SocialDeductionGame.Roles;
using SocialDeductionGame.Worlds;
using SocialDeductionGame.Actions;
using System.Collections.Generic;
using System.Numerics;

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
            
            List<World> allWorlds = WorldManager.GenerateAllWorlds();
            WorldManager.MoveWorldsToPlayers(allWorlds);

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
            List<Role> availableRoles = GetRoles();

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

        private List<Role> GetRoles()
        {
            var availableRoles = new List<Role>();
            
            availableRoles.AddRange(Enumerable.Repeat(new Villager(), GameConfig.Villagers));
            availableRoles.AddRange(Enumerable.Repeat(new Consigliere(), GameConfig.Consigliere));
            availableRoles.AddRange(Enumerable.Repeat(new Godfather(), GameConfig.Godfather));
            availableRoles.AddRange(Enumerable.Repeat(new Mafioso(), GameConfig.Mafioso));
            availableRoles.AddRange(Enumerable.Repeat(new Consort(), GameConfig.Consort));
            availableRoles.AddRange(Enumerable.Repeat(new Escort(), GameConfig.Consort));
            availableRoles.AddRange(Enumerable.Repeat(new Sheriff(), GameConfig.Sheriffs));
            availableRoles.AddRange(Enumerable.Repeat(new Vigilante(), GameConfig.Vigilante));
            availableRoles.AddRange(Enumerable.Repeat(new Veteran(), GameConfig.Veteran));

            return availableRoles;
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
                    int MaxPossiblescore = 0;

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
                    var random = new Random();
                    int index = random.Next(worldList.Count);

                    World SelectedWorld = worldList[index];
                    foreach (PossiblePlayer player1 in SelectedWorld.PossiblePlayers)
                    {
                        // Console.WriteLine(player1.ActualPlayer.Name + " " + player1.PossibleRole);
                    }

                    List<PossiblePlayer> playerlist = new List<PossiblePlayer>();
                    if ( player.Role.IsTown)
                    {
                        foreach (PossiblePlayer susplayer in SelectedWorld.PossiblePlayers.Where(susplayer => susplayer.PossibleRole.IsTown == false))
                        {
                            playerlist.Add(susplayer);
                        }
                    }
                    else 
                    {
                        foreach(PossiblePlayer susplayer in SelectedWorld.PossiblePlayers.Where(susplayer => susplayer.PossibleRole.IsTown == true))
                        {
                            playerlist.Add(susplayer);
                        }
                    }

                    index = random.Next(playerlist.Count);
                    Player SelectedPlayer = playerlist[index].ActualPlayer;

                    foreach (VotingPlayer votingPlayer in votingPlayers)
                    {
                        if (votingPlayer.VotedPlayer == SelectedPlayer)
                        {
                            votingPlayer.Votes++;
                        }
                    }
                    Console.WriteLine("I " + player.Name + " am voting for " + SelectedPlayer.Name + " because I think they are a " + SelectedPlayer.Role.ToString() + "");
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
                //Deicde how to handle this
            }

            Console.WriteLine("Marks");
            foreach (World pWorlds in Players[0].PossibleWorlds)
            {
                Console.Write($" {pWorlds.Marks}");
            }
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

