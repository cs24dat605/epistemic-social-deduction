namespace SocialDeductionGame.Worlds;

public class VotingPlayer
{
    public Player VotedPlayer { get; set; }

    public int Votes {  get; set; }

    public VotingPlayer(Player player,int Vote)
    {
        VotedPlayer = player;
        Votes = Vote;
    }
}