using System.Collections.Generic;
using System.Linq;

namespace Simulation.LocationStrategies.Auctions
{
    public class LeastPullsAuction : Auction
    {
        public LeastPullsAuction(int instanceId, int owner, List<int> candidates, StrategyActionType actionType) 
            : base(instanceId, owner, candidates, actionType)
        {
        }

        public override Bid GetWinnerBid()
        {
            if (ValidBids.Any())
            {
                var winner = ValidBids.OrderBy(x=>x.ImagePulls).First();

                return winner;
            }
            return null;
        }
    }
}
