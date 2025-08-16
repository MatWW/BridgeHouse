using Shared.Enums;

namespace Shared.Models;

public class UserStateDTO
{
    public UserStatus UserStatus { get; set; }
    public Invitation? Invitation { get; set; }
    public long? TableId { get; set; }
    public long? GameId { get; set; }
}
