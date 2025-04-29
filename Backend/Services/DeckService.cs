using Shared;
using Shared.Enums;

namespace Backend.Services;

public class DeckService : IDeckService
{
    private List<Card> deck;
    private Random rnd;

    public DeckService()
    {
        deck = new List<Card>();
        rnd = new Random();
        GenerateDeck();
        Shuffle();
    }

    public void GenerateDeck()
    {
        deck.Clear();
        foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
        {
            foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
            {
                deck.Add(new Card {  Suit = suit, Value = value });
            }
        }
    }

    public void Shuffle()
    {
        for (int i = deck.Count-1; i > 0; i--)
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

        return playersHands;
    }

}
