using Entities;
using Shared.DataTransferObjects.Resources;

namespace Server.Hubs.ClientInterfaces;

public interface IGameHub
{
    Task GameSessionCreated(GameSessionResource session);
    Task GameSessionUpdated(GameSessionResource session);
    Task UserJoinedGameSession(string playerName);
    Task SetSessions(IList<GameSessionResource> sessions);
    Task RedirectToGame();
    Task WaitForPlayers();
    Task RedirectToLobby();
    Task DealHoleCards(TableResource table);
    Task PlayerLeft(TableResource table);
    Task RoundStarted(GameSession session);
    Task WinnerDeclared(string username);
    Task CommunityCardsDealt(IList<CardResource> communityCards);
    Task HoleCardsDealt(List<Player> players);

    Task PlayerCalled(TableResource table);
    Task PlayerFold(TableResource table);
    Task TableUpdated(TableResource tableResource);
}
