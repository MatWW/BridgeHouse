using Backend.Data.Models;
using Backend.Enums;
using Backend.Models;

namespace Backend.Repositories;

public interface IGameHistoryRepository
{
    Task<GameState?> GetGameByIdAsync(int gameId);
    Task<GameShortInfo?> GetGameShortInfoAsync(int gameId);
    
    Task<List<int>> GetUserGamesIdsAsync(string userId);
    Task<Position?> GetUserPositionInGameAsync(string userId, int gameId);

    Task<GameHistory> SaveGameAsync(GameState gameState, GameShortInfo gameShortInfo);
    Task SaveUserGameAsync(string userId, int gameId, Position userPosition);

    Task<bool> IsUserPartOfGame(string userId, int gameId);
}
