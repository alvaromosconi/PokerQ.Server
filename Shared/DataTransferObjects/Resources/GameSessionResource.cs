namespace Shared.DataTransferObjects.Resources;
public record GameSessionResource(string Id, 
                                  string Code,
                                  string Name,
                                  int ConnectedPlayers,
                                  bool Private);