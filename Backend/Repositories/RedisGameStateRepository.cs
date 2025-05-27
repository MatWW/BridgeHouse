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

    public async Task<long> SaveGameStateAsync(GameState gameState)
    {
        gameState.Id ??= await redisDb.StringIncrementAsync("game:id");

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        string gameStateAsString = JsonSerializer.Serialize(gameState, options);

        await redisDb.StringSetAsync($"game:{gameState.Id}", gameStateAsString);

        long id = (long)gameState.Id;

        return id;
    }

    public async Task SaveInformationAboutPlayerBeingInGameAsync(string playerId, long gameId)
    {
        await redisDb.StringSetAsync($"player:{playerId}:game", gameId);
    }

    public async Task<long?> GetGameIdOfPlayerAsync(string playerId)
    {
        var gameIdAsString = await redisDb.StringGetAsync($"player:{playerId}:game");

        if (!gameIdAsString.HasValue)
            return null; 

        if (long.TryParse(gameIdAsString, out long gameId))
            return gameId; 

        return null; 
    }
}
