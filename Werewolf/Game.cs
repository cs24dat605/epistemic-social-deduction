using Werewolf.Roles;

namespace Werewolf;

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
    
    public void StartGame(int players, int werewolves, int seers)
    {
        GameConfig.Players = players;
        GameConfig.Werewolves = werewolves;
        GameConfig.Seers = seers;
        
        CreatePlayers(players);
        // Todo maybe convert werewolves, seers to array
        AssignRoles(werewolves, seers);
        CreatePlayerWorlds(werewolves, seers);
            
        // while (!GameFinished)
        for (int i = 0; i < 10; i++)
        {
            RunDayPhase();
            RunNightPhase();

            if (_gameFinished)
                break;
        }
            
            
        // ... 
    }

    private void CreatePlayers(int numPlayers)
    {
        Console.WriteLine("Creating Players");
        Players = new List<Player>();
        
        for (int i = 0; i < numPlayers; i++)
        {
            this.Players.Add(new Player("Player " + (i + 1)));
        }
        
        // Players.Sort();
        // TODO Need to sort somehow

        Console.WriteLine(this.Players);
    }

    private void AssignRoles(int werewolves, int seers)
    {
        Console.WriteLine("Assign Roles");
        
        int numVillagers = Players.Count - seers - werewolves;

        var availableRoles = new List<Role>();
        
        availableRoles.AddRange(Enumerable.Repeat(new Roles.Villager(), numVillagers));
        availableRoles.AddRange(Enumerable.Repeat(new Roles.Seer(), seers));
        availableRoles.AddRange(Enumerable.Repeat(new Roles.Werewolf(), werewolves));
        

        foreach (var player in Players) 
        {
            player.Role = availableRoles[0]; // Assign the first role
            availableRoles.RemoveAt(0);      // Remove it to prevent reuse
        }
    }

    private void CreatePlayerWorlds(int werewolves, int seers)
    {
        // foreach (var player in this.Players)
        // {
        //     player.PossibleWorlds = world.GeneratePossibleWorlds(this.Players, werewolves, seers);
        // }
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