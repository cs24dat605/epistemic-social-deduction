using SocialDeductionGame.Communication;

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

    public void PrintPossible()
    {
        foreach (var player in this.PossiblePlayers)
        {
            Console.Write(player.PossibleRole.Name + " ");
        }
    }
    
    public void PrintActual()
    {
        foreach (var player in this.PossiblePlayers)
        {
            Console.Write(player.ActualPlayer.Role.Name + " ");
        }
    }
}