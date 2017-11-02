using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Helpers;
using Simulation.Loads;

namespace Simulation.LocationStrategies.Other2015
{
    public abstract class Auction2015
    {
        protected HostLoadInfo PredictedOwnerHostLoadInfo { get; private set; }

        
        private readonly int _candidates;
        public bool OpenSession { get; set; } = true;

        protected readonly List<Bid> Bids = new List<Bid>();
        //protected readonly List<Bid> InValidBids = new List<Bid>();
        protected readonly Dictionary<int, HostLoadInfo> HostsLoads;

        public Auction2015(HostLoadInfo predictedOwnerHostLoadInfo, int candidates)
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

    public class PushAuction2015 : Auction2015
    {
        protected Dictionary<int, ContainerLoadInfo> Containers { get; set; }
            = new Dictionary<int, ContainerLoadInfo>();

        public PushAuction2015(HostLoadInfo predictedOwnerHostLoadInfo, List<ContainerLoadInfo> containers, int candidates) 
            : base(predictedOwnerHostLoadInfo,candidates)
        {
            foreach (var con in containers)
            {
                Containers.Add(con.ContainerId, con);
            }
        }
        public override Bid GetWinnerBid()
        {
            if (Bids.Count == 0)
            {
                return null;
            }
            else
            {
                //Steps
                //1 : I only send push requests that are valid
                //2 : Find all containers migration costs average

                var avgCost =
                    Bids.Select(x => x.ContainerId).Select(x => Containers[x].MigrationCost).Average();
                var NewBids = Bids.Where(x => Containers[x.ContainerId].MigrationCost >= avgCost).ToList();
                Bid winner = NewBids.First();
                var lds = ToBeHostLoadInfos(NewBids[0].BiddingHost, NewBids[0].NewLoadInfo);
                var bentropy = AccountingHelpers.CalculateEntropy(lds);
                //3: Find the new entropy after migration
                for (int i = 1; i < NewBids.Count; i++)
                {
                    lds = ToBeHostLoadInfos(NewBids[i].BiddingHost, NewBids[i].NewLoadInfo);
                    var entropy = AccountingHelpers.CalculateEntropy(lds);
                    if (entropy > bentropy)
                    {
                        bentropy = entropy;
                        winner = NewBids[i];
                    }
                }
                return winner;
            }
        }
    }
    public class PullAuction2015 : Auction2015
    {
        public PullAuction2015(HostLoadInfo predictedOwnerHostLoadInfo, int candidates) 
            : base(predictedOwnerHostLoadInfo, candidates)
        {
        }
        public override Bid GetWinnerBid()
        {
            if (Bids.Count == 0)
            {
                return null;
            }
            else
            {
                //Steps
                //1 : I only send push requests that are valid
                //2 : Find all containers migration costs average

                var avgCost =
                    Bids.Select(x => x.ContainerLoadInfo.MigrationCost).Average();
                var NewBids = Bids.Where(x => x.ContainerLoadInfo.MigrationCost >= avgCost).ToList();
                Bid winner = NewBids.First();
                var lds = ToBeHostLoadInfos(NewBids[0].BiddingHost, NewBids[0].NewLoadInfo);
                var bentropy = AccountingHelpers.CalculateEntropy(lds);
                //3: Find the new entropy after migration
                for (int i = 1; i < NewBids.Count; i++)
                {
                    lds = ToBeHostLoadInfos(NewBids[i].BiddingHost, NewBids[i].NewLoadInfo);
                    var entropy = AccountingHelpers.CalculateEntropy(lds);
                    if (entropy > bentropy)
                    {
                        bentropy = entropy;
                        winner = NewBids[i];
                    }
                }
                return winner;
            }
        }
    }
}
