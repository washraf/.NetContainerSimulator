using System;
using System.Collections.Generic;
using Simulation.Messages;
using Simulation.DataCenter.Containers;
using Simulation.DataCenter.InformationModules;
using Simulation.DataCenter;
using Simulation.Loads;
using Simulation.DataCenter.Machines;
using Simulation.LocationStrategies;
using Simulation.LocationStrategies.Auctions;
using Simulation.Configuration;

namespace Simulation.Modules.Scheduling
{
    public class AuctionScheduler : BaseScheduler
    {
        public AuctionScheduler(UtilizationTable holder, NetworkInterfaceCard communicationModule, IMachinePowerController powerContoller, SchedulingAlgorithm schedulingAlgorithm) : base(holder, communicationModule, powerContoller)
        {
            this.schedulingAlgorithm = schedulingAlgorithm;
        }

        private Auction auction = null;
        private readonly SchedulingAlgorithm schedulingAlgorithm;

        protected override void AddContainer(Container container)
        {
            if (currentContainer != null)
                throw new NotImplementedException("How come");
            List<int> candidates = new List<int>(Holder.GetCandidateHosts(UtilizationStates.Normal, 0));
            var under = Holder.GetCandidateHosts(UtilizationStates.UnderUtilization, 0);
            candidates.AddRange(under);

            if (candidates.Count > 0)
            {
                currentContainer = container;
                AuctionFactory(candidates);
                foreach (var id in candidates)
                {
                    Message m = new CanHaveContainerRequest(id, 0, auction.InstanceId, currentContainer.GetContainerPredictedLoadInfo());
                    CommunicationModule.SendMessage(m);
                }

            }
            else
            {
                FailedScheduling();
            }
        }

        private void AuctionFactory(List<int> candidates)
        {
            int instanceId = Helpers.RandomNumberGenerator.GetInstanceRandomNumber();
            switch (schedulingAlgorithm)
            {
                case SchedulingAlgorithm.MFull:
                    auction = new MostFullAuction(instanceId, 0, candidates, StrategyActionType.Scheduling);
                    break;
                case SchedulingAlgorithm.LFull:
                    auction = new LeastFullAuction(instanceId, 0, candidates, StrategyActionType.Scheduling);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        protected void FailedScheduling()
        {
            powerContoller.PowerOnHost();
            Containers.Enqueue(currentContainer);
            currentContainer = null;
            auction = null;
        }
        
        protected override void HandleCanHaveContainerResponce(CanHaveContainerResponce message)
        {
            if (message.Bid.AuctionId != auction.InstanceId)
                throw new NotImplementedException("How come");
            auction.AddBid(message.Bid);
            if (!auction.OpenSession)
            {
                var winner = auction.GetWinnerBid();
                if (winner == null)
                {
                    FailedScheduling();
                }
                else
                {
                    AddContainerRequest request = new AddContainerRequest(message.SenderId, 0, currentContainer);
                    CommunicationModule.SendMessage(request);
                    currentContainer = null;
                    auction = null;
                }

               
            }
        }

    }
}
