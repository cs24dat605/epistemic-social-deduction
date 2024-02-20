namespace Werewolf;

public class GameConfiguration
{
    public int Players { get; set; }
    public int Werewolves { get; set; }
    public int Seers { get; set; }

    public GameConfiguration()
    {
        Players = 10; // Default number of players
        Werewolves = 2; // Default number of werewolves
        Seers = 1; // Default number of seers
    }
}
