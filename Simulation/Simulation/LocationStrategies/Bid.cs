using Simulation.Helpers;
using Simulation.Loads;

namespace Simulation.LocationStrategies
{
    public class Bid
    {
        public Bid(int host,bool valid, HostLoadInfo load,
            int targetAuction,int containerId,BidReasons reason)
        {
            BiddingHost = host;
            Valid = valid;
            NewLoadInfo = load;
            AuctionId = targetAuction;
            ContainerId = containerId;
            Reason = reason;
        }

        public int ContainerId { get; private set; }
        public BidReasons Reason { get; set; }
        public int BiddingHost { get; private set; }
        public bool Valid { get; private set; }
        public HostLoadInfo NewLoadInfo { get; private set; }
        public int AuctionId { get; private set; }
    }
}