using Shared;

namespace Backend.Repositories;

public interface IRedisGameStateRepository
{
    Task<GameState?> GetGameStateAsync(long gameId);
    Task SetGameStateAsync(GameState gameState);
}
