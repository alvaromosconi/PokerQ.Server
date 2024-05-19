using Entities;
using Services.Contracts;
using Shared.DataTransferObjects.Requests;
using Shared.DataTransferObjects.Resources;
using Shared.Extensions.Mappers;
using System.Collections.Concurrent;

namespace Services;

public class GameSessionService : IGameSessionService
{
    private static readonly ConcurrentDictionary<string, GameSession> Sessions = new ConcurrentDictionary<string, GameSession>();
    private static readonly List<User> RemovedUsers = new List<User>();

    public Result<GameSession> CreateGameSession(CreateGameSessionRequest request, 
                                                 User user)
    {
        if (IsPlayerInTable(user))
            return Result<GameSession>.Failure("Player already is in a table.");

        GameSession session = request.ToEntity()
                                     .AddPlayer(user);

        session.Code = GenerateGameCode();

        var succeeded = Sessions.TryAdd(session.Id, session);

        if (succeeded == false)
            return Result<GameSession>.Failure("Failed to create game session.");

        return Result<GameSession>.Success(session);
    }

    public Result<bool> DeleteGameSession(string id)
    {
        if (!Sessions.TryRemove(id, out GameSession session))
            return Result<bool>.Failure("The table does not exist.");

        return Result<bool>.Success(true);
    }

    public Result<GameSession> JoinGameSession(JoinGameSessionRequest request, 
                                               User user)
    {
        if (IsPlayerInTable(user))
            return Result<GameSession>.Failure("Player already is in a table.");

        Result<GameSession> result = GetSessionByCode(request.Code);

        if (!result.IsSuccess)
            return Result<GameSession>.Failure("The table does not exist.");

        GameSession session = result.Value;
        session.AddPlayer(user);

        return Result<GameSession>.Success(session);
    }

    public bool IsPlayerInTable(User user)
        => Sessions.Values.Any(s => s.IsPlayerInTable(user.Id));

    public IList<GameSessionResource> GetSessions()
        => Sessions.Values
                        .Select(s => s.ToDTO())
                        .ToList();


    public Result<GameSession> LeaveGameSession(User user)
    {
        if (!IsPlayerInTable(user))
        {
            return Result<GameSession>.Failure("Player is not in a table.");
        }
           
        GameSession session = Sessions.Values.First(s => s.IsPlayerInTable(user.Id));
        session.RemovePlayer(user.Id);

        if (session.ConnectedPlayers <= 1)
        {
            session.Started = false;
        }

        return Result<GameSession>.Success(session);
    }


    public Result<GameSession> GetSessionById(string id)
    {
        Sessions.TryGetValue(id, out GameSession session);
            
        return session != null 
                ? Result<GameSession>.Success(session) 
                : Result<GameSession>.Failure("The table does not exist.");
    }

    public Result<GameSession> GetSessionByCode(string code)
    {
        GameSession session = Sessions.Values.First(s => s.Code == code);

        return session == null
                ? Result<GameSession>.Failure("The table does not exist.")
                : Result<GameSession>.Success(session);
    }

    public Result<GameSession> GetUserSession(string userId)
    {
        GameSession session = Sessions.Values.FirstOrDefault(s => s.Table.IsPlayerInTable(userId));

        return session == null
                ? Result<GameSession>.Failure("The table does not exist.")
                : Result<GameSession>.Success(session);
    }

    public void StartGame(GameSession session)
    {
        session.StartGame();        
    }

    private string GenerateGameCode()
    {
        string code = string.Empty;
        do
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            code = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());

        } while (Sessions.ContainsKey(code));

        return code;
    }
}

