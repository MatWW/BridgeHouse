using Shared;
using Shared.Enums;
using System.Net.Http.Json;

namespace Frontend.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<long?> GetSignedInUserInviteTableIdAsync()
    {
        var inviteTableIdDto = await _httpClient.GetFromJsonAsync<PlayerInviteTableIdResponseDTO>("api/player-state/invite/me")
            ?? throw new InvalidOperationException("Api did not return expected data");

        return inviteTableIdDto.TableId;
    }

    public async Task<long?> GetSignedInUserTableIdAsync()
    {
        var tableIdDto = await _httpClient.GetFromJsonAsync<PlayerTableIdResponseDTO>("api/player-state/table/me")
            ?? throw new InvalidOperationException("Api did not return expected data");

        return tableIdDto.TableId;
    }

    public async Task<long?> GetSignedInUserGameIdAsync()
    {
        var gameIdDto = await _httpClient.GetFromJsonAsync<PlayerGameIdResponseDTO>("api/player-state/game/me")
            ?? throw new InvalidOperationException("Api did not return expected data");

        return gameIdDto.GameId;
    }

    public async Task<GamePhase> GetGamePhaseAsync(long gameId)
    {
        return await _httpClient.GetFromJsonAsync<GamePhase>($"api/game/{gameId}/phase");
    }

    public async Task<PlayingState> GetPlayingStateAsync(long gameId)
    {
        var playingState = await _httpClient.GetFromJsonAsync<PlayingState>($"api/game/{gameId}/playingState")
            ?? throw new InvalidOperationException("Api did not return expected data");

        return playingState;
    }

    public async Task<Player> GetSignedInPlayerInfoAsync(long gameId)
    {
        var signedInPlayer = await _httpClient.GetFromJsonAsync<Player>($"api/game/{gameId}/playerInfo/me")
            ?? throw new InvalidOperationException("Api did not return expected data");

        return signedInPlayer;
    }

    public async Task<Player> GetCurrentTurnPlayerInfoAsync(long gameId)
    {
        var currentTurnPlayer = await _httpClient.GetFromJsonAsync<Player>($"api/game/{gameId}/playerInfo/current")
            ?? throw new InvalidOperationException("Api did not return expected data");

        return currentTurnPlayer;
    }

    public async Task<List<Card>> GetSignedInPlayerCardsAsync(long gameId)
    {
        var cards = await _httpClient.GetFromJsonAsync<List<Card>>($"api/game/{gameId}/cards/me")
            ?? throw new InvalidOperationException("Api did not return expected data");

        return cards;
    }

    public async Task<List<Card>> GetDummiesCardsAsync(long gameId)
    {
        var cards = await _httpClient.GetFromJsonAsync<List<Card>>($"api/game/{gameId}/cards/dummy")
            ?? throw new InvalidOperationException("Api did not return expected data");

        return cards;
    }

    public async Task<Contract> GetContractAsync(long gameId)
    {
        var contract = await _httpClient.GetFromJsonAsync<Contract>($"api/game/{gameId}/contract")
            ?? throw new InvalidOperationException("Api did not return expected data");

        return contract;
    }

    public async Task PlayCardAsync(long gameId, CardPlayAction cardPlayAction)
    {
        await _httpClient.PostAsJsonAsync($"api/game/{gameId}/playCard", cardPlayAction);
    }

    public async Task<BiddingState> GetBiddingStateAsync(long gameId)
    {
        var biddingState = await _httpClient.GetFromJsonAsync<BiddingState>($"api/game/{gameId}/biddingState")
            ?? throw new InvalidOperationException("Api did not return expected data");

        return biddingState;
    }

    public async Task PlaceBidAsync(long gameId, BidAction bidAction)
    {
        await _httpClient.PostAsJsonAsync($"api/game/{gameId}/bid", bidAction);
    }
}
