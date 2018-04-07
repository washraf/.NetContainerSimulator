using System;
using Simulation.Helpers;
using Simulation.Loads;

namespace Simulation.Messages
{
    /// <summary>
    /// The Broad Casted Request to all prospective Hosts
    /// </summary>
    public class PushLoadAvailabilityRequest : Message
    {
        public PushLoadAvailabilityRequest(int target,int sender,ContainerLoadInfo containerLoadInfo,int auctionId,int requestOwner) 
            : base(target,sender, MessageTypes.PushLoadAvailabilityRequest)
        {
            //Console.WriteLine($"LoadAvailabilityRequest for {auctionId} Created to host #{target}");

            NewContainerLoadInfo = containerLoadInfo;
            AuctionId = auctionId;
            RequestOwner = requestOwner;
        }

        public int AuctionId { get; private set; }
        public int RequestOwner { get; }
        public ContainerLoadInfo NewContainerLoadInfo { get; private set; }
    }
}