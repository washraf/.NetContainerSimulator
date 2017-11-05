using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Loads;

namespace Simulation.LocationStrategies
{
    public abstract class LocationStrategy
    {
        protected LocationStrategy(int auctionId,int owner,List<int> candidates, StrategyActionType actionType)
        {
            InstanceId = auctionId;
            Owner = owner;
            ActionType = actionType;
            ExpectedBids = candidates.Count;
            _remaningBids = candidates.Count;
            _candidates = candidates;
        }

        public int ExpectedBids { get; set; }
        protected int _remaningBids;
        public int InstanceId { get; private set; }
        public int Owner { get; private set; }
        public StrategyActionType ActionType { get; private set; }

        public bool OpenSession { get; protected set; } = true;

        protected readonly List<int> _candidates;
        
        public IReadOnlyList<int> GetAllCandidates()
        {
            return _candidates;
        }

    }
}
