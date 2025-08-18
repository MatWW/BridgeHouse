using Backend.Models;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Repositories;

public class RedisGameStateRepository : IRedisGameStateRepository
{
    private readonly IDatabase _redisDb;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private static string GameKey(long gameId) => $"game:{gameId}";

    public RedisGameStateRepository(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task<GameState?> GetGameStateAsync(long gameId)
    {
        var gameStateAsString = await _redisDb.StringGetAsync(GameKey(gameId));

        if (!gameStateAsString.HasValue)
            return null;

        return JsonSerializer.Deserialize<GameState>(gameStateAsString!, _jsonOptions);
    }

    public async Task<long> SaveGameStateAsync(GameState gameState)
    {
        gameState.Id ??= await _redisDb.StringIncrementAsync("game:id");

        var serialized = JsonSerializer.Serialize(gameState, _jsonOptions);

        await _redisDb.StringSetAsync(GameKey(gameState.Id.Value), serialized);

        return (long)gameState.Id;
    }

    public Task DeleteGameStateAsync(long gameId) =>
        _redisDb.KeyDeleteAsync(GameKey(gameId));
}
