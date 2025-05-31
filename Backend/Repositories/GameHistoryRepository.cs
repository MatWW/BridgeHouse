using Backend.Data;
using Backend.Data.Models;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Repositories;

public class GameHistoryRepository : IGameHistoryRepository
{
    private readonly AppDbContext _appDbContext;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public GameHistoryRepository(AppDbContext appDbContext)
    {
       _appDbContext = appDbContext;
    }

    public async Task<GameState?> GetGameByIdAsync(int gameId)
    {
        var game = await _appDbContext.GameHistory.FindAsync(gameId);

        return JsonSerializer.Deserialize<GameState?>(game?.GameStateJson ?? "null", _jsonOptions);
    }

    public async Task<GameShortInfo?> GetGameShortInfoAsync(int gameId)
    {
        var game = await _appDbContext.GameHistory.FindAsync(gameId);

        return JsonSerializer.Deserialize<GameShortInfo?>(game?.GameShortInfoJson ?? "null", _jsonOptions);
    }

    public Task<List<int>> GetUserGamesIdsAsync(string userId) =>
        _appDbContext.UserGames
            .Where(ug => ug.UserId == userId)
            .Select(ug => ug.GameId)
            .ToListAsync();

    public async Task<Position?> GetUserPositionInGameAsync(string userId, int gameId)
    {
        var userGameEntry = await _appDbContext.UserGames.FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GameId == gameId);

        return userGameEntry?.UserPosition;
    }

    public async Task<GameHistory> SaveGameAsync(GameState gameState, GameShortInfo gameShortInfo)
    {
        string gameStateJson = JsonSerializer.Serialize(gameState, _jsonOptions);
        string gameShortInfoJson = JsonSerializer.Serialize(gameShortInfo, _jsonOptions);

        var game = new GameHistory
        {
            GameStateJson = gameStateJson,
            GameShortInfoJson = gameShortInfoJson
        };

        _appDbContext.GameHistory.Add(game);
        await _appDbContext.SaveChangesAsync();

        return game;
    }

    public async Task SaveUserGameAsync(string userId, int gameId, Position userPosition)
    {
        var userGame = new UserGames
        {
            UserId = userId,
            GameId = gameId,
            UserPosition = userPosition
        };

        _appDbContext.UserGames.Add(userGame);
        await _appDbContext.SaveChangesAsync();
    }

    public Task<bool> IsUserPartOfGame(string userId, int gameId) =>
        _appDbContext.UserGames
           .AsNoTracking()
           .AnyAsync(ug => ug.UserId == userId && ug.GameId == gameId);
}
