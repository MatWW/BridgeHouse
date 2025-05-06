using Shared;
using Shared.Enums;

namespace Backend.Comparers;

public class PlayerComparer : IEqualityComparer<Player>
{
    public bool Equals(Player? p1, Player? p2)
    {
        return p1 is not null && p2 is not null &&
           p1.PlayerId == p2.PlayerId &&
           p1.Nickname == p2.Nickname &&
           p1.Position == p2.Position;
    }

    public int GetHashCode(Player obj) => obj.PlayerId.GetHashCode();
}
