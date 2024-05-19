namespace Entities;
public class Deck
{
    private readonly IReadOnlyCollection<Card> _cards
        = Enumerable
            .Range(2, 13)
            .SelectMany(value => GenerateCardsOfValue(value))
            .ToList();

    private static IEnumerable<Card> GenerateCardsOfValue(int value)
        => Enumerable.Range(1, 4)
            .Select(suit => new Card(suit, value));
    
    private Card this[int index]
        => RemainingCards()
            .ToArray()[index];

    private IList<Card> _givenCards = new List<Card>();
    private IEnumerable<Card> RemainingCards()
        => _cards
            .Except(_givenCards);

    public IReadOnlyCollection<Card> Cards() => _cards;

    public int NumberOfRemainingCards() => RemainingCards().Count();
    public Card GetFromRemainingCards(int position)
    {
        Card card = this[position];
        _givenCards.Add(card);

        return card;
    }

    public void ToTheDeck()
        => _givenCards = new List<Card>();
    
}
