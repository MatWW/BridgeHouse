using Backend.Data.Models;
using Backend.DTOs;
using Backend.Models;

namespace Backend.Services;

public interface IGameHistoryService
{
    Task<GameState> GetGameByIdAsync(int gameId);
    Task<List<PlayerGameShortInfoDTO>> GetSignedInUserGamesShortInfoAsync();
    Task<GameHistory> SaveGameAsync(GameState gameState);
}
