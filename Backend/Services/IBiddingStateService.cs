using Shared;

namespace Backend.Services;

public interface IBiddingStateService
{
    Task<BiddingState?> GetCurrentBiddingStateAsync();

    Task SetCurrentBiddingStateAsync(BiddingState biddingState);
}
