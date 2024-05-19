using Entities;

public class PlayerManager
{
    public List<Player> Players { get; private set; } = new List<Player>();
    public List<Player> WaitingPlayers { get; private set;} = new List<Player>();
    public List<Player> PlayerWithoutFunds { get; private set; } = new List<Player>();

    public int CurrentPlayerIndex { get; private set; } = 0;

    public Player CurrentPlayer => Players[CurrentPlayerIndex];

    public Player NextPlayer => Players[NextPlayerIndex()];

    public void AddPlayer(Player player)
    {
        Players.Add(player);
    }

    public void AddPlayerToWaitingList(Player player)
    {
        WaitingPlayers.Add(player);
    }

    public void MovePlayerFromWaitingListToTable(Player player)
    {
        WaitingPlayers.Remove(player);
        Players.Add(player);
    }

    public void MovePlayerToPlayerWithoutFunds(Player player)
    {
        Players.Remove(player);
        PlayerWithoutFunds.Add(player);
    }

    public void RemovePlayer(string userId)
    {
        Players.RemoveAll(player => player.User.Id == userId);
        WaitingPlayers.RemoveAll(player => player.User.Id == userId);
        PlayerWithoutFunds.RemoveAll(player => player.User.Id == userId);   
    }

    public bool IsPlayerInTable(string userId)
    {
        return Players.Any(player => player.User.Id == userId);
    }

    public int NextPlayerIndex()
        => (CurrentPlayerIndex + 1) % Players.Count;

    public void MoveToNextPlayer()
    {
        CurrentPlayer.SetTurn(false);
        CurrentPlayerIndex = NextPlayerIndex();
        CurrentPlayer.SetTurn(true);
    }

    public void ResetTurns()
    {
        CurrentPlayerIndex = 0;
    }

}
