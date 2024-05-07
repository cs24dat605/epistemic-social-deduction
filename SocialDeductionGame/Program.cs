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
    string fileName = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "test.txt"));
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



//Summary of tests
if (!readingMode)
{
    // Path to the text file
    string filePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "test.txt"));

    // Dictionary to store the count of wins for each team
    Dictionary<string, int> teamScores = new Dictionary<string, int>
        {
            { "Mafia", 0 },
            { "Town", 0 }
        };

    // Read the text file line by line
    using (StreamReader sr = new StreamReader(filePath))
    {
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            // Split the line into team members and winning team
            string[] parts = line.Split(new string[] { "]\"" }, StringSplitOptions.RemoveEmptyEntries);
            string[] parts2 = parts[1].Split('"');
            // Extract team members and winning team
            string rolesString = parts[0].Trim() + "]";
            string winningTeam = parts2[0].Trim('"');

            // Update team scores only if the team has won
            if (teamScores.ContainsKey(winningTeam))
            {
                teamScores[winningTeam]++;
            }
        }
    }

    // Display total scores of each team
    Console.WriteLine("Total Scores:");
    foreach (var teamScore in teamScores)
    {
        Console.WriteLine($"{teamScore.Key}: {teamScore.Value}");
    }
}
