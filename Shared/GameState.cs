using Shared.Enums;

namespace Shared;

public class GameState
{
    public long Id { get; set; }
    public List<PlayerInfo> Players { get; set; } = new();
    public GamePhase GamePhase { get; set; }
    public Dictionary<string, List<Card>> PlayerHands { get; set; } = new();
    public BiddingState BiddingState { get; set; } = new();
    public PlayingState PlayingState { get; set; } = new();
}
