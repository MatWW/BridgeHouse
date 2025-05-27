namespace Shared.Enums.Extensions;

public static class CardExtensions
{

    public static string ToName(this CardSuit suit) => suit switch
    {
        CardSuit.CLUB => "club",
        CardSuit.DIAMOND => "diamond",
        CardSuit.HEART => "heart",
        CardSuit.SPADE => "spade",
        _ => "?"
    };

    public static string ToSymbol(this CardSuit suit) => suit switch
    {
        CardSuit.CLUB => "♣",
        CardSuit.DIAMOND => "♦",
        CardSuit.HEART => "♥",
        CardSuit.SPADE => "♠",
        _ => "?"
    };

    public static string ToSymbol(this CardValue value) => value switch
    {
        CardValue.TWO => "2",
        CardValue.THREE => "3",
        CardValue.FOUR => "4",
        CardValue.FIVE => "5",
        CardValue.SIX => "6",
        CardValue.SEVEN => "7",
        CardValue.EIGHT => "8",
        CardValue.NINE => "9",
        CardValue.TEN => "10",
        CardValue.JACK => "J",
        CardValue.QUEEN => "Q",
        CardValue.KING => "K",
        CardValue.ACE => "A",
        _ => "?"
    };
}
