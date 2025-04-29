using StackExchange.Redis;
using Shared;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        var players = hashEntries.First(x => x.Name == "Players");

        if (id.Equals(default) || adminId.Equals(default) || numberOfDeals.Equals(default) || players.Equals(default))
        {
            await redisDb.SetRemoveAsync("bridgeTable:ids", bridgeTableId);
            return null;
        }

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        return new BridgeTable
        {
            Id = long.Parse(id.Value!),
            AdminId = adminId.Value!,
            NumberOfDeals = int.Parse(numberOfDeals.Value!),
            Players = JsonSerializer.Deserialize<List<Player>>(players.Value!, options)!
        };
    }

    public async Task<List<Player>?> GetListOfBridgeTablePalyersAsync(long bridgeTableId)
    {
        var redisPlayers = await redisDb.HashGetAsync($"bridgeTable:{bridgeTableId}", "players");

        if (redisPlayers.IsNull)
        {
            return null;
        }

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        //if redisPlayersIds is empty JsonSerializer will return null
        var players = JsonSerializer.Deserialize<List<Player>>(redisPlayers!, options);

        return players ?? [];
    }

    public async Task<string?> GetTableAdminIdAsync(long bridgeTableId)
    {
        var redisAdminId = await redisDb.HashGetAsync($"bridgeTable:{bridgeTableId}", "adminId");

        if (redisAdminId.IsNull)
        {
            return null;
        }

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        string? adminId = JsonSerializer.Deserialize<string>(redisAdminId!, options);

        return adminId ?? string.Empty;
    }

    public async Task<BridgeTable> SaveBridgeTableAsync(BridgeTable table)
    { 
        table.Id ??= await redisDb.StringIncrementAsync("bridgeTable:id");

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };
        
        string playersAsJson = JsonSerializer.Serialize(table.Players, options);

        HashEntry[] hashEntry =
        [
            new ("adminId", table.AdminId),
            new ("players", playersAsJson),
            new ("numberOfDeals", table.NumberOfDeals),
        ];
        await redisDb.HashSetAsync($"table:{table.Id}", hashEntry);

        await redisDb.SetAddAsync("bridgeTable:ids", table.Id);

        return table;
    }

    public async Task UpdateListOfBridgeTablePlayersAsync(long tableId, List<Player> players)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        string updatedPlayersJson = JsonSerializer.Serialize(players, options);

        await redisDb.HashSetAsync($"bridgeTable:{tableId}",
        [
            new HashEntry($"bridgeTable:{tableId}:players", updatedPlayersJson)
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

    private async Task<List<long>> GetListOfBridgeTablesIdsAsync()
    {
        var tablesIds = await redisDb.SetMembersAsync("bridgeTable:ids");
        return tablesIds.Select(tableId => (long)tableId).ToList();
    }
}
