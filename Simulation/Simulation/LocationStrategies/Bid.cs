using Simulation.Helpers;
using Simulation.Loads;

namespace Simulation.LocationStrategies
{
    public enum BidReasons
    {
        //None,
        ValidBid,
        Empty,
        FullLoad,
        MinimumLoad,
        CantBid,
        Evacuate,
    }
    public class Bid
    {
        public Bid(int host,bool valid, HostLoadInfo load,int targetAuction,int containerId,BidReasons reason,ContainerLoadInfo containerLoadInfo = null)
        {
            BiddingHost = host;
            Valid = valid;
            NewLoadInfo = load;
            AuctionId = targetAuction;
            ContainerId = containerId;
            Reason = reason;
            ContainerLoadInfo = containerLoadInfo;
        }

        public int ContainerId { get; private set; }
        public BidReasons Reason { get; set; }
        public ContainerLoadInfo ContainerLoadInfo { get; set; }
        public int BiddingHost { get; private set; }
        public bool Valid { get; private set; }
        public HostLoadInfo NewLoadInfo { get; private set; }
        public int AuctionId { get; private set; }
    }
}