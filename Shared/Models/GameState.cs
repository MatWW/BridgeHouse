using Shared.Enums;

namespace Shared.Models;

public class GameState
{
    public long? Id { get; set; }
    public long TableId { get; set; }
    public List<Player> Players { get; set; } = new();
    public GamePhase GamePhase { get; set; }
    public Dictionary<string, List<Card>> PlayerHands { get; set; } = new();
    public string CurrentPlayerId { get; set; } = string.Empty;
    public BiddingState BiddingState { get; set; } = new();
    public PlayingState PlayingState { get; set; } = new();
}
