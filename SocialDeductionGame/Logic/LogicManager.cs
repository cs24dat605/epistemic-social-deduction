
using SocialDeductionGame.Communication;
using SocialDeductionGame.Roles;
using SocialDeductionGame.Worlds;

namespace SocialDeductionGame.Logic;

public static class LogicManager
{
    // TODO avoid choosing yourself when saying that another player is another role
    // TODO avoid saying i am mafia etc
    public static (PossiblePlayer, int) GetHighestInformationGainPlayer(Player me, List<World> topWorlds)
    {
        if (nonAccusedPlayers.Count != 0 || Game.Instance.Round == 0)
        {
            (var pPlayer, int pWorldId) = GetNonAccusationPlayer(me);
            
            // 1. If player without accusation found return them
            if (pPlayer != null)
                return (pPlayer, pWorldId);
        }

        // 2. Otherwise, find player with most contradicting role information
        return GetPlayerWithMostContradictingInfo(topWorlds, me);
    }
    
    private static List<PossiblePlayer> nonAccusedPlayers = new List<PossiblePlayer>();

    public static (PossiblePlayer?, int) GetNonAccusationPlayer(Player me)
    {
        if (Game.Instance.Round != 0 && nonAccusedPlayers.Count == 0)
            return (null, -1);

        // List to store non-accused players (directly store PossiblePlayer objects)
        if (nonAccusedPlayers.Count == 0 && Game.Instance.Round == 0)
        {
            foreach (PossiblePlayer player in me.PossibleWorlds[0].PossiblePlayers)
            {
                nonAccusedPlayers.Add(player);
            }
        }

        // Remove accused players
        foreach (Message accusation in me.Accusations)
        {
            // Create a copy of the Accused object
            PossiblePlayer accusedPlayerCopy = new PossiblePlayer(accusation.Accused.PossibleRole, accusation.Accused.ActualPlayer)
            {
                IsAlive = accusation.Accused.IsAlive
            };

            // Find the player in the list using the copy
            PossiblePlayer playerToRemove = nonAccusedPlayers.Find(player => player.Id == accusedPlayerCopy.Id);

            // Remove the player if found
            if (playerToRemove != null)
            {
                nonAccusedPlayers.Remove(playerToRemove);
            }
        }

        // Skip self
        nonAccusedPlayers.Remove(me.PossibleWorlds[0].PossiblePlayers.First(p => p.Id == me.Id));

        if (nonAccusedPlayers.Count == 0)
            return (null, -1);

        // Randomly choose a non-accused player
        Random random = new Random();
        if (nonAccusedPlayers.Count != 0)
        {
            int randomIndex = random.Next(0, nonAccusedPlayers.Count);
            PossiblePlayer nonAccusedPlayer = nonAccusedPlayers[randomIndex];
            return (nonAccusedPlayer, 0);
        }

        return (null, -1);
        
        // bool[] nonAccusedPlayers = new bool[me.PossibleWorlds[0].PossiblePlayers.Count+1];
        // Array.Fill(nonAccusedPlayers, true);
        //
        // foreach (Message accusation in me.Accusations)
        // {
        //     nonAccusedPlayers[accusation.Accused.Id] = false;
        // }
        //
        // // Skip self
        // nonAccusedPlayers[me.Id] = false;
        //
        // // Random choose a accusedPlayer from the array and then return them
        // Random random = new Random();
        //
        // // Print count of true values in nonAccusedPlayers
        // Console.WriteLine(nonAccusedPlayers.Count(x => x));
        //
        // // foreach (var nAPlayer in nonAccusedPlayers)
        // //     Console.Write(Game.Instance.Players[nAPlayer] + " ");
        //
        // // Console.WriteLine("");
        //
        //
        // if (nonAccusedPlayers.Length > 0)
        // {
        //     PossiblePlayer nonAccusedPlayer = me.PossibleWorlds[0].PossiblePlayers[random.Next(0, nonAccusedPlayers.Length)];
        //     return (nonAccusedPlayer, 0);
        // }
        //
        // return (null, -1);
    }
    
    public static (PossiblePlayer, int) GetPlayerWithMostContradictingInfo(List<World> topWorlds, Player me)
    {
        // Dictionary to store role distribution for each player
        var roleCounts = new Dictionary<PossiblePlayer, Dictionary<Role, int>>();

        // Iterate through top worlds and count role occurrences for each player
        foreach (var world in topWorlds)
        {
            if (!world.IsActive)
                continue;
            
            foreach (var possiblePlayer in world.PossiblePlayers.Where(p => p.ActualPlayer != me && p.IsAlive))
            {
                if (!roleCounts.ContainsKey(possiblePlayer))
                    roleCounts[possiblePlayer] = new Dictionary<Role, int>();
                
                if (!roleCounts[possiblePlayer].ContainsKey(possiblePlayer.PossibleRole))
                    roleCounts[possiblePlayer][possiblePlayer.PossibleRole] = 0;
                
                roleCounts[possiblePlayer][possiblePlayer.PossibleRole]++;
            }
        }

        // Find the player with the most even distribution of roles and their world index
        PossiblePlayer mostContradictingPlayer = null;
        int mostContradictingWorldIndex = -1;
        double maxContradictionScore = -1;
        
        for (int i = 0; i < topWorlds.Count; i++) 
        {
            var world = topWorlds[i];
            foreach (var possiblePlayer in world.PossiblePlayers.Where(p => p.ActualPlayer != me && p.IsAlive))
            {
                // Calculate contradiction score as the ratio of the two most frequent roles
                int secondHighestCount = 0;
                int highestCount = 0;

                if (roleCounts.ContainsKey(possiblePlayer))
                {
                    highestCount = roleCounts[possiblePlayer].Values.Max();
                    
                    if (roleCounts[possiblePlayer].Values.Any(c => c != highestCount))
                        secondHighestCount = roleCounts[possiblePlayer].Values.Where(c => c != highestCount).Max();
                }
                
                double contradictionScore = (double)secondHighestCount / highestCount;

                if (contradictionScore > maxContradictionScore)
                {
                    maxContradictionScore = contradictionScore;
                    mostContradictingPlayer = possiblePlayer;
                    mostContradictingWorldIndex = i;
                }
            }
        }

        return (mostContradictingPlayer, mostContradictingWorldIndex);
    }
    
    
    
    public static PossiblePlayer GetPlayerToAsk(Player player, PossiblePlayer inquirePlayer, int pWorldID)
    {
        List<PossiblePlayer> players = player.PossibleWorlds[pWorldID].PossiblePlayers
            .Where(x => x != inquirePlayer && x.Name != player.Name)
            .ToList();
        
        Random random = new Random();
        
        return players[random.Next(0, players.Count)];
    }
    
    private static List<Player> defendedRoles = new List<Player>();
    
    public static Message ChooseQuestion(Player player, World topWorld)
    {
        // Defend if player is mafia in top world
        if (IsPlayerMafiaInTopWorld(player, topWorld))
        {
            // Only allow defending once
            if (defendedRoles.Contains(player))
            {
                defendedRoles.Add(player);
                
                Message defendMessage = CommunicationTemplates.Messages.First(t => t.Intent == MessageIntent.Defend);
                
                // Deep copy
                Message defendMessageCopy = new Message(defendMessage.Intent, defendMessage.Text, defendMessage.Responses, defendMessage.UpdateWorlds);
                return defendMessageCopy;
            }
        }
        
        // Otherwise, choose a random non-defend message
        Random random = new Random();

        var templates = CommunicationTemplates.Messages.Where(t => t.Intent != MessageIntent.Defend).ToArray();
        var template = templates[
            random.Next(0, templates.Count())
        ];
        
        // Deep copy
        var templateCopy = new Message(template.Intent, template.Text, template.Responses, template.UpdateWorlds);
        return templateCopy;
    }

    public static bool IsPlayerMafiaInTopWorld(Player player, World topWorld)
    {
        if (topWorld.PossiblePlayers.First(p => p.ActualPlayer == player).PossibleRole.IsTown)
            return false;
        
        return true;
    }
}