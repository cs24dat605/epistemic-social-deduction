namespace SocialDeductionGame;

public class GameConfiguration
{
    public int Players { get; set; }
    public int Godfather { get; set; }
    public int Mafioso {  get; set; }
    public int Consort { get; set; }
    public int Consigliere { get; set; }
    public int Sheriffs { get; set; }
    public int Escort { get; set; } 
    public int Vigilante { get; set; }
    public int Veteran { get; set; }
    public int Villagers { get; set; }

    public GameConfiguration()
    {
        Players = 10; // Default number of players
        //Mafia roles
        Godfather = 1; // Default number of Godfathers
        Mafioso = 1; //Default number of Mafioso
        Consort = 1; // Default number of consorts
        Consigliere = 1; //Default number of Consiglieres
        //Town roles
        Sheriffs = 1; // Default number of Sheriffs
        Escort = 1; // Default number of escorts
        Veteran = 1;
        Vigilante = 1;
        Villagers = Players - Godfather - Mafioso - Consort - Consigliere - Sheriffs - Escort - Veteran - Vigilante;
    }
}
