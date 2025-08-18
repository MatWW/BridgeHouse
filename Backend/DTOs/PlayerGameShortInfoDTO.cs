using Backend.Enums;
using Backend.Models;

namespace Backend.DTOs;

public class PlayerGameShortInfoDTO
{
    public GameShortInfo GameShortInfo { get; set; } = new();
    public Position UserPosition { get; set; }
}
