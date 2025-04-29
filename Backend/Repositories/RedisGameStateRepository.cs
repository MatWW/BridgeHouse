using Shared;
using StackExchange.Redis;
using System.Text.Json;

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

        GameState? gameState = JsonSerializer.Deserialize<GameState>(GameStateAsString);

        return gameState ?? null;
    }

    public async Task SetGameStateAsync(GameState gameState)
    {
        string gameStateAsString = JsonSerializer.Serialize(gameState);
        long gameId = gameState.Id;

        await redisDb.StringSetAsync($"game:{gameId}", gameStateAsString);
    }

}
