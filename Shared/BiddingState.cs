namespace Shared;

public class BiddingState
{
    public List<BidAction> BidActions { get; set; } = new();
    public Contract? Contract { get; set; } 
}
