using SocialDeductionGame.Logic;
using SocialDeductionGame.Worlds;

namespace SocialDeductionGame.Communication;

public class CommunicationManager
{
    public void Communicate(Player player)
    {
        // Probability check if player should communicate
        if (!ProbabilityManager.ShouldEventOccur("Communicate"))
        {
            Console.WriteLine($"Comm: {player.Name} decided not to communicate");
            return;
        }
        
        List<World> possibleWorlds = player.PossibleWorlds;
        
        int numWorlds = (int)Math.Ceiling(possibleWorlds.Count * 0.01);

        // Sort and get 1% of worlds
        var topWorlds = possibleWorlds
            .OrderByDescending(world => world.Marks)
            .Take(numWorlds)
            .ToList();
        
        
        // TODO maybe choose specific question later
        // TODO if player has explicit knowledge leverage it

        Message question = LogicManager.ChooseQuestion(player, topWorlds[0]);
        question.Accuser = player;

        if (question.Intent == MessageIntent.Defend)
        {
            question.Role = player.Role;
        }
        else
        {
            // TODO change depending on if is town or mafia
            // if (player.Role.IsTown)
            var (inquirePlayer, pWorldId) = LogicManager.GetHighestInformationGainPlayer(player, topWorlds);
            question.Accused = inquirePlayer;
            question.PlayerAsk = LogicManager.GetPlayerToAsk(player, inquirePlayer, pWorldId);
            question.Role = inquirePlayer.PossibleRole;
            // else
            //        (inquirePlayer, pWorldId) = LogicManager.GetLeastInformationGainPlayer(player.PossibleWorlds);
        }

        Console.WriteLine($"Comm: {question.GenerateText()}");

        if (question.Responses != null && question.Responses.Count > 0)
        {
            RequestResponse(question);
            
            // Handle UpdateWorldsByMessage in RequestResponse
            return;
        }

        // Call the custom defined UpdateWorlds function
        if (question.UpdateWorlds != null)
            question.UpdateWorlds(question);


        // WorldManager.UpdateWorldsByMessage(question);
    }

    public void RequestResponse(Message question)
    {
        Random random = new Random();
        Message response = question.Responses[random.Next(0, question.Responses.Count)];
        question.Response = response.Text;
        Console.WriteLine($"Comm Response: {response.Text}");
        
        // Wait till response is added to update the world by message
        if (question.UpdateWorlds != null)
            question.UpdateWorlds(question);
    }
}