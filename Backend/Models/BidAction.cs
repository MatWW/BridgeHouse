namespace Backend.Models;

public class BidAction
{
    public Player Player { get; set; } = new();
    public Bid Bid { get; set; } = new();
}
