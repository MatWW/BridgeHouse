using Shared;
using Shared.Enums;

namespace Backend.Services;

public interface IGameService
{
    Task StartGameAsync(long tableId, List<Player> players);
    Task<GameState> GetGameStateAsync(long gameId); 
    Task<BiddingState> GetBiddingStateAsync(long gameId);
    Task<PlayingState> GetPlayingStateAsync(long gameId); 
    Task<Contract?> GetContractAsync(long gameId);
    Task<List<Card>> GetPlayerCardsAsync(long gameId, string playerId);
    Task<List<Card>> GetDummiesCardsAsync(long gameId);
    Task<Player> GetInfoAboutPlayerInGameAsync(string playerId, long gameId);
    Task<Player> GetCurrentPlayerInfoAsync(long gameId);
    Task<Player> GetSignedInPlayerInfoAsync(long gameId);
    Task<GamePhase> GetGamePhaseAsync(long gameId);
    Task EndGameAsync(long gameId);
}
