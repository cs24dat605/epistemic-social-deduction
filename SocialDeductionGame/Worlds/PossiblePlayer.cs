using System.Text.Json.Serialization;
using SocialDeductionGame.Roles;

namespace SocialDeductionGame.Worlds;

public class PossiblePlayer
{
    public Player ActualPlayer { get; set; }
    
    [JsonConverter(typeof(RoleConverter))]
    public Role PossibleRole { get; set; }
    public bool IsAlive { get; set; }
    
    public bool RoleExplicitKnown { get; set; }
    
    public string Name => ActualPlayer.Name;
    
    public PossiblePlayer(Role role, Player player)
    {
        ActualPlayer = player;
        PossibleRole = role;
        IsAlive = true;
        RoleExplicitKnown = false;
    }
    
    public PossiblePlayer()
    {
        ActualPlayer = new Player();
        PossibleRole = new Villager(); // Assuming you have a default Role constructor
        IsAlive = true;
        RoleExplicitKnown = false;
    }
}