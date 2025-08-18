namespace Backend.Models;

public class Contract
{
    public BidAction BidAction { get; set; } = new();
    public bool IsDoubled { get; set; }
    public bool IsRedoubled { get; set; }
}
