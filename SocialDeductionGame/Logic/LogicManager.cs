using SocialDeductionGame.Worlds;

namespace SocialDeductionGame.Logic;

public static class LogicManager
{
    public static PossiblePlayer GetHighestInformationGainPlayer(List<World> PossibleWorlds)
    {
        World targetWorld = PossibleWorlds.MaxBy(x => x.Marks);
        List<PossiblePlayer> unknownPlayer = new List<PossiblePlayer>(targetWorld.PossiblePlayers);
        
        // TODO Possiblity: Check for conflicting knowledge about player
        
        // Copy list to avoid reference modification
        List<PossiblePlayer> unknownPlayerCopy = new List<PossiblePlayer>(unknownPlayer);

        // Check world for if no information is know about the player
        foreach (var possiblePlayer in unknownPlayerCopy)
        {
            // Remove player for target list if we already have knowledge about them
            if (targetWorld.Accusations.Any(x => x.Accused == possiblePlayer))
            {
                unknownPlayer.Remove(possiblePlayer);
            }
        }
        
        // If every player has been accused of something reset list
        if (unknownPlayerCopy.Count == 0)
        {
            unknownPlayerCopy = new List<PossiblePlayer>(targetWorld.PossiblePlayers);
        }
        
        Random random = new Random();
        
        return unknownPlayerCopy[random.Next(0, unknownPlayerCopy.Count)];
    }
}