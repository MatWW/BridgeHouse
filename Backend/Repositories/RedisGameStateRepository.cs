using Shared;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Repositories;

public class RedisGameStateRepository : IRedisGameStateRepository
{
    private readonly IDatabase redisDb;

    public RedisGameStateRepository(IConnectionMultiplexer redis)
    {
        redisDb = redis.GetDatabase();
    }

    public async Task<GameState?> GetGameStateAsync(long gameId)
    {
        string? GameStateAsString = await redisDb.StringGetAsync($"game:{gameId}");

        if (GameStateAsString is null)
        {
            return null;
        }

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        GameState? gameState = JsonSerializer.Deserialize<GameState>(GameStateAsString, options);

        return gameState ?? null;
    }

    public async Task SaveGameStateAsync(GameState gameState)
    {
        gameState.Id ??= await redisDb.StringIncrementAsync("game:id");

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        string gameStateAsString = JsonSerializer.Serialize(gameState, options);

        await redisDb.StringSetAsync($"game:{gameState.Id}", gameStateAsString);
    }

}
