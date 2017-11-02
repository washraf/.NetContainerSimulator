using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Loads;
using Simulation.LocationStrategies;

namespace Simulation.Messages.Other
{
    public class OtherPushRequest : Request
    {

        public OtherPushRequest(int target, int sender, HostLoadInfo hostLoad,int auctionId, List<ContainerLoadInfo> containerLoads) : base(target, sender, hostLoad, MessageTypes.PushRequest)
        {
            AuctionId = auctionId;
            ContainerLoads = containerLoads;
        }

        public int AuctionId { get; set; }
        public List<ContainerLoadInfo> ContainerLoads { get; private set; }

    }
    public class OtherPullRequest : Request
    {

        public OtherPullRequest(int target, int sender, HostLoadInfo hostLoad, int auctionId) : base(target, sender, hostLoad, MessageTypes.PullRequest)
        {
            AuctionId = auctionId;
        }

        public int AuctionId { get; set; }

    }
    public class OtherLoadAvailabilityResponce : Message
    {
        //This is the Bid
        public OtherLoadAvailabilityResponce(int target, int sender,HostLoadInfo oldInfo, bool valid,List<Bid> bids)
            : base(target, sender, MessageTypes.LoadAvailabilityResponse)
        {
            //Console.WriteLine($"LoadAvailabilityResponce for {auctionId} Created to host #{sender}");
            OldLoadInfo = oldInfo;
            Valid = valid;
            Bids = bids;
        }

        public bool Valid { get; private set; }
        public List<Bid> Bids { get; private set; }
        public HostLoadInfo OldLoadInfo { get; private set; }
    }
}
