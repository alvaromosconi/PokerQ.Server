using Entities;
using Shared.DataTransferObjects.Resources;

namespace Shared.Extensions.Mappers;
public static class CardMapper
{
    public static CardResource ToDTO(this Card card)
    {
        return new CardResource(card.Suit.ToString(), card.Rank.ToString());
    }

    public static Card ToEntity(this CardResource cardResource)
    {
        if (!Enum.TryParse(cardResource.Suit, out Card.SuitEnum suitEnum))
        {
            throw new ArgumentException($"Invalid suit: {cardResource.Suit}");
        }

        if (!Enum.TryParse(cardResource.Rank, out Card.RankEnum rankEnum))
        {
            throw new ArgumentException($"Invalid rank: {cardResource.Rank}");
        }

        return new Card(suitEnum, rankEnum);
    }

}
