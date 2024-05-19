using Entities;
using Shared.DataTransferObjects.Resources;

namespace Shared.Extensions.Mappers;
public static class TableMapper
{
    public static TableResource ToDTO(this Table table)
    {
        return new TableResource(table.Players.Select(player => player.ToDTO()).ToList(),
                                 table.SmallBlind,
                                 table.BigBlind,
                                 table.Pot,
                                 table.CurrentBet,
                                 table.CurrentPlayer.ToDTO(),
                                 table.CommunityCards.Select(card => card.ToDTO()).ToList(),
                                 table.Stage,
                                 table.Winner);
    }

    public static TableResource ToDTOWithoutRevealingPlayerHoleCards(this Table table, string currentUserId)
    {
        return new TableResource(table.Players.Select(player => player.User.Id == currentUserId ? player.ToDTO() : player.ToDTOWithHoleEmptyCards()).ToList(),
                                 table.SmallBlind,
                                 table.BigBlind,
                                 table.Pot,
                                 table.CurrentBet,
                                 table.CurrentPlayer.ToDTO(),
                                 table.CommunityCards.Select(card => card.ToDTO()).ToList(),
                                 table.Stage,
                                 table.Winner);
    }
}
