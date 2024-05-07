using SocialDeductionGame.Roles;
using static SocialDeductionGame.Worlds.WorldManager;

namespace SocialDeductionGame.Communication;

public static class CommunicationTemplates
{
    private static List<Message> yesNoResponse = new List<Message>
    {
        new Message( MessageIntent.Response, "Yes", null, (m) => UpdateWorldsByMessage(m), 3),
        new Message(MessageIntent.Response, "No", null, (m) => UpdateWorldsByMessage(m), 3)
    };
    
    private static List<Message> roleResponse = new List<Message>
    {
        new Message(MessageIntent.Response, "I am Villager", null, (m) => UpdateWorldsByMessage(m, 2), new Villager()),
        new Message(MessageIntent.Response, "I am Sheriff", null, (m) => UpdateWorldsByMessage(m, 2), new Sheriff()),
        new Message(MessageIntent.Response, "I am Escort", null, (m) => UpdateWorldsByMessage(m, 2), new Escort()),
        new Message(MessageIntent.Response, "I am Veteran", null, (m) => UpdateWorldsByMessage(m, 2), new Veteran()),
        new Message(MessageIntent.Response, "I am Vigilante", null, (m) => UpdateWorldsByMessage(m, 2), new Vigilante()),
        new Message(MessageIntent.Response, "I am Doctor", null, (m) => UpdateWorldsByMessage(m, 2), new Doctor()),
        new Message(MessageIntent.Response, "I am Investigator", null, (m) => UpdateWorldsByMessage(m, 2), new Investigator()),
    };

    public static List<Message> Messages = new List<Message>
    {
        new Message(
            MessageIntent.Defend,
            "{Me} say: I am {MyRole}",
            new List<Message> { },
            (m) => UpdateWorldsByMessage(m),
            0
        ),
        new Message(
            MessageIntent.Accuse,
            "{Me} say: {Accused} is {Role}",
            new List<Message> { },
            (m) => UpdateWorldsByMessage(m),
            1
        ),
        new Message(
            MessageIntent.Inquire,
            "{Me} say: What is your role, {Accused}?",
            roleResponse
        ),
        new Message(
            MessageIntent.Inquire,
            "{Me} say: {PlayerAsk} Do you believe {Accused} is {Role}?",
            yesNoResponse
        ),

    };
}

