using SocialDeductionGame.Worlds;

namespace SocialDeductionGame.Logic;

public static class LogicManager
{
    public static PossiblePlayer GetHighestInformationGainPlayer(List<World> PossibleWorlds)
    {
        World targetWorld = PossibleWorlds.MaxBy(x => x.Marks);
        
        // TODO Possiblity: Check for conflicting knowledge about player
        
        // Copy list to avoid reference modification
        List<PossiblePlayer> unknownPlayerCopy = new List<PossiblePlayer>(targetWorld.PossiblePlayers);
        
        List<PossiblePlayer> toRemove = new List<PossiblePlayer>();

        // Check world for if no information is know about the player
        foreach (var possiblePlayer in unknownPlayerCopy)
        {
            // Remove player for target list if we already have knowledge about them
            if (targetWorld.Accusations.Any(x => x.Accused == possiblePlayer))
            {
                toRemove.Add(possiblePlayer);
            }
        }
        unknownPlayerCopy.RemoveAll(player => toRemove.Contains(player)); // Remove from original list
        
        // If every player has been accused of something reset list
        if (unknownPlayerCopy.Count == 0)
        {
            unknownPlayerCopy = new List<PossiblePlayer>(targetWorld.PossiblePlayers);
        }

        Random random = new Random();
        return unknownPlayerCopy[random.Next(0, unknownPlayerCopy.Count)];
    }
}