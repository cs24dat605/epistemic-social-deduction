using SocialDeductionGame.Communication;
using SocialDeductionGame.Roles;

namespace SocialDeductionGame.Worlds;

public static class WorldManager
{
    public static List<World> GenerateAllWorlds()
    {
        var counts = new Dictionary<Role, int>
        {
            { new Villager(), Game.Instance.GameConfig.Villagers }, 
            { new Seer(), Game.Instance.GameConfig.Seers },
            { new Werewolf(), Game.Instance.GameConfig.Werewolves }
        };
        
        return GenerateArraysBruteForce(counts);
    }
    
    private static List<World> GenerateArraysBruteForce(Dictionary<Role, int> counts)
    {
        var numbers = counts.Keys.ToList();
        var uniqueArrays = new List<World>();

        GenerateCombinations(new Role[Game.Instance.GameConfig.Players], 0, numbers, counts, uniqueArrays);

        return uniqueArrays;
    }

    private static void GenerateCombinations(Role[] curArray, int i, List<Role> numbers, Dictionary<Role, int> counts, List<World> uniqueArrays)
    {
        if (i == curArray.Length)
        {
            if (counts.Any(kvp => curArray.Count(num => num == kvp.Key) != kvp.Value)) return;
            
            var world = new World(curArray.Select((role, playerIndex) =>
                new PossiblePlayer(role, Game.Instance.Players.ElementAt(playerIndex))
            ).ToList());
            uniqueArrays.Add(world);

            return;
        }

        foreach (var num in numbers)
        {
            curArray[i] = num;
            GenerateCombinations(curArray, i + 1, numbers, counts, uniqueArrays);
        }
    }

    public static void MoveWorldsToPlayers(List<World> worlds)
    {
        foreach (var player in Game.Instance.Players)
        {
            player.PossibleWorlds = worlds;
        }
    }

    public static void UpdateWorldsByMessage(Message message)
    {
        foreach (var player in Game.Instance.Players)
        {
            UpdatePossibleWorlds(player, message);
        }
    }

    public static void UpdatePossibleWorlds(Player player, Message message)
    {
        foreach (World world in player.PossibleWorlds) 
        {
            if (world.isActive) 
            {
                // Create the accusation
                // TODO this can be done better
                Accusations newAccusation = new Accusations {
                    Accuser = message.Me,
                    Acussee = message.Accused,
                    Message = message
                };

                world.Accusations.Add(newAccusation);              
            }
        }
    }
}