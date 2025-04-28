using Shared;

namespace Backend.Repositories;

public interface IRedisBridgeTableRepository
{
    Task<List<BridgeTable>> GetAllBridgeTablesAsync();
    Task<BridgeTable?> GetBridgeTableByIdAsync(long bridgeTableId);
    Task<List<string>?> GetListOfBridgeTablePalyersIdsAsync(long bridgeTableId);
    Task<string?> GetTableAdminIdAsync(long bridgeTableId);
    Task<BridgeTable> SaveBridgeTableAsync(BridgeTable table);
    Task UpdateListOfBridgeTablePlayersIdsAsync(long bridgeTableId, List<string> playersIds);
    Task<bool> DeleteBridgeTableAsync(long bridgeTableId);
}
