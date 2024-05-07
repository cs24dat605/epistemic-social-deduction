using SocialDeductionGame.Communication;
using SocialDeductionGame.Roles;
using SocialDeductionGame.Worlds;
using Newtonsoft.Json;

namespace SocialDeductionGame;

public class Player
{
    public int Id { get; }
    public string Name { get; }
    
    private bool _isAlive;

    public bool IsAlive
    {
        get => _isAlive;
    }
    
    // [JsonConverter(typeof(RoleConverter))]
    public Role Role { get; }
    public List<World> PossibleWorlds;
    public List<Message> Accusations = [];

    public void Kill()
    {
        _isAlive = false;
        WorldManager.UpdateWorldByDeath(this);
    }

    public Player(int id, Role role)
    {
        Id = id;
        Name = $"Player {id}" ;
        Role = role;
        _isAlive = true;
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