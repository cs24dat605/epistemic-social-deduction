using SocialDeductionGame.Communication;
using SocialDeductionGame.Roles;
using Newtonsoft.Json;

namespace SocialDeductionGame.Worlds;

public static class WorldManager
{
    private static string worldFile = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "worlds.json"));
    
    public static List<World> LoadOrGenerateWorlds()
    {
        if (File.Exists(worldFile))
        {
            if (Game.Instance.shouldPrint)
                Console.WriteLine("Loading generated worlds from file!");
            
            List<World> worlds = new List<World>();

            // Read the file line by line
            // foreach (string line in File.ReadLines(worldFile))
            // {
            //     // Skip empty lines
            //     if (string.IsNullOrWhiteSpace(line))
            //     {
            //         continue;
            //     }
            //
            //     // Read JSON string from file
            //     // Deserialize back into an object
            //     World world = JsonConvert.DeserializeObject<World>(line, new PossiblePlayerConverter());
            //     
            //     worlds.Add(world); 
            // }
            var lines = File.ReadLines(worldFile); // Read all lines once

            Parallel.ForEach(lines, line =>
            {
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line))
                {
                    return;  // Skip to the next iteration
                }

                // Read JSON string from file
                // Deserialize back into an object
                World world = JsonConvert.DeserializeObject<World>(line, new PossiblePlayerConverter());

                // Since worlds.Add is not thread-safe, use a lock to protect it
                lock (worlds) 
                {
                    worlds.Add(world); 
                }
            });

            
            return worlds;
        }
        
        var counts = new Dictionary<Role, int>();

        if (Game.Instance.GameConfig.Villagers != 0)    counts.Add(new Villager(), Game.Instance.GameConfig.Villagers);
        if (Game.Instance.GameConfig.Sheriffs != 0)     counts.Add(new Sheriff(), Game.Instance.GameConfig.Sheriffs);
        if (Game.Instance.GameConfig.Escort != 0)       counts.Add(new Escort(), Game.Instance.GameConfig.Escort);
        if (Game.Instance.GameConfig.Vigilante != 0)    counts.Add(new Vigilante(), Game.Instance.GameConfig.Vigilante);
        if (Game.Instance.GameConfig.Veteran != 0)      counts.Add(new Veteran(), Game.Instance.GameConfig.Veteran);
        if (Game.Instance.GameConfig.Doctor != 0)       counts.Add(new Doctor(), Game.Instance.GameConfig.Doctor);
        if (Game.Instance.GameConfig.Investigator != 0) counts.Add(new Investigator(), Game.Instance.GameConfig.Investigator);
        if (Game.Instance.GameConfig.Godfather != 0)    counts.Add(new Godfather(), Game.Instance.GameConfig.Godfather);
        if (Game.Instance.GameConfig.Mafioso != 0)      counts.Add(new Mafioso(), Game.Instance.GameConfig.Mafioso);
        if (Game.Instance.GameConfig.Consort != 0)      counts.Add(new Consort(), Game.Instance.GameConfig.Consort);
        if (Game.Instance.GameConfig.Consigliere != 0)  counts.Add(new Consigliere(), Game.Instance.GameConfig.Consigliere);
        if (Game.Instance.GameConfig.Blackmailer != 0)  counts.Add(new Blackmailer(), Game.Instance.GameConfig.Blackmailer);
        
        Console.WriteLine("No generated worlds file was found!");
        Console.WriteLine("Generating worlds");
        
        var numbers = counts.Keys.ToList();
        List<World> allWorlds = new List<World>();

        GenerateCombinations(new Role[Game.Instance.GameConfig.Players], 0, numbers, counts, allWorlds);
        
         // allWorlds = GenerateArraysBruteForce(counts);
        
        // File.WriteAllText(worldFile, JsonSerializer.Serialize(allWorlds));

        return allWorlds;
    }

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
            
            var serializedWorld = JsonConvert.SerializeObject(world, new PossiblePlayerConverter());
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
        foreach (var player in Game.Instance.Players)
        {
            List<World> worldList = new List<World>();
            
            foreach (var world in worlds)
            {
                World worldCopy = new World(world.PossiblePlayers);

                worldList.Add(worldCopy);
            }

            player.PossibleWorlds = worldList;
        }
    }

    public static void UpdateWorldsByMessage(Message message, int type)
    {
        foreach (var player in Game.Instance.Players)
        {
            player.Accusations.Add(message);
            
            foreach (var pWorld in player.PossibleWorlds)
            {
                // if (!pWorld.IsActive)
                //     continue;
                
                foreach (var pPlayer in pWorld.PossiblePlayers)
                {
                    // If pPlayer is not the accuser, skip
                    if (type == 0 && pPlayer.ActualPlayer != message.Accuser)
                        continue;
                    
                    // If pPlayer is not the accused, skip
                    if (type is 1 or 2 or 3 && pPlayer != message.Accused)
                        continue;
                    
                    bool isCorrect;
                    
                    // Check correct by if role matches world
                    if (type is 0 or 1 or 2)
                        isCorrect = pPlayer.PossibleRole.Name == message.Role.Name;
                    
                    // Flip statement by marking the opposite roles
                    if (type == 3 || message.Response == "No")
                        isCorrect = pPlayer.PossibleRole.Name != message.Role.Name;
                    else
                        isCorrect = pPlayer.PossibleRole.Name == message.Role.Name;
                    
                    // Add or remove marks
                    if (isCorrect)
                        pWorld.Marks++;
                    else
                        pWorld.Marks--;
                }
            }
        }
    }
}