using SocialDeductionGame.Logic;
using SocialDeductionGame.Worlds;
using System.Numerics;

namespace SocialDeductionGame.Communication;

public class CommunicationManager
{
    public void Communicate(Player player)
    {
        // Probability check if player should communicate
        if (!ProbabilityManager.ShouldEventOccur("Communicate"))
        {
            if (Game.Instance.shouldPrint)
                Console.WriteLine($"Comm: {player.Name} decided not to communicate");
            return;
        }
        
        //List<World> possibleWorlds = player.PossibleWorlds;
        List<World> possibleWorlds = (from world in player.PossibleWorlds
                                     where world.IsActive
                                     select world).ToList();
        
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

        if (Game.Instance.shouldPrint)
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
        int Min = Int32.MaxValue;
        foreach (World possibleWorld in question.PlayerAsk.ActualPlayer.PossibleWorlds.Where(possibleWorld => possibleWorld.IsPrivateActive == true))
        {
            if (possibleWorld.Marks < Min)
            {
                Min = possibleWorld.Marks;
            }
        };

        World selectedWorld = question.PlayerAsk.ActualPlayer.PossibleWorlds.Find(possibleWorld => possibleWorld.Marks == Min);

        Message response = null;

        //ask for roles
        if (question.Type == 2)
        {
            //If mafia member
            if (!question.Accused.ActualPlayer.Role.IsTown && response == null)
            {
                Random random = new Random();
                response = question.Responses[random.Next(0, question.Responses.Count)];
                question.Response = response.Text;
            }
            else
                for (int i = 0;  i < question.Responses.Count; i++)
                {
                    if (question.Responses[i].Role.Name == question.Accused.ActualPlayer.Role.Name)
                    {
                        response = question.Responses[i];
                        question.Response = response.Text;
                        break;
                    }
                    
                }
        }
        //ask yes or no
        else if (response == null)
        {
            //If mafia member
            if (!question.PlayerAsk.ActualPlayer.Role.IsTown && response == null)
            {
                Random random = new Random();
                response = question.Responses[random.Next(0, question.Responses.Count)];
                question.Response = response.Text;
            }
            else
            {
                if (selectedWorld.PossiblePlayers.Find(player => player.PossibleRole.Name == question.Role.Name && player.Id == question.Accused.Id) != null)
                {
                    response = question.Responses[0];
                    question.Response = response.Text;
                }
                else
                {
                    response = question.Responses[1];
                    question.Response = response.Text;
                }
            }
            
            
        }
        
        
        
        if (Game.Instance.shouldPrint)
            Console.WriteLine($"Comm Response: {response.Text}");
        
        // Wait till response is added to update the world by message
        if (question.UpdateWorlds != null)
            question.UpdateWorlds(question);
    }
}