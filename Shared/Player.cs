using Shared.Enums;

namespace Shared;

public class Player
{
    public string PlayerId { get; set; } = string.Empty;
    public string Nickname { get; set;} = string.Empty;
    public Position Position { get; set; }
}
