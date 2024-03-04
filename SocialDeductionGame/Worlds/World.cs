namespace SocialDeductionGame.Worlds;

public class World
{
    public List<PossiblePlayer> PossiblePlayer { get; set; }
    public int PossibleScore = 0;
    public List<Accusations> Accusations = new List<Accusations>();
    
    public World(List<PossiblePlayer> players)
    {
        PossiblePlayer = players;
    }

    public void PrintPossible()
    {
        foreach (var player in this.PossiblePlayer)
        {
            Console.Write(player.PossibleRole.Name + " ");
        }
    }
    
    public void PrintActual()
    {
        foreach (var player in this.PossiblePlayer)
        {
            Console.Write(player.ActualPlayer.Role.Name + " ");
        }
    }
}