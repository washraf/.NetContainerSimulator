using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Loads;
using Simulation.LocationStrategies;

namespace Simulation.LocationStrategies.InOrderProping
{
    public class InOrderProping:LocationStrategy
    {

        public InOrderProping(int auctionId, int owner,List<int> candidates, StrategyActionType actionType, ContainerLoadInfo loadInfo) 
            : base(auctionId, owner,candidates, actionType)
        {
            if (actionType == StrategyActionType.PullAction && ContainerLoadInfo != null)
            {
                throw new NotImplementedException();
            }

            ContainerLoadInfo = loadInfo;
        }

        public ContainerLoadInfo ContainerLoadInfo { get; set; }

        public int GetNextCandidate()
        {
            int n = ExpectedBids - _remaningBids;
            _remaningBids --;
            if (_remaningBids == 0)
            {
                OpenSession = false;
            }
            return _candidates[n];
        }
    }
}
