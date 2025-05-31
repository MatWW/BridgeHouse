using Shared.Enums;

namespace Shared;

public class PlayerGameShortInfoDTO
{
    public GameShortInfo GameShortInfo { get; set; } = new();
    public Position UserPosition { get; set; }
}
