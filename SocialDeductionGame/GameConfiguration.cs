namespace SocialDeductionGame;

public class GameConfiguration
{
    public int Players { get; set; }
    public int Godfather { get; set; }
    public int Mafioso {  get; set; }
    public int Sheriffs { get; set; }
    public int Consort { get; set; }
    public int Escort { get; set; } 
    public int Villagers { get; set; }

    public GameConfiguration()
    {
        Players = 10; // Default number of players
        Godfather = 1; // Default number of Godfathers
        Mafioso = 1; //Default number of Mafioso
        Sheriffs = 1; // Default number of Sheriffs
        Consort = 1; // Default number of consorts
        Escort = 1; // Default number of escorts
        Villagers = Players - Godfather - Mafioso - Consort - Sheriffs - Escort;
    }
}
