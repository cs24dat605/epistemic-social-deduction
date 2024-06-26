using SocialDeductionGame.Logic;
using SocialDeductionGame.Roles;
using SocialDeductionGame.Worlds;

namespace SocialDeductionGame.Communication;

public class Message
{
    public MessageIntent Intent { get; }
    private string Template { get; }
    public List<Message> Responses { get; }
    
    public Action<Message> UpdateWorlds { get; }
    public int Type { get; }
    
    public string Response { get; set; }

    public Role Role { get; set; }
    public Player Accuser { get; set; } // Player who sends the message
    public PossiblePlayer Accused { get; set; }
    public PossiblePlayer PlayerAsk { get; set; }

    public string Text { get; set; }

    public Message(MessageIntent intent, string template,
        List<Message> responses = null, Action<Message> updateWorlds = null, int type = -1,
        Role role = null, Player accuser = null, PossiblePlayer accused = null,
        PossiblePlayer playerAsk = null
    )
    {
        Intent = intent;
        Template = template;
        UpdateWorlds = updateWorlds;
        Type = type;

        // Assign values to the class properties
        Role = role;
        Accuser = accuser;
        Accused = accused;
        PlayerAsk = playerAsk;

        Responses = responses ?? new List<Message>();

        // Generate the Text
        Text = GenerateText();
    }

    public string GenerateText()
    {
        // if (Intent == MessageIntent.Response)
        //     return Template;

        string messageText = Template;

        if (Accuser != null)
        {
            if (!Accuser.Role.IsTown)
            {
                Role claimRole = LogicManager.GetClaimRoleMafia(Accuser);
                messageText = messageText.Replace("{MyRole}", claimRole.Name);
            }
            else
            {
                messageText = messageText.Replace("{MyRole}", Accuser.Role.Name);
            }
            
            messageText = messageText.Replace("{Me}", Accuser.Name);
        }

        if (Role != null)
            messageText = messageText.Replace("{Role}", Role.Name);

        if (Accused != null)
            messageText = messageText.Replace("{Accused}", Accused.Name);

        if (PlayerAsk != null)
            messageText = messageText.Replace("{PlayerAsk}", PlayerAsk.Name);

        return messageText;
    }
}