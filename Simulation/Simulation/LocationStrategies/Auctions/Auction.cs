using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.LocationStrategies;

namespace Simulation.LocationStrategies.Auctions
{
    public abstract class Auction:LocationStrategy
    {
        public Auction(int instanceId,int owner,List<int> candidates, StrategyActionType actionType):base(instanceId,owner,candidates,actionType)
        {
            
        }

        

        protected readonly List<AuctionBid> ValidBids = new List<AuctionBid>();
        protected readonly List<AuctionBid> InValidBids = new List<AuctionBid>();


        public void AddBid(Bid bid)
        {
            var nbid = bid as AuctionBid;
            _remaningBids--;
            if (bid.Valid)
            {
                ValidBids.Add(nbid);
            }
            else
            {
                InValidBids.Add(nbid);
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
