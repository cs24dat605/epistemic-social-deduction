namespace SocialDeductionGame.Roles;
using SocialDeductionGame.Actions;

public interface IRoleNightAction
{
    void PerformNightAction(Player player, List<Action> actions);
}

public interface IRoleDayAction 
{
    void PerformDayAction(List<Player> players); 
}