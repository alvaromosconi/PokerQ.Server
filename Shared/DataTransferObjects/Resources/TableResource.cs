using Entities;

namespace Shared.DataTransferObjects.Resources;
public record TableResource(List<PlayerResource> Players,
                            int SmallBlind, 
                            int BigBlind, 
                            int Pot, 
                            int CurrentBet, 
                            PlayerResource CurrentPlayer, 
                            List<CardResource> CommunityCards,
                            Stage Stage,
                            Player Winner
);
