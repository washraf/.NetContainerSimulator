using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.LocationStrategies;
using Simulation.LocationStrategies.Auctions;

namespace Simulation.LocationStrategies.Auctions
{
    public class LeastFullAuction:Auction
    {
        public LeastFullAuction(int auctionId,int auctionOwner,List<int> candidates, StrategyActionType strategyActionType)
            :base(auctionId,auctionOwner,candidates, strategyActionType)
        {
        }


        public override Bid GetWinnerBid()
        {
            if (ValidBids.Any())
            {
                var winner = ValidBids.OrderBy(x => x.NewLoadInfo.Volume).First();

                return winner;
            }
            return null;
        }
    }
}
