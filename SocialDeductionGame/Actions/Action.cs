namespace SocialDeductionGame.Actions;
public class Action
{
    public Player player {  get; set; }
    public string typeOfAction { get; set; }
    public Player target {  get; set; }

    public Action (Player player, string typeOfAction, Player target)
    {
        this.player = player;
        this.typeOfAction = typeOfAction;
        this.target = target;
    }
}
