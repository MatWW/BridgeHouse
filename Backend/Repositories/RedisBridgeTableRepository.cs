using StackExchange.Redis;
using Shared;
using System.Text.Json;

namespace Backend.Repositories;

public class RedisBridgeTableRepository : IRedisBridgeTableRepository
{
    private readonly IDatabase redisDb;

    public RedisBridgeTableRepository(IConnectionMultiplexer redis)
    {
        redisDb = redis.GetDatabase();
    }

    public async Task<List<BridgeTable>> GetAllBridgeTablesAsync()
    {
        List<long> tablesIds = await GetListOfBridgeTablesIdsAsync();
        List<BridgeTable> tables = [];

        foreach (var tableId in tablesIds)
        {
            BridgeTable? table = await GetBridgeTableByIdAsync(tableId);

            if (table is not null)
            {
                tables.Add(table);
            }
        }

        return tables;
    }

    public async Task<BridgeTable?> GetBridgeTableByIdAsync(long bridgeTableId)
    {
        var hashEntries = await redisDb.HashGetAllAsync($"bridgeTable:{bridgeTableId}");

        if (hashEntries.Length == 0)
        {
            await redisDb.SetRemoveAsync("bridgeTable:ids", bridgeTableId);
            return null;
        }

        var id = hashEntries.FirstOrDefault(x => x.Name == "Id");
        var adminId = hashEntries.FirstOrDefault(x => x.Name == "AdminId");
        var numberOfDeals = hashEntries.FirstOrDefault(x => x.Name == "NumberOfDeals");
        var playersIds = hashEntries.First(x => x.Name == "PlayersIds");

        if (id.Equals(default) || adminId.Equals(default) || numberOfDeals.Equals(default) || playersIds.Equals(default))
        {
            await redisDb.SetRemoveAsync("bridgeTable:ids", bridgeTableId);
            return null;
        }

        return new BridgeTable
        {
            Id = long.Parse(id.Value!),
            AdminId = adminId.Value!,
            NumberOfDeals = int.Parse(numberOfDeals.Value!),
            PlayersIds = JsonSerializer.Deserialize<List<string>>(playersIds.Value!)!
        };
    }

    public async Task<List<string>?> GetListOfBridgeTablePalyersIdsAsync(long bridgeTableId)
    {
        var redisPlayersIds = await redisDb.HashGetAsync($"bridgeTable:{bridgeTableId}", "playersIds");

        if (redisPlayersIds.IsNull)
        {
            return null;
        }

        //if redisPlayersIds is empty JsonSerializer will return null
        List<string>? playersIds = JsonSerializer.Deserialize<List<string>>(redisPlayersIds!);

        return playersIds ?? [];
    }

    public async Task<string?> GetTableAdminIdAsync(long bridgeTableId)
    {
        var redisAdminId = await redisDb.HashGetAsync($"bridgeTable:{bridgeTableId}", "adminId");

        if (redisAdminId.IsNull)
        {
            return null;
        }

        string? adminId = JsonSerializer.Deserialize<string>(redisAdminId!);

        return adminId ?? string.Empty;
    }

    public async Task<BridgeTable> SaveBridgeTableAsync(BridgeTable table)
    { 
        table.Id ??= await redisDb.StringIncrementAsync("bridgeTable:id");

        string key = $"table:{table.Id}";
        string playersIdsAsJson = JsonSerializer.Serialize(table.PlayersIds);

        HashEntry[] hashEntry =
        [
            new ("adminId", table.AdminId),
            new ("playersIds", playersIdsAsJson),
            new ("numberOfDeals", table.NumberOfDeals),
        ];
        await redisDb.HashSetAsync(key, hashEntry);

        await redisDb.SetAddAsync("bridgeTable:ids", table.Id);

        return table;
    }

    public async Task UpdateListOfBridgeTablePlayersIdsAsync(long tableId, List<string> playersIds)
    {
        string updatedPlayersIdsJson = JsonSerializer.Serialize(playersIds);

        await redisDb.HashSetAsync($"bridgeTable:{tableId}",
        [
            new HashEntry($"bridgeTable:{tableId}", updatedPlayersIdsJson)
        ]);
    }

    public async Task<bool> DeleteBridgeTableAsync(long tableId)
    {
        bool deleted = await redisDb.KeyDeleteAsync($"bridgeTable:{tableId}");

        if (deleted)
        {
            await redisDb.SetRemoveAsync("bridgeTable:ids", tableId);
        }

        return deleted;
    }

    public async Task<List<long>> GetListOfBridgeTablesIdsAsync()
    {
        var tablesIds = await redisDb.SetMembersAsync("bridgeTable:ids");
        return tablesIds.Select(tableId => (long)tableId).ToList();
    }
}
