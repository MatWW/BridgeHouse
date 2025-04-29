using Shared;

namespace Backend.Services;

public interface IGameService
{
    Task PlaceBid(long gameId, BidAction bidAction);
}
