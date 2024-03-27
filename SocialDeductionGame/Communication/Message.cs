using SocialDeductionGame.Roles;
using SocialDeductionGame.Worlds;

namespace SocialDeductionGame.Communication;

public class Message
{
    public MessageIntent Intent { get; set; }
    private string Template { get; set; }
    public List<Message> Responses { get; set; } 
    
    public Player Me { get; set; } // Player who sends the message
    public PossiblePlayer Accused { get; set; } 
    public PossiblePlayer PlayerAsk { get; set; } 
    public Role Role { get; set; } 
    
    public string Text { get; set; }
    
    public Message(MessageIntent intent, string template, 
        List<Message> responses = null, Player me = null,
        PossiblePlayer accused = null, 
        PossiblePlayer playerAsk = null, Role role = null
        ) 
    { 
        Intent = intent; 
        Template = template; 

        // Assign values to the class properties
        Me = me;
        Accused = accused;
        PlayerAsk = playerAsk;
        Role = role;

        Responses = responses ?? new List<Message>(); 

        // Generate the Text (explained below)
        Text = GenerateText(); 
    } 
    
    public string GenerateText() 
    {
        string messageText = Template;

        if (Me != null)
        {
            messageText = messageText.Replace("{MyRole}", Me.Role.Name);
            messageText = messageText.Replace("{Me}", Me.Name);
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