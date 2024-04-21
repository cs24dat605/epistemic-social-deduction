using SocialDeductionGame.Roles;

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
    
    public List<Role> GetRoleCounts()
    {
        var availableRoles = new List<Role>();
            
        availableRoles.AddRange(Enumerable.Repeat(new Villager(), Villagers));
        availableRoles.AddRange(Enumerable.Repeat(new Consigliere(), Consigliere));
        availableRoles.AddRange(Enumerable.Repeat(new Godfather(), Godfather));
        availableRoles.AddRange(Enumerable.Repeat(new Mafioso(), Mafioso));
        availableRoles.AddRange(Enumerable.Repeat(new Consort(), Consort));
        availableRoles.AddRange(Enumerable.Repeat(new Escort(), Consort));
        availableRoles.AddRange(Enumerable.Repeat(new Sheriff(), Sheriffs));
        availableRoles.AddRange(Enumerable.Repeat(new Vigilante(), Vigilante));
        availableRoles.AddRange(Enumerable.Repeat(new Veteran(), Veteran));

        return availableRoles;
    }
    
    public List<Role> GetRoles()
    {
        var availableRoles = new List<Role>();
            
        availableRoles.Add(new Villager());
        availableRoles.Add(new Consigliere());
        availableRoles.Add(new Godfather());
        availableRoles.Add(new Mafioso());
        availableRoles.Add(new Consort());
        availableRoles.Add(new Escort());
        availableRoles.Add(new Sheriff());
        availableRoles.Add(new Vigilante());
        availableRoles.Add(new Veteran());

        return availableRoles;
    }
}
