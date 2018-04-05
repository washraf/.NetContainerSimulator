using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Loads;

namespace Simulation.LocationStrategies.Forsman2015
{
    public class ForsmanBid : Bid
    {
        public ForsmanBid(int host, bool valid, HostLoadInfo load, int targetAuction,
            int containerId, BidReasons reason,
            ContainerLoadInfo containerLoadInfo = null) 
            : base(host, valid, load, targetAuction, containerId, reason)
        {
            ContainerLoadInfo = containerLoadInfo;
        }
        public ContainerLoadInfo ContainerLoadInfo { get; set; }

    }
}
