using Newtonsoft.Json;
using SocialDeductionGame.Communication;

namespace SocialDeductionGame.Worlds;

public class World
{
    
    public List<PossiblePlayer> PossiblePlayers { get; set; }
    
    [JsonIgnore]
    public bool IsActive { get; set; }

    [JsonIgnore]
    public bool IsPrivateActive { get; set; }

    [JsonIgnore]
    public int Marks { get; set; }
    
    public World(List<PossiblePlayer> players)
    {
        PossiblePlayers = players;
        IsActive = true;
        IsPrivateActive = true;
        Marks = 0;
    }
}