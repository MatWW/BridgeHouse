namespace Shared.Enums.Extensions;

public static class BiddingExtensions
{
    public static string ToSymbol(this BiddingSuit suit) => suit switch
    {
        BiddingSuit.CLUB => "♣",
        BiddingSuit.DIAMOND => "♦",
        BiddingSuit.HEART => "♥",
        BiddingSuit.SPADE => "♠",
        BiddingSuit.NO_TRUMP => "NT",
        BiddingSuit.NONE => "",
        _ => "?"
    };

    public static string ToSymbol(this BiddingValue value) => value switch
    {
        BiddingValue.ONE => "1",
        BiddingValue.TWO => "2",
        BiddingValue.THREE => "3",
        BiddingValue.FOUR => "4",
        BiddingValue.FIVE => "5",
        BiddingValue.SIX => "6",
        BiddingValue.SEVEN => "7",
        BiddingValue.PASS => "PASS",
        BiddingValue.DOUBLE => "x",
        BiddingValue.REDOUBLE => "xx",
        _ => "?"
        
    };

    public static CardSuit ToCardSuit(this BiddingSuit suit) => suit switch
    {
        BiddingSuit.CLUB => CardSuit.CLUB,
        BiddingSuit.DIAMOND => CardSuit.DIAMOND,
        BiddingSuit.HEART => CardSuit.HEART,
        BiddingSuit.SPADE => CardSuit.SPADE,
        _ => default
    };

    public static string ToLowercaseString(this BiddingSuit suit) => suit switch
    {
        BiddingSuit.CLUB => "club",
        BiddingSuit.DIAMOND => "diamond",
        BiddingSuit.HEART => "heart",
        BiddingSuit.SPADE => "spade",
        BiddingSuit.NO_TRUMP => "no-trump",
        BiddingSuit.NONE => "",
        _ => "?"
    };
}
