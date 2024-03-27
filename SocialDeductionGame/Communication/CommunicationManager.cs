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
        
        Random random = new Random();
        Message question = CommunicationTemplates.Messages[random.Next(0, CommunicationTemplates.Messages.Count)];
        
        question.Me = player;
        question.Accused = inquirePlayer;
        
        List<PossiblePlayer> players = player.PossibleWorlds[0].PossiblePlayers;
        players.Remove(inquirePlayer);
        players = players.Where(x => x.Name != player.Name).ToList(); 


        if (players.Count > 0) {
            question.PlayerAsk = players[random.Next(0, players.Count)];
        } else {
            Console.WriteLine("ERORR ERROR");
        }
        
        question.Role = inquirePlayer.PossibleRole;

        Console.WriteLine($"{question.GenerateText()}");

        WorldManager.UpdateWorldsByMessage(question);
    }
}