using Newtonsoft.Json;
using SocialDeductionGame.Roles;
using SocialDeductionGame.Communication;

namespace SocialDeductionGame.Worlds;

public class PossiblePlayer
{
    [JsonProperty("AP")]
    public Role PossibleRole { get; set; }
    
    [JsonProperty("PR")]
    public Player ActualPlayer { get; set; }
    
    
    [JsonIgnore]
    public bool IsAlive { get; set; }
    
    [JsonIgnore]
    public bool RoleExplicitKnown { get; set; }
    
    [JsonIgnore]
    public string Name => ActualPlayer.Name;
    
    [JsonIgnore]
    public int Id => ActualPlayer.Id;

    [JsonIgnore]
    public List<Message> Accusations => ActualPlayer.Accusations;
    
    public PossiblePlayer(Role role, Player player)
    {
        ActualPlayer = player;
        PossibleRole = role;
        IsAlive = true;
        RoleExplicitKnown = false;
    }
}