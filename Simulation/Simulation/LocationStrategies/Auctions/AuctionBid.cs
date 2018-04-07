using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Loads;

namespace Simulation.LocationStrategies.Auctions
{
    public class AuctionBid : Bid
    {
        public AuctionBid(int host, bool valid, HostLoadInfo load,
            int targetAuction, int containerId, BidReasons reason,int imagePulls) 
            : base(host, valid, load, targetAuction, containerId, reason)
        {
            ImagePulls = imagePulls;
        }

        public int ImagePulls { get; }
    }
}
