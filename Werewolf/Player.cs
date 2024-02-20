namespace Werewolf;

public class Player
{
    public string Name { get; set; }
    public bool IsAlive { get; set; }
    public Role Role { get; set; }
    public Dictionary<World, HashSet<Role>> PossibleWorlds { get; set; }

    public Player(string name)
    {
        Name = name;
        IsAlive = true;
    }
}