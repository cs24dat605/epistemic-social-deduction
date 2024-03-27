namespace SocialDeductionGame.Roles;

public class Seer : Role, IRoleNightAction
{
    public Seer()
    {
        Name = "Seer";
        IsTown = true;
    }

    public override void PerformNightAction(List<Player> players)
    {
        // Logic to select a player and learn their role
    }
}