using Backend.Data.Models;
using Backend.Repositories;
using Shared.Enums;
using Backend.Exceptions;
using Shared.DTOs;
using Shared.Models;

namespace Backend.Services;

public class GameHistoryService : IGameHistoryService
{
    private readonly IGameHistoryRepository _gameHistoryRepository;
    private readonly IUserService _userService;

    public GameHistoryService(IGameHistoryRepository gameHistoryRepository, IUserService userService)
    {
        _gameHistoryRepository = gameHistoryRepository;
        _userService = userService;
    }

    public async Task<GameState> GetGameByIdAsync(int gameId)
    {
        var game = await _gameHistoryRepository.GetGameByIdAsync(gameId);

        if (game is null) throw new GameNotFoundException($"Game (from game history) with id: {gameId} was not found");

        return await _gameHistoryRepository.IsUserPartOfGame(_userService.GetCurrentUserId(), gameId) ?
            game : throw new UnauthorizedAccessException($"Player was not part of game with id: {gameId}");
    }

    public async Task<List<PlayerGameShortInfoDTO>> GetSignedInUserGamesShortInfoAsync()
    {
        var userId = _userService.GetCurrentUserId();

        var gamesIds = await _gameHistoryRepository.GetUserGamesIdsAsync(userId);

        List<PlayerGameShortInfoDTO> shortInfos = [];

        foreach (var gameId in gamesIds)
        {
            var gameShortInfo = await _gameHistoryRepository.GetGameShortInfoAsync(gameId);
            var userPosition = await _gameHistoryRepository.GetUserPositionInGameAsync(userId, gameId);

            if (gameShortInfo is not null && userPosition.HasValue)
                shortInfos.Add(new PlayerGameShortInfoDTO { GameShortInfo = gameShortInfo, UserPosition = userPosition.Value });
        }

        return shortInfos;
    }

    public async Task<GameHistory> SaveGameAsync(GameState gameState)
    {
        var game = await _gameHistoryRepository.SaveGameAsync(gameState, ConvertGameStateToGameShortInfo(gameState));

        List<Player> players = gameState.Players;

        foreach(var player in players)
        {
            await SaveUserGameAsync(player.PlayerId, game.Id, gameState);
        }

        return game;
    }

    private async Task SaveUserGameAsync(string userId, int gameId, GameState gameState)
    {
        Position userPosition = gameState.Players
            .Where(p => p.PlayerId == userId)
            .Select(p  => p.Position)
            .FirstOrDefault();

        await _gameHistoryRepository.SaveUserGameAsync(userId, gameId, userPosition);
    }

    private GameShortInfo ConvertGameStateToGameShortInfo(GameState gameState)
    {
        var finalContract = gameState.BiddingState.Contract?.BidAction.Bid;
        var declarerPosition = gameState.PlayingState.Declarer?.Position;
        var NSTricks = gameState.PlayingState.NSTricks;
        var EWTricks = gameState.PlayingState.EWTricks;


        GameShortInfo gameShortInfo = new GameShortInfo
        {
            FinalContract = finalContract,
            IsDoubled = gameState.BiddingState.Contract?.IsDoubled ?? false,
            IsRedoubled = gameState.BiddingState.Contract?.IsRedoubled ?? false,
            TrickBalance = finalContract is not null ? GetTrickBalance(finalContract.Value, declarerPosition!.Value, NSTricks, EWTricks) : 0,
            DeclarerPosition = declarerPosition
        };

        return gameShortInfo;
    }

    private static int GetTrickBalance(BiddingValue contractValue, Position declarerPosition, int NSTricks, int EWTricks)
    {
        int tricksToTake = (int)contractValue + 6;

        return declarerPosition == Position.N || declarerPosition == Position.S ? NSTricks : EWTricks - tricksToTake;
    }

}
