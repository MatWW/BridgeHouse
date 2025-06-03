using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Shared.Models;

namespace Backend.Repositories;

public class RedisBridgeTableRepository : IRedisBridgeTableRepository
{
    private readonly IDatabase _redisDb;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private static string TableKey(long tableId) => $"bridgeTable:{tableId}";

    public RedisBridgeTableRepository(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task<List<BridgeTable>> GetAllBridgeTablesAsync()
    {
        List<long> tablesIds = await GetListOfBridgeTablesIdsAsync();
        List<BridgeTable> tables = [];

        foreach (var tableId in tablesIds)
        {
            BridgeTable? table = await GetBridgeTableByIdAsync(tableId);
            if (table is not null) tables.Add(table);
        }

        return tables;
    }

    public async Task<BridgeTable?> GetBridgeTableByIdAsync(long bridgeTableId)
    {
        var hashEntries = await _redisDb.HashGetAllAsync(TableKey(bridgeTableId));

        if (hashEntries.Length == 0)
        {
            await _redisDb.SetRemoveAsync("bridgeTable:ids", bridgeTableId);
            return null;
        }

        var adminId = hashEntries.FirstOrDefault(x => x.Name == "adminId");
        var numberOfDeals = hashEntries.FirstOrDefault(x => x.Name == "numberOfDeals");
        var players = hashEntries.FirstOrDefault(x => x.Name == "players");
        var dealsIds = hashEntries.FirstOrDefault(x => x.Name == "dealsIds");

        if (adminId.Equals(default) || numberOfDeals.Equals(default) || players.Equals(default) || dealsIds.Equals(default))
        {
            await _redisDb.SetRemoveAsync("bridgeTable:ids", bridgeTableId);
            return null;
        }

        return new BridgeTable
        {
            Id = bridgeTableId,
            AdminId = adminId.Value!,
            NumberOfDeals = int.Parse(numberOfDeals.Value!),
            Players = JsonSerializer.Deserialize<List<Player>>(players.Value!, _jsonOptions)!,
            DealsIds = JsonSerializer.Deserialize<List<long>>(dealsIds.Value!)!,
        };
    }

    public async Task<List<Player>?> GetListOfBridgeTablePalyersAsync(long bridgeTableId)
    {
        var redisPlayers = await _redisDb.HashGetAsync(TableKey(bridgeTableId), "players");

        return redisPlayers.IsNull ? null : JsonSerializer.Deserialize<List<Player>>(redisPlayers!, _jsonOptions);
    }

    public async Task<string?> GetTableAdminIdAsync(long bridgeTableId)
    {
        var redisAdminId = await _redisDb.HashGetAsync(TableKey(bridgeTableId), "adminId");

        return redisAdminId.IsNull ? null : redisAdminId.ToString();
    }

    public async Task<BridgeTable> SaveBridgeTableAsync(BridgeTable table)
    { 
        table.Id ??= await _redisDb.StringIncrementAsync("bridgeTable:id");
        
        string playersAsJson = JsonSerializer.Serialize(table.Players, _jsonOptions);
        string dealsIdsAsJson = JsonSerializer.Serialize(table.DealsIds);

        HashEntry[] hashEntry =
        [
            new ("adminId", table.AdminId),
            new ("players", playersAsJson),
            new ("numberOfDeals", table.NumberOfDeals),
            new ("dealsIds", dealsIdsAsJson)
        ];
        await _redisDb.HashSetAsync(TableKey(table.Id.Value), hashEntry);

        await _redisDb.SetAddAsync("bridgeTable:ids", table.Id);

        return table;
    }

    public async Task UpdateListOfBridgeTablePlayersAsync(long tableId, List<Player> players)
    {
        string updatedPlayersJson = JsonSerializer.Serialize(players, _jsonOptions);

        await _redisDb.HashSetAsync($"bridgeTable:{tableId}",
        [
            new HashEntry($"players", updatedPlayersJson)
        ]);
    }

    public async Task<bool> DeleteBridgeTableAsync(long tableId)
    {
        bool deleted = await _redisDb.KeyDeleteAsync(TableKey(tableId));

        if (deleted) await _redisDb.SetRemoveAsync("bridgeTable:ids", tableId);
        
        return deleted;
    }

    public Task<bool> TableExistsAsync(long tableId) =>
        _redisDb.KeyExistsAsync(TableKey(tableId));
    

    public async Task<List<long>?> GetDealsIdsAsync(long tableId)
    {
        var redisDealsIds = await _redisDb.HashGetAsync(TableKey(tableId), "dealsIds");

        return redisDealsIds.IsNull ? null : JsonSerializer.Deserialize<List<long>>(redisDealsIds!);
    }

    public async Task UpdateListOfDealsIdsAsync(long tableId, List<long> dealsIds)
    {
        string dealsIdsAsJson = JsonSerializer.Serialize(dealsIds);

        await _redisDb.HashSetAsync(TableKey(tableId),
        [
            new HashEntry($"{TableKey(tableId)}:dealsIds", dealsIdsAsJson)
        ]);
    }

    private async Task<List<long>> GetListOfBridgeTablesIdsAsync()
    {
        var tablesIds = await _redisDb.SetMembersAsync("bridgeTable:ids");
        return tablesIds.Select(tableId => (long)tableId).ToList();
    }
}
