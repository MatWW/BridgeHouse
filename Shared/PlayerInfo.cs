using Shared.Enums;

namespace Shared;

public class PlayerInfo
{
    public string PlayerId { get; set; } = string.Empty;
    public string UserName { get; set;} = string.Empty;
    public Position Position { get; set; }
}
