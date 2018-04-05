using System.Collections.Generic;
using System.Linq;

namespace Simulation.LocationStrategies.Auctions
{
    public class RandomAuction : Auction
    {
        public RandomAuction(int auctionId, int auctionOwner, List<int> candidates, StrategyActionType strategyActionType)
            : base(auctionId, auctionOwner, candidates, strategyActionType)
        {

        }

        public override Bid GetWinnerBid()
        {
            if (ValidBids.Any())
            {

                var winner = ValidBids.First();
                return winner;
            }
            return null;
        }
    }
}