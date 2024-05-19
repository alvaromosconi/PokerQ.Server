using Entities;
using Shared.DataTransferObjects.Requests;
using Shared.DataTransferObjects.Resources;

namespace Services.Contracts;

public interface IGameSessionService
{
    Result<GameSession> CreateGameSession(CreateGameSessionRequest request, User user);
    Result<bool> DeleteGameSession(string id);
    Result<GameSession> GetSessionById(string id);
    Result<GameSession> GetSessionByCode(string code);
    IList<GameSessionResource> GetSessions();
    bool IsPlayerInTable(User user);
    Result<GameSession> JoinGameSession(JoinGameSessionRequest request, User user);
    Result<GameSession> GetUserSession(string userId);
    void StartGame(GameSession session);
    Result<GameSession> LeaveGameSession(User user);
}