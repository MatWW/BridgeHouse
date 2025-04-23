using Backend.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Shared;

namespace Backend.Services;

public class BiddingStateService : IBiddingStateService
{
    private readonly IDistributedCache cache;

    public BiddingStateService(IDistributedCache cache)
    {
        this.cache = cache;
    }

    public async Task<BiddingState?> GetCurrentBiddingStateAsync()
    {
        return await cache.GetRecordAsync<BiddingState>("biddingState");
    }

    public async Task SetCurrentBiddingStateAsync(BiddingState biddingState)
    {
        await cache.SetRecordAsync<BiddingState>("biddingState", biddingState);
    }
}
