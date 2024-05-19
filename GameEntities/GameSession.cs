namespace Entities;

public class GameSession(string name)
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Code { get; set; }
    public Table Table { get; set; } = new Table();
    public bool Private { get; set; }
    public bool Started { get; set; } = false;
    public int ConnectedPlayers => Table.PlayersCount;

    public GameSession AddPlayer(User user)
    {
        Table.AddPlayer(user);
      
        return this;
    }   

    public void RemovePlayer(string userId)
    {
        Table.RemovePlayer(userId);
    }

    public void RemovePlayersWithoutFunds()
    {
        Table.RemovePlayersWithoutFunds();
    }

    public void StartGame()
    {
        if (Started == false)
        {
            Started = true;
            NextStage();
        }
 
    }

    public void NextStage()
    {
        Table.NextStage();
    }

    public Player? GetWinner()
    {
        return Table.Winner;
    }

    public void ResetTable()
    {
        Table.ResetTable();
    }

    public void HandlePlayerAction(PlayerAction action, int? amount)
    {
        Table.HandlePlayerAction(action, amount);
    }

    public bool IsBettingRoundComplete()
    {
        return Table.IsBettingRoundComplete();
    }

    public bool IsPlayerInTable(string userId)
    {
        return Table.IsPlayerInTable(userId);
    }
}

