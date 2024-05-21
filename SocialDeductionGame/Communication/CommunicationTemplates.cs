using SocialDeductionGame.Roles;
using static SocialDeductionGame.Worlds.WorldManager;
using SocialDeductionGame;

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
        Game.Instance.GameConfig.Villagers!=0?new Message(MessageIntent.Response, "I am Villager", null, (m) => UpdateWorldsByMessage(m), 2, new Villager()):null,
        Game.Instance.GameConfig.Sheriffs!=0?new Message(MessageIntent.Response, "I am Sheriff", null, (m) => UpdateWorldsByMessage(m), 2, new Sheriff()):null,
        Game.Instance.GameConfig.Escort!=0?new Message(MessageIntent.Response, "I am Escort", null, (m) => UpdateWorldsByMessage(m), 2, new Escort()):null,
        Game.Instance.GameConfig.Veteran!=0?new Message(MessageIntent.Response, "I am Veteran", null, (m) => UpdateWorldsByMessage(m), 2, new Veteran()):null,
        Game.Instance.GameConfig.Vigilante!=0?new Message(MessageIntent.Response, "I am Vigilante", null, (m) => UpdateWorldsByMessage(m), 2, new Vigilante()):null,
        Game.Instance.GameConfig.Doctor!=0?new Message(MessageIntent.Response, "I am Doctor", null, (m) => UpdateWorldsByMessage(m), 2, new Doctor()):null,
        Game.Instance.GameConfig.Investigator!=0?new Message(MessageIntent.Response, "I am Investigator", null, (m) => UpdateWorldsByMessage(m), 2, new Investigator()):null,
    }.Where(message => message != null).ToList();


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
            roleResponse,
            (m) => UpdateWorldsByMessage(m),
            2
        ),
        new Message(
            MessageIntent.Inquire,
            "{Me} say: {PlayerAsk} Do you believe {Accused} is {Role}?",
            yesNoResponse,
            (m) => UpdateWorldsByMessage(m),
            3
        ),

    };
}

