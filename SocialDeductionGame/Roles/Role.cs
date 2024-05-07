namespace SocialDeductionGame.Roles;

public abstract class Role
{
    public string Name { get; set; }
    public bool IsTown { get; set; }
    public bool forceAction { get; set; }
    public List<string>? checkedPlayers { get; set; }
    public bool? blackmailed { get; set; }

    public virtual void PerformNightAction(List<Player> players) { }
    public virtual void PerformDayAction(List<Player> players) { }
}