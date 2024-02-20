namespace Werewolf;

public class World
{
    // it complains about PlayerRolePossibilities not being intialized fix it
    
    public Dictionary<Player, HashSet<Role>> PlayerRolePossibilities { get; set; }
    
    public List<World> GeneratePossibleWorlds(List<Player> players, int werewolves, int seers)
    {
        var worlds = new List<World>();
        RecursiveWorldGenerator(worlds, new World(), players, werewolves, seers, 0);
        return worlds;
    }

    private void RecursiveWorldGenerator(List<World> worlds, World currentWorld, List<Player> players, int werewolvesLeft, int seersLeft, int playerIndex)
    {
        // If all roles add
        if (werewolvesLeft == 0 && seersLeft == 0)
        {
            worlds.Add(currentWorld);
            return;
        }

        // If we added all players
        if (playerIndex == players.Count)
        {
            return;
        }

        if (werewolvesLeft > 0)
        {
            currentWorld.PlayerRolePossibilities[players[playerIndex]] = new HashSet<Role>{ new Roles.Werewolf() };
            RecursiveWorldGenerator(worlds, currentWorld, players, werewolvesLeft - 1, seersLeft, playerIndex + 1);
        }

        // Add other classes such as seer

        // Villager (the 'else' scenario)
        currentWorld.PlayerRolePossibilities[players[playerIndex]] = new HashSet<Role>{ new Roles.Villager() }; 
        RecursiveWorldGenerator(worlds, currentWorld, players, werewolvesLeft, seersLeft, playerIndex + 1);
    }
}