using System.Collections.Generic;
using Simulation.Loads;

namespace Simulation.Messages.Forsman
{
    public class ForsmanPushRequest : Request
    {
        public ForsmanPushRequest(int target, int sender, HostLoadInfo hostLoad,int auctionId, List<ContainerLoadInfo> containerLoads) : base(target, sender, hostLoad, MessageTypes.PushRequest)
        {
            AuctionId = auctionId;
            ContainerLoads = containerLoads;
        }

        public int AuctionId { get; set; }
        public List<ContainerLoadInfo> ContainerLoads { get; private set; }

    }
}
