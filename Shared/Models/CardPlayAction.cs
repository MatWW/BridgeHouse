namespace Shared.Models;

public class CardPlayAction
{
    public Player Player { get; set; } = new();
    public Card CardPlayed { get; set; } = new();
}
