using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.LocationStrategies;
using Simulation.LocationStrategies.Auctions;

namespace Simulation.Auctions
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
                Bid winner = ValidBids.First();
                var volume = ValidBids.Last().NewLoadInfo.Volume;
                foreach (var bid in ValidBids)
                {
                    if (volume > bid.NewLoadInfo.Volume)
                    {
                        volume = bid.NewLoadInfo.Volume;
                        winner = bid;
                    }
                }
                return winner;
            }
            //throw new Exception();

            return null;
        }
    }
}
