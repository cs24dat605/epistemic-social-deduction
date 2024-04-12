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

    private static void GenerateCombinations(Role[] curArray, int i, List<Role> numbers, Dictionary<Role, int> counts,
        List<World> uniqueArrays)
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