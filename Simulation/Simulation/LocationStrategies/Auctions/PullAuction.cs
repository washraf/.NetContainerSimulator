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

        //public override AuctionFailureAction GetAction()
        //{
        //    return AuctionFailureAction.RemoveHost;
        //    //if (InValidBids.Any(x => x.Reason == BidReasons.MinimumLoad))
        //    //{
        //    //    return AuctionFailureAction.RemoveHost;
        //    //}

        //    //var count = InValidBids.Count(x => x.Reason == BidReasons.MinimumLoad);
            
        //    //if (count == ExpectedBids)
        //    //{
        //    //    return AuctionFailureAction.RemoveHost;
        //    //}
        //    //return AuctionFailureAction.Nothing;
        //}

        public override Bid GetWinnerBid()
        {
            if (ValidBids.Any())
            {

                var winner = ValidBids.OrderBy(x => x.NewLoadInfo.Volume).Last();
                //Bid winner = ValidBids.First();
                //var volume = ValidBids.First().NewLoadInfo.Volume;
                //foreach (var bid in ValidBids)
                //{
                //    if (volume < bid.NewLoadInfo.Volume)
                //    {
                //        volume = bid.NewLoadInfo.Volume;
                //        winner = bid;
                //    }
                //}
                return winner;
            }
            //throw new Exception();
            return null;
        }
    }
}