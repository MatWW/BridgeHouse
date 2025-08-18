using Backend.Enums;

namespace Backend.Models;

public class Card
{
    public CardValue Value { get; set; }
    public CardSuit Suit { get; set; }
}
