using SocialDeductionGame.Communication;

namespace SocialDeductionGame.Worlds;

public class Accusations
{
    public Player Accuser { get; set; }
    public PossiblePlayer Acussee { get; set; }
    public Message Message { get; set; }
}