using SocialDeductionGame.Logic;
using SocialDeductionGame.Worlds;

namespace SocialDeductionGame.Communication;

public class CommunicationManager
{
    public void Communicate(Player player)
    {
        // TODO change depending on if is town or mafia
        PossiblePlayer inquirePlayer = LogicManager.GetHighestInformationGainPlayer(player.PossibleWorlds);

        // TODO maybe ask contradiction information here
        // TODO maybe choose specific question later
        
        
        // TODO if player has explicit knowledge maybe leverage it?
        
        List<PossiblePlayer> players = new List<PossiblePlayer>(player.PossibleWorlds[0].PossiblePlayers);
        players.Remove(inquirePlayer);
        players = players.Where(x => x.Name != player.Name).ToList(); 
        
        Random random = new Random();
        Message question = CommunicationTemplates.Messages[random.Next(0, CommunicationTemplates.Messages.Count)];
        
        question.Accuser = player;
        question.Accused = inquirePlayer;
        question.PlayerAsk = players[random.Next(0, players.Count)];
        question.Role = inquirePlayer.PossibleRole;

        Console.WriteLine($"Communication: {question.GenerateText()}");

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
        question.Response = question.Responses[random.Next(0, question.Responses.Count)].Text;
        Console.WriteLine($"Communication Response: {question.Response}");
        
        // Wait till response is added to update the world by message
        if (question.UpdateWorlds != null)
            question.UpdateWorlds(question);
    }
}