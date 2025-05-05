using Shared;

namespace Backend.Services;

public interface IBiddingService
{
    Task PlaceBid(long gameId, BidAction bidAction);
}
