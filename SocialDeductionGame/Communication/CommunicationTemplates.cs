using SocialDeductionGame.Roles;
using SocialDeductionGame.Worlds;

namespace SocialDeductionGame.Communication;

public static class CommunicationTemplates
{
    static Message _yesResponse = new Message( MessageIntent.Response, "Yes");
    static Message _noResponse = new Message(MessageIntent.Response, "No");
    
    static Message _iAmVillager = new Message(MessageIntent.Response, "I am Villager");
    static Message _iAmSeer = new Message(MessageIntent.Response, "I am Seer");
    // TODO add rest of villager rolesmm

    public static List<Message> Messages = new List<Message>
    {
        new Message(
            MessageIntent.Defend,
            "{Me} say: I am {MyRole}",
            new List<Message> { }
        ),
        new Message(
            MessageIntent.Accuse,
            "{Me} say: {Accused} is {Role}",
            new List<Message> { }
        ),
        new Message(
            MessageIntent.Inquire,
            "{Me} say: What is your role, {Accused}?",
            new List<Message> { _iAmVillager, _iAmSeer }
        ),
        new Message(
            MessageIntent.Inquire,
            "{Me} say: {PlayerAsk} Do you believe {Accused} is {Role}?",
            new List<Message> { _yesResponse, _noResponse }
        ),

    };
}

