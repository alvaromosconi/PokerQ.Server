using Entities;
using Shared.DataTransferObjects.Requests;
using Shared.DataTransferObjects.Resources;
using System.Xml.Linq;

namespace Shared.Extensions.Mappers;

public static class GameSessionMapper
{
    public static GameSessionResource ToDTO(this GameSession gameSession)
    {
        return new GameSessionResource(gameSession.Id, 
                                       gameSession.Code,           
                                       gameSession.Name,
                                       gameSession.ConnectedPlayers,
                                       gameSession.Private);
    }

    public static GameSession ToEntity(this GameSessionResource gameSessionResource)
    {
        return new GameSession(gameSessionResource.Name);
    }

    public static GameSession ToEntity(this CreateGameSessionRequest request)
    {
        return new GameSession(request.Name);
    }
}
