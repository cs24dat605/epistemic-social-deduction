using SocialDeductionGame.Roles;
using SocialDeductionGame.Worlds;

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
        
        public void StartGame(int players = -1, int werewolves = -1, int seers = -1)
        {
            if (players != -1)
            {
                GameConfig.Players = players;
                if (werewolves != -1)
                    GameConfig.Werewolves = werewolves;
                
                if (seers != -1)
                    GameConfig.Seers = seers;
            }
            
            Players = CreatePlayers();
            
            List<World> allWorlds = WorldManager.GenerateAllWorlds();
            WorldManager.MoveWorldsToPlayers(allWorlds);

            foreach (var world in allWorlds)
            {
                Console.Write("Possible: ");
                world.PrintPossible();
                Console.Write("\nActual: ");
                world.PrintActual();

                Console.WriteLine("\n");
            }
                
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
            int numVillagers = GameConfig.Players - GameConfig.Seers - GameConfig.Werewolves;
            
            availableRoles.AddRange(Enumerable.Repeat(new Villager(), numVillagers));
            availableRoles.AddRange(Enumerable.Repeat(new Seer(), GameConfig.Seers));
            availableRoles.AddRange(Enumerable.Repeat(new Werewolf(), GameConfig.Werewolves));
            
            return availableRoles;
        }

        private void RunDayPhase()
        {
            Console.WriteLine("Day Phase");
            
            // Preform day action before voting might need changing?
            foreach (Player player in Players)
            {
                if (player is {IsAlive:true, Role: IRoleDayAction dayAction})
                {
                    dayAction.PerformDayAction(Players);
                }
            }

            // Voting stuff
            
        }
        
        private void RunNightPhase()
        {
            Console.WriteLine("Night Phase");
            
            foreach (var player in Players)
            {
                if (player is { IsAlive: true, Role: IRoleNightAction nightAction })
                {
                    nightAction.PerformNightAction(Players); 
                }
            }
        }
    }
};

