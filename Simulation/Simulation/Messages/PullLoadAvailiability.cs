using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.LocationStrategies;

namespace Simulation.Messages
{
    public class PullLoadAvailabilityRequest : Message
    {
        public PullLoadAvailabilityRequest(int target, int sender, int auctionId)
            : base(target, sender, MessageTypes.PullLoadAvailabilityRequest)
        {
            //Console.WriteLine($"LoadAvailabilityRequest for {auctionId} Created to host #{target}");

            AuctionId = auctionId;
        }

        public int AuctionId { get; private set; }
    }

    public class LoadAvailabilityResponce : Message
    {
        //This is the Bid
        public LoadAvailabilityResponce(int target, int sender, int auctionId, Bid bid)
            : base(target, sender, MessageTypes.LoadAvailabilityResponse)
        {
            //Console.WriteLine($"LoadAvailabilityResponce for {auctionId} Created to host #{sender}");
            //OldLoadInfo = oldInfo;
            AuctionId = auctionId;
            HostBid = bid;
        }

        public int AuctionId { get; private set; }
        public Bid HostBid { get; private set; }
        //public HostLoadInfo OldLoadInfo { get; private set; }
    }
}
