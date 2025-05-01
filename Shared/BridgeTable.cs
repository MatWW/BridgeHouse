namespace Shared;

public class BridgeTable
{
    public long? Id { get; set; }
    public string AdminId { get; set; } = string.Empty;
    public List<Player> Players { get; set; } = [];
    public int NumberOfDeals { get; set; }
    public List<long> DealsIds { get; set; } = [];
}
