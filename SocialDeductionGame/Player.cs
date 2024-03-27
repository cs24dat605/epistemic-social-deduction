using SocialDeductionGame.Communication;
using SocialDeductionGame.Roles;
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

    public void Communicate()
    {
        // Check for worlds if one of them has maybe a higher chance of being the actual world
        // Then ask questions based on that
        
        // Randomly choose to invistigate other players 
        CommunicationManager CM = new CommunicationManager();
        CM.Communicate(this);
    }
}