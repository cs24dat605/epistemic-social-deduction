using SocialDeductionGame;
using System.Text.Json;
using SocialDeductionGame.Worlds;

bool readingMode = true;

for (int i = 0; i < 100; i++)
{
    Game WerewolfGame = Game.Instance;
    Game.Instance.Players = Game.Instance.CreatePlayers();
    List<World> allWorlds = WorldManager.LoadOrGenerateWorlds();
    
    WerewolfGame.StartGame(allWorlds);

    List<string> roles = new List<string>();
    string fileName = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "results.txt"));
    //Start by adding all roles to the string
    foreach (var p in WerewolfGame.Players)
    {
        roles.Add(p.Role.Name); 
    }
    string jsonRoles = JsonSerializer.Serialize(roles);

    string jsonWin;
    if (WerewolfGame.townWin)
    {
        jsonWin = JsonSerializer.Serialize("Town");
    }
    else
    {
        jsonWin = JsonSerializer.Serialize("Mafia");
    }
    
    string jsonRoundCount = JsonSerializer.Serialize(Game.Instance.Round.ToString());

    List<string> activeWorlds = new List<string>();
    foreach (var p in WerewolfGame.Players)
    {
        int numberOfActiveWorlds = 0;
        foreach (var w in p.PossibleWorlds.Where(w => w.IsActive == true))
        {
            numberOfActiveWorlds++;
        }
        activeWorlds.Add(numberOfActiveWorlds.ToString());
    }
    string jsonActiveWorlds = JsonSerializer.Serialize(activeWorlds);

    List<string> playerAlive = new List<string>();
    foreach (var p in WerewolfGame.Players)
    {
        if (p.IsAlive)
        {
            playerAlive.Add("1");
        }
        else
        {
            playerAlive.Add("0");
        }
    }
    string jsonPlayerAlive = JsonSerializer.Serialize(playerAlive);

    List<string> correctVotes = new List<string>();
    foreach (var c in WerewolfGame.correctVotes)
    {
        correctVotes.Add(c.ToString());
    }
    string jsonCorrectVotes = JsonSerializer.Serialize(correctVotes);   

    try
    {
        using (StreamWriter writetext = new StreamWriter(fileName, true))
        {
            if(readingMode)
            {
                writetext.WriteLine("Roles: " + jsonRoles + "\n" +
                                    "Winning team: " + jsonWin + "\n" +
                                    "Number of Rounds: " + jsonRoundCount + "\n" +
                                    "Active Worlds pr. player: " + jsonActiveWorlds + "\n" +
                                    "Alive players: " + jsonPlayerAlive + "\n" +
                                    "Number of correct votes: " + jsonCorrectVotes);
            }
            else
            {
                writetext.WriteLine(jsonRoles + 
                                    jsonWin + 
                                    jsonRoundCount + 
                                    jsonActiveWorlds +
                                    jsonPlayerAlive + 
                                    jsonCorrectVotes);
            }
            
        }
    }
    catch (Exception Ex)
    {
        Console.WriteLine(Ex.ToString());
    }
}