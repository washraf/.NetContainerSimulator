using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Loads;

namespace Simulation.LocationStrategies.Forsman2015
{
    public abstract class ForsmanAuction
    {
        protected HostLoadInfo PredictedOwnerHostLoadInfo { get; private set; }

        
        private readonly int _candidates;
        public bool OpenSession { get; set; } = true;

        protected readonly List<Bid> Bids = new List<Bid>();
        //protected readonly List<Bid> InValidBids = new List<Bid>();
        protected readonly Dictionary<int, HostLoadInfo> HostsLoads;

        public ForsmanAuction(HostLoadInfo predictedOwnerHostLoadInfo, int candidates)
        {
            PredictedOwnerHostLoadInfo = predictedOwnerHostLoadInfo;
            HostsLoads = new Dictionary<int, HostLoadInfo>();
           
            _candidates = candidates;
        }

        public void AddBid(Bid bid,double min,double max)
        {
            if (bid.Valid)
            {
                var state = bid.NewLoadInfo.CalculateTotalUtilizationState(min, max);
                if (state==UtilizationStates.Normal)
                    Bids.Add(bid);
            }
            else
            {
                throw new NotImplementedException();
                //InValidBids.Add(bid);
            }

        }

        public abstract Bid GetWinnerBid();

        //public abstract AuctionFailureAction GetAction();

        public void EndWaitFor(int senderId, HostLoadInfo currentHostLoadInfo)
        {
            HostsLoads.Add(senderId, currentHostLoadInfo);
            //HostsLoads.AddOrUpdate(senderId,currentHostLoadInfo);
            if (HostsLoads.Count == _candidates)
            {
                OpenSession = false;
            }
        }

        protected List<HostLoadInfo> ToBeHostLoadInfos(int hId, HostLoadInfo loadInfo)
        {
            List<HostLoadInfo> hostLoadInfos = new List<HostLoadInfo>();
            foreach (var hostLoad in HostsLoads.Values.Where(x=>x!=null))
            {
                if (hostLoad.HostId == hId) 
                {
                    hostLoadInfos.Add(loadInfo);
                }
                else
                    hostLoadInfos.Add(hostLoad);
            }
            return hostLoadInfos;
        } 

    }
}
