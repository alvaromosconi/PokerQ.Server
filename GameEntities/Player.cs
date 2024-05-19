
namespace Entities;

public class Player(User user)
{

    public User User { get; private set; } = user;
    public bool IsSmallBlind { get; private set; } = false;
    public bool IsBigBlind { get; private set; } = false;

    public IReadOnlyCollection<Card> HoleCards { get; private set; } = new List<Card>();
    public IReadOnlyCollection<Card> CommunityCards { get; private set; } = new List<Card>();
    public IEnumerable<Card> Cards => HoleCards.Concat(CommunityCards);

    public int CurrentBet { get; private set; } = 0;
    public int Balance { get; private set; } = 1000;

    public bool Turn { get; private set; } = false;
    public bool HasActed { get; private set; } = false;
    public PlayerAction CurrentAction { get; private set; } = PlayerAction.None;
    public List<PlayerAction> ValidActions { get; private set; } = new List<PlayerAction>();

    public void SetHoleCards(IReadOnlyCollection<Card> holeCards)
        => HoleCards = holeCards;

    public void SetCommunityCards(IReadOnlyCollection<Card> cards)
        => CommunityCards = cards;

    public List<PlayerAction> CalculateValidActions(int highestBet)
    {
        var validActions = new List<PlayerAction>();

        if (Turn && Balance > 0)
        {
            if (CurrentAction != PlayerAction.Fold)
            {
                if (highestBet > CurrentBet)
                {
                    if (highestBet >= Balance)
                    {
                        validActions.Add(PlayerAction.AllIn);
                    }
                    else
                    {
                        validActions.Add(PlayerAction.Call);
                        validActions.Add(PlayerAction.Bet);
                    }
                }    
                else if (highestBet == CurrentBet)
                {
                    validActions.Add(PlayerAction.Check);
                }

                validActions.Add(PlayerAction.Fold);
            }
        }
      
        ValidActions = validActions;

        return validActions;
    }

    public void Bet(int amount)
    {
        SetAction(PlayerAction.Bet);
        AdjustBetAndBalance(amount);
    }

    public void Call(int highestBet)
    {
        SetAction(PlayerAction.Call);
        int moneyToEqualHighestBet = CalculateMoneyToEqualHighestBet(highestBet);
        AdjustBetAndBalance(moneyToEqualHighestBet);
    }

    public void Fold()
    {
        SetAction(PlayerAction.Fold);
        CurrentBet = 0;
    }

    public void Check()
    {
        SetAction(PlayerAction.Check);
    }

    private void AdjustBetAndBalance(int amount)
    {
        CurrentBet += amount;
        Balance -= amount;
    }

    public void Reset()
    {
        ResetActionAndCurrentBet();
        HoleCards = new List<Card>();
        CommunityCards = new List<Card>();
    }

    public void ResetActionAndCurrentBet()
    {
        CurrentAction = PlayerAction.None;
        ValidActions = new List<PlayerAction>();
        CurrentBet = 0;
        HasActed = false;
    }

    public int CalculateMoneyToEqualHighestBet(int highestBet)
    {
        return highestBet - CurrentBet;
    }

    public void SetAction(PlayerAction action)
    {
        CurrentAction = action;
        if (action != PlayerAction.None)
            HasActed = true;
    }

    public void SetTurn(bool isPlayerTurn)
    {
        Turn = isPlayerTurn;
    }

    public void SetAsSmallBlind(bool isSmallBlind)
    {
        IsSmallBlind = isSmallBlind;
    }

    public void SetAsBigBlind(bool isBigBlind)
    {
        IsBigBlind = isBigBlind;
    }

    public void AddToBalance(int amount)
    {
        Balance += amount;
    }

    public void ReduceCurrentBet(int amount)
    {
        CurrentBet -= amount;
    }

    public void BlockAllActions()
    {
        ValidActions = new List<PlayerAction>();
    }
}

public sealed class EmptyPlayer : Player
{
    public EmptyPlayer() : base(null)
    {
    }
}
