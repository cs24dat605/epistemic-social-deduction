// Path to the text file
string filePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "results.txt"));

// Dictionary to store the count of wins for each team
Dictionary<string, int> teamScores = new Dictionary<string, int>
{
    { "Mafia", 0 },
    { "Town", 0 }
};

List<int> survivability = new List<int>();
List<int> surviveTeamWin = new List<int>();
List<string> roles = new List<string>();
List<int> votes = new List<int>();

// Read the text file line by line
using (StreamReader sr = new StreamReader(filePath))
{
    string line;
    line = sr.ReadToEnd();
    // Split the line into team members and winning team

    
    string[] parts = line.Split("\n");

    for (int i = 0; i < parts.Length - (parts.Length%6); i += 6)
    {
        if (i == 0)
        {
            string[] init = parts[i].Replace("Roles: ", "").Replace("\"", "").Replace("\r", "").Replace("[", "").Replace("]", "").Split(",");
            foreach (string s in init)
            {
                roles.Add(s);
                survivability.Add(0);
                surviveTeamWin.Add(0);
                votes.Add(0);

            }
        }
        string winningTeam = parts[i + 1].Replace("Winning team: ", "").Replace("\"", "").Replace("\r", "");
        teamScores[winningTeam] += 1;
        int rounds = int.Parse(parts[i + 2].Replace("Number of Rounds: ", "").Replace("\"", "").Replace("\r", ""));
        string[] buffer = parts[i + 3].Replace("Active Worlds pr. player: ", "").Replace("\"", "").Replace("\r", "").Replace("[", "").Replace("]", "").Split(",");
        int[] worldPrPlayer = new int[buffer.Length];
        for(int j = 0; j < buffer.Length; j++)
        {
            worldPrPlayer[j] = int.Parse(buffer[j]);
        }
        buffer = parts[i + 4].Replace("Alive players: ", "").Replace("\"", "").Replace("\r", "").Replace("[", "").Replace("]", "").Split(",");
        bool[] alivePlayers = new bool[buffer.Length];
        for (int j = 0; j < buffer.Length; j++)
        {
            alivePlayers[j] = int.Parse(buffer[j]) == 1;
            survivability[j] += int.Parse(buffer[j]);
        }
        buffer = parts[i + 5].Replace("Number of correct votes: ", "").Replace("\"", "").Replace("\r", "").Replace("[", "").Replace("]", "").Split(",");
        int[] correctVotes = new int[buffer.Length];
        for (int j = 0; j < buffer.Length; j++)
        {
            correctVotes[j] = int.Parse(buffer[j]);
            votes[j] += int.Parse(buffer[j]);
        }

        
        for (int j = 0; j < buffer.Length; j++)
        {
            bool isTown = true;
            switch (roles[j])
            {
                case "Blackmailer":
                case "Consigliere":
                case "Godfather":
                case "Mafioso":
                case "Consort":
                    isTown = false;
                    break;
                default: break;
            }
            if(isTown && winningTeam == "Town" && alivePlayers[j])
            {
                
                surviveTeamWin[j]++;
                
            }
            else if(!isTown && winningTeam == "Mafia" && alivePlayers[j])
            {
                surviveTeamWin[j]++;   
            }
        }

    }
}

// Display total scores of each team
Console.WriteLine("Total Scores:");
double total = 0;
foreach (var teamScore in teamScores)
{
    total += teamScore.Value;
}
foreach (var teamScore in teamScores)
{
    Console.WriteLine($"{teamScore.Key}: ".PadRight(7) + $"{teamScore.Value}".PadLeft(6));
}

for(int i = 0; i < roles.Count; i++)
{
    bool isTown = true;
    switch (roles[i])
    {
        case "Blackmailer":
        case "Consigliere":
        case "Godfather":
        case "Mafioso":
        case "Consort":
            isTown = false;
            break;
        default: break;
    }
    double town = teamScores["Town"];
    double mafia = teamScores["Mafia"];
    Console.WriteLine("Player:" + roles[i].PadRight(12) + "Total times survived: " + survivability[i].ToString().PadRight(6) + "Survivalrate: " + Math.Round(((survivability[i] / total) * 100), 2).ToString().PadRight(6) + "Total correct votes: " + votes[i].ToString().PadRight(6) + "Survivalrate on team win: " + (isTown ? Math.Round((surviveTeamWin[i] / town) * 100, 2).ToString().PadRight(6) : Math.Round((surviveTeamWin[i] / mafia) * 100, 2).ToString().PadRight(6)));

}