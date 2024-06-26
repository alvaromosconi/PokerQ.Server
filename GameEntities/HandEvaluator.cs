﻿namespace Entities;
public class HandEvaluator
{
    public enum HandRank
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }
    public HandRank EvaluateHand(List<Card> hand)
    {
        if (IsRoyalFlush(hand)) return HandRank.RoyalFlush;
        if (IsStraightFlush(hand)) return HandRank.StraightFlush;
        if (IsFourOfAKind(hand)) return HandRank.FourOfAKind;
        if (IsFullHouse(hand)) return HandRank.FullHouse;
        if (IsFlush(hand)) return HandRank.Flush;
        if (IsStraight(hand)) return HandRank.Straight;
        if (IsThreeOfAKind(hand)) return HandRank.ThreeOfAKind;
        if (IsTwoPair(hand)) return HandRank.TwoPair;
        if (IsOnePair(hand)) return HandRank.OnePair;
        return HandRank.HighCard;
    }

    private bool IsRoyalFlush(List<Card> hand)
    {
        return IsStraightFlush(hand) && 
               hand.All(card => card.Rank >= Card.RankEnum.Ten);
    }

    private bool IsStraightFlush(List<Card> hand)
    {
        return IsFlush(hand) && 
               IsStraight(hand);
    }

    private bool IsFourOfAKind(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Rank);
        
        return rankGroups.Any(group => group.Count() == 4);
    }

    private bool IsFullHouse(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Rank);
        
        return rankGroups.Any(group => group.Count() == 3) &&
               rankGroups.Any(group => group.Count() == 2);
    }

    private bool IsFlush(List<Card> hand)
    {
        return hand.GroupBy(card => card.Suit)
                   .Count() == 1;
    }

    private bool IsStraight(List<Card> hand)
    {
        var sortedRanks = hand.Select(card => (int)card.Rank)
                              .OrderBy(rank => rank)
                              .ToList();

        if (sortedRanks.Last() == (int)Card.RankEnum.Ace && 
            sortedRanks.First() == (int)Card.RankEnum.Two)
        {
            // Handle A-2-3-4-5 as a valid straight (wheel)
            sortedRanks.Remove(sortedRanks.Last());
            sortedRanks.Insert(0, 1);
        }
        for (int i = 1; i < sortedRanks.Count; i++)
        {
            if (sortedRanks[i] != sortedRanks[i - 1] + 1)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsThreeOfAKind(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Rank);
        
        return rankGroups.Any(group => group.Count() == 3);
    }

    private bool IsTwoPair(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Rank);
        
        return rankGroups.Count(group => group.Count() == 2) == 2;
    }

    private bool IsOnePair(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Rank);
       
        return rankGroups.Any(group => group.Count() == 2);
    }
}
