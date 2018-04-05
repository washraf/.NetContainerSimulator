using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.LocationStrategies.Forsman2015;

namespace Simulation.Messages.Forsman
{
    public class ForsmanLoadAvailabilityResponce : Message
    {
        //This is the Bid
        public ForsmanLoadAvailabilityResponce(int target, int sender,HostLoadInfo oldInfo, bool valid,List<ForsmanBid> bids)
            : base(target, sender, MessageTypes.LoadAvailabilityResponse)
        {
            //Console.WriteLine($"LoadAvailabilityResponce for {auctionId} Created to host #{sender}");
            OldLoadInfo = oldInfo;
            Valid = valid;
            Bids = bids;
        }

        public bool Valid { get; private set; }
        public List<ForsmanBid> Bids { get; private set; }
        public HostLoadInfo OldLoadInfo { get; private set; }
    }
}
