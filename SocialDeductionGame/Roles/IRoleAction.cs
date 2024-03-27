namespace SocialDeductionGame.Roles;

public interface IRoleNightAction
{
    void PerformNightAction(List<Player> players);
}

public interface IRoleDayAction 
{
    void PerformDayAction(List<Player> players); 
}