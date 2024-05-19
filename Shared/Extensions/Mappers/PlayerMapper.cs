using Entities;
using Shared.DataTransferObjects.Resources;

namespace Shared.Extensions.Mappers;

public static class PlayerMapper
{
    public static PlayerResource ToDTO(this Player player)
    {
        return new PlayerResource(player.User.ToDTO(),
                                  player.HoleCards.Select(c => c.ToDTO()).ToList(),
                                  player.CommunityCards.Select(c => c.ToDTO()).ToList(),
                                  player.Cards.Select(c => c.ToDTO()).ToList(),
                                  player.IsSmallBlind,
                                  player.IsBigBlind,
                                  player.Balance,
                                  player.CurrentBet,
                                  player.ValidActions,
                                  player.Turn);
    }

    public static PlayerResource ToDTOWithHoleEmptyCards(this Player player)
    {
        return new PlayerResource(player.User.ToDTO(),
                                  new List<CardResource> { new EmptyCard().ToDTO(), new EmptyCard().ToDTO() },
                                  player.CommunityCards.Select(c => c.ToDTO()).ToList(),
                                  player.Cards.Select(c => c.ToDTO()).ToList(),
                                  player.IsSmallBlind,
                                  player.IsBigBlind,
                                  player.Balance,
                                  player.CurrentBet,
                                  player.ValidActions,
                                  player.Turn);
    }


    public static Player ToEntity(this PlayerResource playerResource)
    {
        return new Player(playerResource.User.ToEntity());
    }
}
