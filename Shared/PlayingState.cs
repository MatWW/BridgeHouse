namespace Shared;

public class PlayingState
{
    public PlayerInfo Declarer { get; set; } = new();
    public PlayerInfo Dummy { get; set; } = new();
    public List<CardPlayAction> CardPlayActions { get; set; } = new();
    public List<Card> CardsOnTable { get; set; } = new();
    public int NSTricks;
    public int EWTricks;
}
