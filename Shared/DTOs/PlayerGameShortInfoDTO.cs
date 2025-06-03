using Shared.Enums;
using Shared.Models;

namespace Shared.DTOs;

public class PlayerGameShortInfoDTO
{
    public GameShortInfo GameShortInfo { get; set; } = new();
    public Position UserPosition { get; set; }
}
