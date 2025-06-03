using Shared.Enums;
using Shared.Models;

namespace Backend.Services;

public class DeckService : IDeckService
{
    private readonly List<Card> deck;
    private readonly Random rnd;

    public DeckService()
    {
        deck = new List<Card>();
        rnd = new Random();
        GenerateDeck();
        Shuffle();
    }

    private void GenerateDeck()
    {
        deck.Clear();
        foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
        {
            foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
            {
                deck.Add(new Card { Suit = suit, Value = value });
            }
        }
    }

    private void Shuffle()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            (deck[i], deck[j]) = (deck[j], deck[i]);
        }
    }

    public Dictionary<string, List<Card>> DealCards(List<string> playersIds)
    {
        if (playersIds.Count != 4)
            throw new ArgumentException("There need to be exactly four players");

        var playersHands = playersIds.ToDictionary(id => id, id => new List<Card>());

        int playerInd = 0;
        foreach (Card card in deck)
        {
            var playerId = playersIds[playerInd];
            playersHands[playerId].Add(card);
            playerInd = (playerInd + 1) % 4;
        }

        foreach(var playerId in playersIds)
        {
            playersHands[playerId] = SortHand(playersHands[playerId]);
        }

        return playersHands;
    }

    private List<Card> SortHand(List<Card> handCards)
    {
        return handCards
        .OrderByDescending(card => GetSuitPriority(card.Suit))
        .ThenByDescending(card => card.Value)
        .ToList();
    }

    private int GetSuitPriority(CardSuit suit) => suit switch
    {
        CardSuit.SPADE => 4,
        CardSuit.HEART => 3,
        CardSuit.CLUB => 2,
        CardSuit.DIAMOND => 1,
        _ => 0
    };
}
