namespace Entities;
public class Dealer
{
    private Deck _deck;

    public Dealer (Deck deck)
    {
        _deck = deck;
    }

    public Card DealCard()
    {
        Random random = new Random();

        return _deck.GetFromRemainingCards(random.Next(_deck.NumberOfRemainingCards()));
    }

    public IReadOnlyCollection<Card> DealCards(int numberOfCards)
    {
        List<Card> cards = new List<Card>();
       
        for (int card = 0; card < numberOfCards; card++)
        {
            cards.Add(DealCard());
        }

        return cards;
    }
}
