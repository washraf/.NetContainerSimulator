using System;
using System.Collections.Generic;
using System.Linq;
using Simulation.LocationStrategies;
using Simulation.LocationStrategies.Auctions;

namespace Simulation.LocationStrategies.Auctions
{
    public class MostFullAuction:Auction
    {
        public MostFullAuction(int auctionId, int auctionOwner, List<int> candidates, StrategyActionType strategyActionType) 
            :base(auctionId,auctionOwner, candidates, strategyActionType)
        {

        }

        public override Bid GetWinnerBid()
        {
            if (ValidBids.Any())
            {

                var winner = ValidBids.OrderBy(x => x.NewLoadInfo.Volume).Last();
                return winner;
            }
            return null;
        }
    }
}