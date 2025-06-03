using Shared.Models;

namespace Backend.Services;

public interface IBiddingService
{
    Task PlaceBidAsync(long gameId, BidAction bidAction);
}
