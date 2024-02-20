namespace Werewolf;

public abstract class Role
{
    public string Name { get; set; }
    public bool IsOnVillagerTeam { get; set; }

    public virtual void PerformNightAction(List<Player> players) { }
    public virtual void PerformDayAction(List<Player> players) { }
}