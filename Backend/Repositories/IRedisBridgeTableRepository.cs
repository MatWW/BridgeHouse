using Shared;

namespace Backend.Repositories;

public interface IRedisBridgeTableRepository
{
    Task<List<BridgeTable>> GetAllBridgeTablesAsync();
    Task<BridgeTable?> GetBridgeTableByIdAsync(long bridgeTableId);
    Task<List<Player>?> GetListOfBridgeTablePalyersAsync(long bridgeTableId);
    Task<string?> GetTableAdminIdAsync(long bridgeTableId);
    Task<BridgeTable> SaveBridgeTableAsync(BridgeTable table);
    Task UpdateListOfBridgeTablePlayersAsync(long bridgeTableId, List<Player> players);
    Task<bool> DeleteBridgeTableAsync(long bridgeTableId);
}
