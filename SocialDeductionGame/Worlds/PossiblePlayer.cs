namespace SocialDeductionGame.Worlds;

public class PossiblePlayer
{
    public Player ActualPlayer { get; set; }
    public Role PossibleRole { get; set; }
    public bool IsAlive { get; set; }
    
    public PossiblePlayer(Role role, Player player)
    {
        ActualPlayer = player;
        PossibleRole = role;
        IsAlive = true;
    }
}