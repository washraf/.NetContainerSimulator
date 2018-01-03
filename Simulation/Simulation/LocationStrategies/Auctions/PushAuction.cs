using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.LocationStrategies;
using Simulation.LocationStrategies.Auctions;

namespace Simulation.LocationStrategies.Auctions
{
    public class PushAuction:Auction
    {
        public PushAuction(int auctionId,int auctionOwner,int containerId,List<int> candidates)
            :base(auctionId,auctionOwner,candidates, StrategyActionType.PushAction)
        {
            ContainerId = containerId;
        }

        private int ContainerId { get; set; }

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
