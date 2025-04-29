using Shared;
using Shared.Enums;

namespace Backend.Services;

public interface IBridgeTablesService
{
    public Task<List<BridgeTable>> GetAllBridgeTablesAsync();
    public Task<BridgeTable> GetBridgeTableByIdAsync(long bridgeTableId);
    public Task AddUserToBridgeTableAsync(long bridgeTableId, string userId, Position position);
    public Task RemoveUserFromBridgeTableAsync(long bridgeTableId, string userId);
    public Task<BridgeTable> CreateBridgeTableAsync(int numberOfDeals);
    public Task DeleteBridgeTableAsync(long bridgeTableId);
}
