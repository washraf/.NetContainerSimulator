using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Messages
{
    public class PullLoadAvailabilityRequest : Message
    {
        public PullLoadAvailabilityRequest(int target, int sender, int auctionId,int requestOwner)
            : base(target, sender, MessageTypes.PullLoadAvailabilityRequest)
        {
            //Console.WriteLine($"LoadAvailabilityRequest for {auctionId} Created to host #{target}");

            AuctionId = auctionId;
            RequestOwner = requestOwner;
        }

        public int AuctionId { get; private set; }
        public int RequestOwner { get; }
    }
}
