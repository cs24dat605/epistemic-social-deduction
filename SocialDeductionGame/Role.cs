namespace SocialDeductionGame;

public abstract class Role
{
    public string Name { get; set; }
    public bool IsOnVillagerTeam { get; set; }
    public bool forceAction { get; set; }
    public List<string>? checkedPlayers { get; set; }

    public virtual void PerformNightAction(List<Player> players) { }
    public virtual void PerformDayAction(List<Player> players) { }
}