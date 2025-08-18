using Backend.Enums;
using StackExchange.Redis;

namespace Backend.Repositories;

public class RedisPlayerStateRepository : IRedisPlayerStateRepository
{
    private readonly IDatabase _redisDb;

    public RedisPlayerStateRepository(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    private static string InviteKey(string playerId) => $"player:{playerId}:invite";
    private static string TableKey(string playerId) => $"player:{playerId}:table";
    private static string GameKey(string playerId) => $"player:{playerId}:game";

    public async Task<long?> GetTableIdOfPlayerInviteAsync(string playerId)
    {
        var value = await GetHashValueAsync(InviteKey(playerId), "tableId");
        return long.TryParse(value, out var tableId) ? tableId : null;
    }

    public async Task<Position?> GetPositionOfPlayerInviteAsync(string playerId)
    {
        var value = await GetHashValueAsync(InviteKey(playerId), "position");
        return Enum.TryParse(value, out Position position) ? position : null;
    }

    public Task<long?> GetTableIdOfPlayerAsync(string playerId) =>
        GetLongValueFromKeyAsync(TableKey(playerId));

    public Task<long?> GetGameIdOfPlayerAsync(string playerId) =>
        GetLongValueFromKeyAsync(GameKey(playerId));

    public Task SaveInformationAboutPlayerBeingInvitedToTableAsync(string playerId, long tableId, Position position)
    {
        var entries = new[]
        {
            new HashEntry("tableId", tableId),
            new HashEntry("position", position.ToString())
        };

        return _redisDb.HashSetAsync(InviteKey(playerId), entries);
    }

    public Task SaveInformationAboutPlayerBeingPartOfTableAsync(string playerId, long tableId) =>
        _redisDb.StringSetAsync(TableKey(playerId), tableId);
  

    public Task SaveInformationAboutPlayerBeingInGameAsync(string playerId, long gameId) =>
        _redisDb.StringSetAsync(GameKey(playerId), gameId);

    public Task DeleteInformationAboutPlayerBeingInvitedToTableAsync(string playerId) =>
        _redisDb.KeyDeleteAsync(InviteKey(playerId));

    public async Task DeleteInformationAboutPlayerBeingPartOfTableAsync(string playerId) =>
        await _redisDb.KeyDeleteAsync(TableKey(playerId));

    public Task DeleteInformationAboutPlayerBeingInGameAsync(string playerId) =>
       _redisDb.KeyDeleteAsync(GameKey(playerId));
  
    private async Task<string?> GetHashValueAsync(string key, string field)
    {
        var value = await _redisDb.HashGetAsync(key, field);
        return value.HasValue ? value.ToString() : null;
    }

    private async Task<long?> GetLongValueFromKeyAsync(string key)
    {
        var value = await _redisDb.StringGetAsync(key);
        return long.TryParse(value, out var result) ? result : null;
    }
}
