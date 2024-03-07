namespace SocialDeductionGame.Roles;

public class Seer : Role, IRoleNightAction
{
    public Seer()
    {
        Name = "Seer";
        IsOnVillagerTeam = true;
    }

    public override void PerformNightAction(List<Player> players)
    {
        // Logic to select a player and learn their role
    }
}