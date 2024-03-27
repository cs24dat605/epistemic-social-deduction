using SocialDeductionGame.Worlds;

namespace SocialDeductionGame.Logic;

public static class LogicManager
{
    public static PossiblePlayer GetHighestInformationGainPlayer(List<World> PossibleWorlds)
    {
        World targetWorld = PossibleWorlds.MaxBy(x => x.PossibleScore);
        List<PossiblePlayer> unknownPlayer = targetWorld.PossiblePlayers;
        
        // TODO Possiblity: Check for conflicting knowledge about player
        
        // Check world for if no information is know about the player
        foreach (var possiblePlayer in unknownPlayer)
        {
            // Remove player for target list if we already have knowledge about them
            if (targetWorld.Accusations.Any(x => x.Acussee == possiblePlayer))
            {
                unknownPlayer.Remove(possiblePlayer);
            }
        }

        Console.WriteLine($"daskdlkas: {unknownPlayer.Count}");
        
        // If every player has been accused of something reset list
        if (unknownPlayer.Count == 0)
        {
            unknownPlayer = targetWorld.PossiblePlayers;
        }
        
        Console.WriteLine($"daskdlkas: {targetWorld.PossiblePlayers.Count}");
        
        Random random = new Random();
        
        return unknownPlayer[random.Next(0, unknownPlayer.Count)];
    }
}