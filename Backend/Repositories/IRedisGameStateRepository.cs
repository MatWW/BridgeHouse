using Shared;

namespace Backend.Repositories;

public interface IRedisGameStateRepository
{
    Task<GameState?> GetGameStateAsync(long gameId);
    Task<long> SaveGameStateAsync(GameState gameState);
    Task DeleteGameStateAsync(long gameId);
}
