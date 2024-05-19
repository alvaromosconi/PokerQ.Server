using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs.ClientInterfaces;
using Services.Contracts;
using Shared.DataTransferObjects.Requests;
using Shared.DataTransferObjects.Resources;
using Shared.Extensions.Mappers;
namespace Server.Hubs;

[Authorize]
public class GameHub : Hub<IGameHub>
{
    private readonly IGameSessionService _sessionService;
    private readonly IAuthenticationService _authenticationService;
    public GameHub(IGameSessionService gameSessionService, 
                   IAuthenticationService authenticationService)
    {
        _sessionService = gameSessionService;
        _authenticationService = authenticationService;
    }

    public override async Task OnConnectedAsync()
    {
        GameSession? session = GetUserSession();

        if (session is GameSession)
        {
            if (session.Started && session.ConnectedPlayers > 1)
            {
                await Clients.Caller.RedirectToGame();
            }
            await Clients.Caller.TableUpdated(session.Table.ToDTO());
        }
        else
        {
            await SendSessions();
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task CreateGameSession(CreateGameSessionRequest request)
    {
        User user = await GetUserAsync();
        
        Result<GameSession> result = _sessionService.CreateGameSession(request, user);

        if (result.IsSuccess)
        {
            GameSession session = result.Value;
            await Groups.AddToGroupAsync(Context.ConnectionId, session.Id);
            await Clients.Caller.GameSessionCreated(session.ToDTO());
            await Clients.All.SetSessions(_sessionService.GetSessions().ToList());
        }
    }

    public async Task JoinGameSession(JoinGameSessionRequest request)
    {
        User user = await GetUserAsync();

        Result<GameSession> result = _sessionService.JoinGameSession(request, user);

        if (result.IsSuccess)
        {
            GameSession session = result.Value;

            if (session.Started == false)
            {
                session.StartGame();

            }

            await Groups.AddToGroupAsync(Context.ConnectionId, session.Id);
            await Clients.All.GameSessionUpdated(session.ToDTO());
            await Clients.Group(session.Id).RedirectToGame();
        }
    }

    public async Task LeaveGameSession()
    {
        User user = await GetUserAsync();

        Result<GameSession> result = _sessionService.LeaveGameSession(user);

        if (result.IsSuccess)
        {
            GameSession session = result.Value;
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, session.Id);

            if (session.ConnectedPlayers == 0)
            {
                _sessionService.DeleteGameSession(session.Id);
            }
            else
            {
                await Clients.All.GameSessionUpdated(session.ToDTO());
                await Clients.Group(session.Id).TableUpdated(session.Table.ToDTO());
            }
        }
    }

    public async Task SendSessions()
    {
        List<GameSessionResource> sessions = _sessionService.GetSessions()
                                                            .ToList();

        await Clients.Caller.SetSessions(sessions);
    }

    public async Task GetGame()
    {
        GameSession session = GetUserSession();
        
        if (session is GameSession)
        {            
            await Clients.Group(session.Id)
                         .TableUpdated(session.Table.ToDTO());
        }
        else
        {
            await Clients.Caller.RedirectToLobby();
        }
    }

    public async Task PlayerAction(PlayerAction action, int? amount)
    {
        GameSession session = GetUserSession();

        if (session is GameSession)
        {
            session.HandlePlayerAction(action, amount);

            await Clients.Group(session.Id)
                         .TableUpdated(session.Table.ToDTO());

            await HandleWinner(session);
        }
    }

    private async Task HandleWinner(GameSession session)
    {
        Player? winner = session.GetWinner();
        if (winner is not null)
        {
            await Clients.Group(session.Id)
                         .WinnerDeclared(winner.User.UserName);

            await Task.Delay(TimeSpan.FromSeconds(3));

            session.RemovePlayersWithoutFunds();
            session.ResetTable();
            session.StartGame();

            await Clients.Group(session.Id)
                         .TableUpdated(session.Table.ToDTO());

        }
    }

    private async Task<User> GetUserAsync()
    {
        return await _authenticationService.GetUserByIdAsync(Context.UserIdentifier);
    }

    private GameSession? GetUserSession()
    {
        string userIdentifier = Context.UserIdentifier;
        Result<GameSession> result = _sessionService.GetUserSession(userIdentifier);

        if (result.IsSuccess)
        {
            return result.Value;
        }

        return null;
    }


}
