namespace SocialDeductionGame.Worlds;

public class VotingPlayer
{
    private int _votes;
    public Player VotedPlayer { get; set; }

    public int Votes => Interlocked.CompareExchange(ref _votes, 0, 0);

    public int IncrementVote()
    {
        Interlocked.Increment(ref _votes);
        return _votes;
    }
    

    public VotingPlayer(Player player,int vote)
    {
        VotedPlayer = player;
        Interlocked.Exchange(ref _votes, vote);
    }
}