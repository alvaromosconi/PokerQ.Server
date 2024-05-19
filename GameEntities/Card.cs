using System.Runtime.Serialization;

namespace Entities;
public class Card
{
    public enum SuitEnum
    {
        None = 0,
        Spades,
        Hearts,
        Diamonds,
        Clovers
    }

    public enum RankEnum
    {
        None = 0,
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public SuitEnum Suit { get; }
    public RankEnum Rank { get; }

    public Card()
    {
        Random random = new Random();
        int randomSuit = random.Next(Enum.GetValues<SuitEnum>().Length);
        int randomRank = random.Next(Enum.GetValues<RankEnum>().Length);

        Suit = (SuitEnum)randomSuit;
        Rank = (RankEnum)randomRank;
    }
    public Card(int suit, int rank)
    {
        Suit = (SuitEnum)suit;
        Rank = (RankEnum)rank;
    }
    public Card(SuitEnum suit, RankEnum rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public bool Equals(Card other)
        => other is not null &&
           other.Suit == Suit &&
           other.Rank == Rank;

    public override string ToString()
        => $"Suit: {this.Suit}, Rank: {this.Rank}";
}

public sealed class EmptyCard : Card
{
    public EmptyCard() : base(SuitEnum.None, RankEnum.None)
    {
    }
}