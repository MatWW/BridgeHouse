using Shared;
using Shared.Enums;

namespace Backend.Services;

public interface IBridgeTablesService
{
    public Task<List<BridgeTable>> GetAllBridgeTablesAsync();
    public Task<BridgeTable> GetBridgeTableByIdAsync(long bridgeTableId);
    public Task<BridgeTable> CreateBridgeTableAsync(CreateBridgeTableRequestDTO numberOfDeals);
    public Task AddUserToBridgeTableAsync(long bridgeTableId, string userId, Position position);
    public Task RemoveUserFromBridgeTableAsync(long bridgeTableId, string userId);
    public Task InviteUserToBridgeTableAsync(long bridgeTableId, string userId, Position position);
    public Task AcceptInviteToBridgeTableAsync();
    public Task DeclineInviteToBridgeTableAsync();
    Task LeaveTableAsync(long bridgeTableId);
    public Task DeleteBridgeTableAsync(long bridgeTableId);
}
