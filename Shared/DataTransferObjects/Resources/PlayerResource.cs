using Entities;

namespace Shared.DataTransferObjects.Resources;

public record PlayerResource(UserResource User,
                             IReadOnlyCollection<CardResource>? HoleCards,
                             IReadOnlyCollection<CardResource>? CommunityCards,
                             IEnumerable<CardResource>? Cards,
                             bool IsSmallBlind,
                             bool isBigBling,
                             int Balance,
                             int CurrentBet,
                             List<PlayerAction> ValidActions,
                             bool Turn);