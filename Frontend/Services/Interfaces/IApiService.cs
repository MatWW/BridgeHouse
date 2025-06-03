using Shared;
using Shared.Enums;

namespace Frontend.Services.Interfaces;

public interface IApiService
{
    Task<long?> GetSignedInUserInviteTableIdAsync();
    Task<long?> GetSignedInUserTableIdAsync();
    Task<long?> GetSignedInUserGameIdAsync();
    Task<GamePhase> GetGamePhaseAsync(long gameId);
    Task<PlayingState> GetPlayingStateAsync(long gameId);
    Task<BridgeTable> GetBridgeTableAsync(long tableId);
    Task CreateBridgeTableAsync();
    Task<string> GetIdByNickname(string nickname);
    Task SendInviteAsync(long tableId, string userId, Position position);
    Task AcceptInviteAsync();
    Task DeclineInviteAsync();
    Task LeaveTableAsync(long tableId);
    Task DeleteTableAsync(long tableId);
    Task<string> GetSignedInUserIdAsync();
    Task StartGameAsync(long tableId, List<Player> players);
    Task<Player> GetSignedInPlayerInfoAsync(long gameId);
    Task<Player> GetCurrentTurnPlayerInfoAsync(long gameId);
    Task<List<Card>> GetSignedInPlayerCardsAsync(long gameId);
    Task<List<Card>> GetDummiesCardsAsync(long gameId);
    Task<Contract> GetContractAsync(long gameId);
    Task PlayCardAsync(long gameId, CardPlayAction cardPlayAction0);
    Task<BiddingState> GetBiddingStateAsync(long gameId);
    Task PlaceBidAsync(long gameId, BidAction bidAction);
    Task<List<PlayerGameShortInfoDTO>> GetSignedInUserGameHistoryAsync();
}
