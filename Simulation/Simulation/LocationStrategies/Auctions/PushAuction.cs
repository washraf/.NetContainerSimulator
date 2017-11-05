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

        //public override AuctionFailureAction GetAction()
        //{
        //    return AuctionFailureAction.AddHost;

        //    //var count = InValidBids.Count(x => x.Reason == BidReasons.FullLoad);
        //    //if (count == ExpectedBids)// && ValidBids.Count==0)
        //    //{
        //    //    return AuctionFailureAction.AddHost;
        //    //}
        //    //return AuctionFailureAction.Nothing;
        //}
        private int ContainerId { get; set; }

        public override Bid GetWinnerBid()
        {
            if (ValidBids.Any())
            {
                var winner = ValidBids.OrderBy(x => x.NewLoadInfo.Volume).First();

                return winner;
            }
            //throw new Exception();

            return null;
        }
    }
}
