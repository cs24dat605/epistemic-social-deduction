namespace SocialDeductionGame.Worlds;

public abstract class WorldList : List<World>
{
    private List<World> _list { get; set; }
    
    public WorldList(List<World> worlds)
    {
        _list = worlds;
    }

}