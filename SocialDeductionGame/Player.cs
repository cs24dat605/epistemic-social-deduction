using SocialDeductionGame.Worlds;

namespace SocialDeductionGame;

public class Player
{
    public string Name { get; }
    public bool IsAlive { get; set; }
    public Role Role { get; }

    public List<World> PossibleWorlds;

    public Player(string name, Role role)
    {
        Name = name;
        Role = role;
        IsAlive = true;
    }
}