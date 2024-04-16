using System.Text.Json.Serialization;
using SocialDeductionGame.Communication;
using SocialDeductionGame.Roles;

namespace SocialDeductionGame.Worlds;

public class World
{
    
    public List<PossiblePlayer> PossiblePlayers { get; set; }
    public bool IsActive = true;
    public int Marks = 0;
    public List<Message> Accusations = new List<Message>();
    
    public World(List<PossiblePlayer> players)
    {
        PossiblePlayers = players;
    }
    
    // Needed for JSON Deserialization
    public World()
    {
    }
}