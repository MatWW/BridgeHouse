namespace Shared;
public class BiddingState
{
    public int LowestClub { get; set; } = 1;
    public int LowestDiamond { get; set; } = 1;
    public int LowestHeart { get; set; } = 1;
    public int LowestSpade { get; set; } = 1;
    public int LowestNoTrump { get; set; } = 1;

    public Player StartingPlayer { get; set; } = Player.S;
    public Player CurrentPlayer { get; set; } = Player.S;

    public List<KeyValuePair<int?, string>> Bids { get; set; } = new();
    public (int, string) CurrentWinningBid { get; set; } = (0, "");

    public bool IsDoubled { get; set; } = false;
    public bool IsRedoubled { get; set; } = false;
    public bool IsUndoubledBidPlaced { get; set; } = false;
    public Player WhoPlacedLastBid { get; set; } = Player.NONE;
}

