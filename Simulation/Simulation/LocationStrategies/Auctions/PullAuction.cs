using System;
using System.Collections.Generic;
using System.Linq;
using Simulation.LocationStrategies;
using Simulation.LocationStrategies.Auctions;

namespace Simulation.LocationStrategies.Auctions
{
    public class PullAuction:Auction
    {
        public PullAuction(int auctionId, int auctionOwner, List<int> candidates) 
            :base(auctionId,auctionOwner, candidates, StrategyActionType.PullAction)
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