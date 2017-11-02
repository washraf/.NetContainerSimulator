using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.LocationStrategies;
using Simulation.Auctions;

namespace Simulation.LocationStrategies.Auctions
{
    public abstract class Auction:LocationStrategy
    {
        public Auction(int instanceId,int owner,List<int> candidates, StrategyActionType actionType):base(instanceId,owner,candidates,actionType)
        {
            
        }

        

        protected readonly List<Bid> ValidBids = new List<Bid>();
        protected readonly List<Bid> InValidBids = new List<Bid>();


        public void AddBid(Bid bid)
        {
            _remaningBids--;
            if (bid.Valid)
            {
                ValidBids.Add(bid);
            }
            else
            {
                InValidBids.Add(bid);
            }
            if (_remaningBids == 0)
            {
                OpenSession = false;
            }
        }

        public abstract Bid GetWinnerBid();

        //public abstract AuctionFailureAction GetAction();

    }
}
