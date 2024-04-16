using System.Text.Json;
using SocialDeductionGame.Communication;
using SocialDeductionGame.Roles;

namespace SocialDeductionGame.Worlds;

public static class WorldManager
{
    private static string worldFile = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "worlds.json"));
    
    public static List<World> LoadOrGenerateWorlds()
    {
        if (File.Exists(worldFile))
        {
            Console.WriteLine("Loading generated worlds from file!");
            
            var options = new JsonSerializerOptions { IncludeFields = true, Converters = { new WorldConverter() } };
            
            List<World> worlds = new List<World>();

            // Read the file line by line
            foreach (string line in File.ReadLines(worldFile))
            {
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
            
                // Deserialize the world from the line
                World world = JsonSerializer.Deserialize<World>(line, options);
                worlds.Add(world);
            }
            
            return worlds;
        }
        
        var counts = new Dictionary<Role, int>
        {
            { new Villager(), Game.Instance.GameConfig.Villagers }, 
            { new Sheriff(), Game.Instance.GameConfig.Sheriffs },
            { new Escort(), Game.Instance.GameConfig.Escort },
            { new Vigilante(), Game.Instance.GameConfig.Vigilante },
            { new Veteran(), Game.Instance.GameConfig.Veteran },
            { new Godfather(), Game.Instance.GameConfig.Godfather },
            { new Mafioso(), Game.Instance.GameConfig.Mafioso },
            { new Consort(), Game.Instance.GameConfig.Consort },
            { new Consigliere(), Game.Instance.GameConfig.Consigliere}
        };

        Console.WriteLine("No generated worlds file was found!");
        Console.WriteLine("Generating worlds");
        
        var numbers = counts.Keys.ToList();
        List<World> allWorlds = new List<World>();

        GenerateCombinations(new Role[Game.Instance.GameConfig.Players], 0, numbers, counts, allWorlds);
        
         // allWorlds = GenerateArraysBruteForce(counts);
        
        // File.WriteAllText(worldFile, JsonSerializer.Serialize(allWorlds));

        return allWorlds;
    }

    // private static List<World> GenerateArraysBruteForce(Dictionary<Role, int> counts)
    // {
    //     var numbers = counts.Keys.ToList();
    //     var uniqueArrays = new List<World>();
    //
    //     GenerateCombinations(new Role[Game.Instance.GameConfig.Players], 0, numbers, counts, uniqueArrays);
    //
    //     return uniqueArrays;
    // }

    private static void GenerateCombinations(Role[] curArray, int i, List<Role> roleList, Dictionary<Role, int> counts, List<World> uniqueArrays)
    {
        if (i == curArray.Length)
        {
            if (counts.Any(kvp => curArray.Count(role => role == kvp.Key) != kvp.Value))
                return;
                
            var world = new World(curArray.Select((role, playerIndex) =>
                new PossiblePlayer(role, Game.Instance.Players.ElementAt(playerIndex))
            ).ToList());
            uniqueArrays.Add(world);
            
            var serializedWorld = JsonSerializer.Serialize(world, new JsonSerializerOptions { IncludeFields = true, Converters = { new WorldConverter() } });
            File.AppendAllText(worldFile, serializedWorld + Environment.NewLine);

            return;
        }

        foreach (var role in roleList)
        {
            curArray[i] = role;
            GenerateCombinations(curArray, i + 1, roleList, counts, uniqueArrays);
        }
    }
    
    public static void MoveWorldsToPlayers(List<World> worlds)
    {
        List<World> worldCopy = new List<World>(worlds);
        foreach (var player in Game.Instance.Players)
        {
            player.PossibleWorlds = new List<World>(worldCopy);
        }
    }

    public static void UpdateWorldsByMessage(Message message, int type)
    {
        foreach (var player in Game.Instance.Players)
        {
            foreach (var pWorld in player.PossibleWorlds)
            {
                if (!pWorld.IsActive)
                    continue;

                pWorld.Accusations.Add(message);

                foreach (var pPlayer in pWorld.PossiblePlayers)
                {
                    // TODO maybe implement that this on is acccused instead in the message
                    if (type == 0)
                        if (pPlayer.Name != message.Accuser.Name)
                            continue;

                    if (type is 1 or 2 or 3)
                        if (pPlayer.Name != message.Accused.Name)
                            continue;
                    
                    bool isCorrect = pPlayer.PossibleRole.Name == message.Role.Name;

                    // Flip statement by marking the opposite roles
                    if (type == 3 || message.Response == "No")
                        isCorrect = pPlayer.PossibleRole.Name != message.Role.Name;
                    
                    if (isCorrect)
                        pWorld.Marks++;
                    else
                        pWorld.Marks--;
                }
            }
        }
    }
}