using System.ComponentModel.DataAnnotations;

namespace Shared;

public class BridgeTable
{
    public long? Id { get; set; }
    public string AdminId {  get; set; }
    public List<string> PlayersIds { get; set; }
    public int NumberOfDeals { get; set; }

}
