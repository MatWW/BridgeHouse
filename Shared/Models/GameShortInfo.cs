using Shared.Enums;

namespace Shared.Models;

public class GameShortInfo
{
    public Bid? FinalContract { get; set; }
    public bool IsDoubled { get; set; }
    public bool IsRedoubled { get; set; }
    public int TrickBalance { get; set; }
    public Position? DeclarerPosition { get; set; }

}
