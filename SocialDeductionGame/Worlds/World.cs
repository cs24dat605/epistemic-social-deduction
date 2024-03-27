namespace SocialDeductionGame.Worlds;

public class World
{
    public List<PossiblePlayer> PossiblePlayers { get; set; }
    public int PossibleScore = 0;
    public bool isActive = true;
    public List<Accusations> Accusations = new List<Accusations>();
    
    public World(List<PossiblePlayer> playerses)
    {
        PossiblePlayers = playerses;
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