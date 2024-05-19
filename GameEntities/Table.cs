
namespace Entities;

public class Table
{
    private readonly Dealer Dealer = new Dealer(new Deck());
    private readonly PlayerManager PlayerManager = new PlayerManager();
    public readonly int SmallBlind = 50;
    public readonly int BigBlind = 100;

    public List<Player> Players => PlayerManager.Players;
    public List<Player> WaitingPlayers => PlayerManager.WaitingPlayers;
    public Player CurrentPlayer => PlayerManager.CurrentPlayer;

    private IReadOnlyCollection<Card> Flop { get; set; }
        = new List<Card> { new EmptyCard(),
                           new EmptyCard(),
                           new EmptyCard() };

    public Card Turn { get; private set; } = new EmptyCard();
    public Card River { get; private set; } = new EmptyCard();

    public int Pot { get; private set; } = 0;
    private List<int> SidePots { get; set; } = new List<int>();
    public int CurrentBet => Players.Sum(p => p.CurrentBet);
    public int HighestBet => Players.Max(p => p.CurrentBet);

    public Player? Winner { get; private set; } = null;
    public Stage Stage = Stage.Initial;
    public List<Card> CommunityCards => (Flop.Append(Turn)
                                             .Append(River)).ToList();

    public int PlayersCount => Players.Count;

    public void NextStage()
    {
        if (Stage == Stage.Initial)
        {
            StartBettingRound();
        }
        
        DealCardsBasedOnStage();
        CurrentPlayer.CalculateValidActions(HighestBet);
    }
    private void StartBettingRound()
    {
        Player smallBlind = CurrentPlayer;
        Player bigBlind = PlayerManager.NextPlayer;

        smallBlind.SetAsSmallBlind(true);
        bigBlind.SetAsBigBlind(true);

        smallBlind.SetTurn(true);
        HandlePlayerAction(PlayerAction.Bet, SmallBlind);
        HandlePlayerAction(PlayerAction.Bet, BigBlind);
    }

    public void HandlePlayerAction(PlayerAction action, int? amount)
    {
        PerformAction(action, amount);
        PlayerManager.MoveToNextPlayer();
        CurrentPlayer.CalculateValidActions(HighestBet);
        
        if (IsBettingRoundComplete())
        {
            if (Players.All(p => p.Balance == 0|| p.CurrentAction == PlayerAction.AllIn) || 
                Players.Count(p => p.CurrentAction == PlayerAction.Fold) == PlayersCount - 1)
            {
                do
                {
                    NextStage();
                } while (Stage != Stage.Showdown);
            }       
            else
            {
                Players.ForEach(p => p.ResetActionAndCurrentBet());
                NextStage();
            }
            
        }

        if (Stage == Stage.Showdown)
            SetRoundWinner();
    }

    private bool IsActionValid(PlayerAction action)
    {
        return CurrentPlayer.CalculateValidActions(HighestBet)
                            .Contains(action);
    }

    private void PerformAction(PlayerAction action, int? amount)
    {
        switch (action)
        {
            case PlayerAction.Bet:
                Bet(amount);
                break;
            case PlayerAction.Call:
                Call();
                break;
            case PlayerAction.AllIn:
                Bet(CurrentPlayer.Balance);
                break;
            case PlayerAction.Check:
                Check();
                break;
            case PlayerAction.Fold:
            default:
                Fold();
                break;
        }

        CurrentPlayer.BlockAllActions();
    }


    private void DealCardsBasedOnStage()
    {
        Stage = Stage switch
        {
            Stage.Initial => DealHoleCards(),
            Stage.PreFlop => DealFlop(),
            Stage.Flop => DealTurn(),
            Stage.Turn => DealRiver(),
            Stage.River => MoveToShowdown(),
            Stage.Showdown => Stage.Initial,
            _ => Stage.Initial
        };
    }

    private Stage DealHoleCards()
    {
        Players.ForEach(player => player.SetHoleCards(Dealer.DealCards(2)));
        
        return Stage.PreFlop;
    }

    private Stage DealFlop()
    {
        Flop = Dealer.DealCards(3);

        return Stage.Flop;
    }

    private Stage DealTurn()
    {
        Turn = Dealer.DealCard();

        return Stage.Turn;
    }

    private Stage DealRiver()
    {
        River = Dealer.DealCard();
        
        return Stage.River;
    }

    private Stage MoveToShowdown()
    {
        return Stage.Showdown;
    }

    public void SetRoundWinner()
    {
        if (Players.Count(p => p.CurrentAction != PlayerAction.Fold) == 1)
        {
            Winner = Players.First(p => p.CurrentAction != PlayerAction.Fold);
            Winner.AddToBalance(Pot);
        }
        else
        {
            HandEvaluator evaluator = new();
            List<Player> sortedPlayers = Players
                .Where(player => player.Cards != null)
                .OrderByDescending(player => evaluator.EvaluateHand(player.Cards.ToList()))
                .ToList();

            for (int i = 0; i < SidePots.Count; i++)
            {
                Player winner = sortedPlayers[i];
                winner.AddToBalance(SidePots[i]);
            }

            Winner = sortedPlayers[SidePots.Count];
            Winner.AddToBalance(Pot);
        }
    }

    private void Bet(int? amount)
    {
        CurrentPlayer.Bet(amount.Value);
        Pot += amount.Value;
        UpdateSidePots();
    }

    private void Call()
    {
        int moneyToCall = CurrentPlayer.CalculateMoneyToEqualHighestBet(HighestBet);
        CurrentPlayer.Call(HighestBet);
        Pot += moneyToCall;
        UpdateSidePots();
    }

    private void UpdateSidePots()
    {
        List<Player> allIns = Players.Where(p => p.CurrentAction == PlayerAction.AllIn).ToList();
        if (allIns.Count > 0)
        {
            int allInAmount = allIns.Min(p => p.CurrentBet);
            if (allInAmount > 0)
            {
                int sidePot = allInAmount * Players.Count;
                SidePots.Add(sidePot);
                Pot -= sidePot;
                Players.ForEach(p => p.ReduceCurrentBet(allInAmount));
            }
        }
    }

    private void Check()
    {
        CurrentPlayer.Check();
    }

    private void Fold()
    {
        CurrentPlayer.Fold();
    }

    public bool IsBettingRoundComplete()
    {
        bool isRoundComplete =
            Players.Count(p => p.CurrentAction != PlayerAction.Fold) <= 1 || 
            Players.All(p => p.HasActed && p.CurrentBet == HighestBet);

        return isRoundComplete;
    }


    public void ResetTable()
    {
        ResetCards();
        PlayerManager.ResetTurns();
        Players.ForEach(p => p.Reset());
        Stage = Stage.Initial;
        Pot = 0;
        Winner = null;

        while (PlayerManager.WaitingPlayers.Count > 0 && Players.Count <= 8)
        {
            PlayerManager.Players.Add(PlayerManager.WaitingPlayers[0]);
        }
    }

    public void RemovePlayersWithoutFunds()
    {
        List<Player> playersToMove = Players.FindAll(p => p.Balance < BigBlind);
        PlayerManager.PlayerWithoutFunds.AddRange(playersToMove);
    }

    private void ResetCards()
    {
        Flop = new List<Card> { new EmptyCard(), new EmptyCard(), new EmptyCard() };
        Turn = new EmptyCard();
        River = new EmptyCard();
    }

    public void AddPlayer(User user)
    {
        if (Stage == Stage.Initial)
        {
            PlayerManager.AddPlayer(new Player(user));
        }
        else
        {
            PlayerManager.AddPlayerToWaitingList(new Player(user));
        }
    }

    public void RemovePlayer(string userId)
    {
        PlayerManager.RemovePlayer(userId);
    } 

    public bool IsPlayerInTable(string userId)
        => Players.Any(p => p.User.Id == userId) || 
           WaitingPlayers.Any(p => p.User.Id == userId) ||
           PlayerManager.PlayerWithoutFunds.Any(p => p.User.Id == userId);
}
