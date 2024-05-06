using SocialDeductionGame.Roles;

namespace SocialDeductionGame;

public class GameConfiguration
{
    public int Players { get; set; }
    public int Godfather { get; set; }
    public int Mafioso {  get; set; }
    public int Consort { get; set; }
    public int Consigliere { get; set; }
    public int Blackmailer {  get; set; }
    public int Sheriffs { get; set; }
    public int Escort { get; set; } 
    public int Vigilante { get; set; }
    public int Veteran { get; set; }
    public int Doctor { get; set; }
    public int Investigator { get; set; }
    public int Villagers { get; set; }

    public GameConfiguration()
    {
        Players = 10; // Default number of players
        //Mafia roles
        Godfather = 1; // Default number of Godfathers
        Mafioso = 0; //Default number of Mafioso
        Consort = 0; // Default number of consorts
        Consigliere = 0; //Default number of Consiglieres
        Blackmailer = 0;
        //Town roles
        Sheriffs = 1; // Default number of Sheriffs
        Escort = 0; // Default number of escorts
        Veteran = 0;
        Vigilante = 0;
        Doctor = 0;
        Investigator = 1;
        Villagers = Players - Godfather - Mafioso - Consort - Consigliere - Blackmailer - Sheriffs - Escort - Veteran - Vigilante - Doctor - Investigator;
    }
    
    public List<Role> GetRoleCounts()
    {
        var availableRoles = new List<Role>();
            
        if (Villagers != 0)     availableRoles.AddRange(Enumerable.Repeat(new Villager(), Villagers));
        if (Consigliere != 0)   availableRoles.AddRange(Enumerable.Repeat(new Consigliere(), Consigliere));
        if (Godfather != 0)     availableRoles.AddRange(Enumerable.Repeat(new Godfather(), Godfather));
        if (Mafioso != 0)       availableRoles.AddRange(Enumerable.Repeat(new Mafioso(), Mafioso));
        if (Consort != 0)       availableRoles.AddRange(Enumerable.Repeat(new Consort(), Consort));
        if (Blackmailer != 0)   availableRoles.AddRange(Enumerable.Repeat(new Blackmailer(), Blackmailer));
        if (Escort != 0)        availableRoles.AddRange(Enumerable.Repeat(new Escort(), Escort));
        if (Sheriffs != 0)      availableRoles.AddRange(Enumerable.Repeat(new Sheriff(), Sheriffs));
        if (Vigilante != 0)     availableRoles.AddRange(Enumerable.Repeat(new Vigilante(), Vigilante));
        if (Veteran != 0)       availableRoles.AddRange(Enumerable.Repeat(new Veteran(), Veteran));
        if (Doctor != 0)        availableRoles.AddRange(Enumerable.Repeat(new Doctor(), Doctor));
        if (Investigator != 0)  availableRoles.AddRange(Enumerable.Repeat(new Investigator(), Investigator));

        return availableRoles;
    }
    
    public List<Role> GetRoles()
    {
        var availableRoles = new List<Role>();

        if (Villagers != 0) availableRoles.Add(new Villager());
        if (Consigliere != 0) availableRoles.Add(new Consigliere());
        if (Godfather != 0) availableRoles.Add(new Godfather());
        if (Mafioso != 0) availableRoles.Add(new Mafioso());
        if (Consort != 0) availableRoles.Add(new Consort());
        if (Blackmailer != 0) availableRoles.Add(new Blackmailer());
        if (Escort != 0) availableRoles.Add(new Escort());
        if (Sheriffs != 0) availableRoles.Add(new Sheriff());
        if (Vigilante != 0) availableRoles.Add(new Vigilante());
        if (Veteran != 0) availableRoles.Add(new Veteran());
        if (Doctor != 0) availableRoles.Add(new Doctor());
        if (Investigator != 0) availableRoles.Add(new Investigator());










        return availableRoles;
    }
}
