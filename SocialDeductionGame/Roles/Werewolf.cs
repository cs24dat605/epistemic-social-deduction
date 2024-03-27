namespace SocialDeductionGame.Roles;

public class Werewolf : Role, IRoleNightAction
{
    public Werewolf() 
    {
        Name = "Werewolf";
        IsTown = false;
    }

    public override void PerformNightAction(List<Player> players)
    {
        // Kill person
    }
}