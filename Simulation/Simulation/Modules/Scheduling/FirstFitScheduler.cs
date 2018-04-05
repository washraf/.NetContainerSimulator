using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simulation.Messages;
using Simulation.DataCenter.Containers;
using Simulation.DataCenter.InformationModules;
using Simulation.DataCenter;
using Simulation.Loads;
using Simulation.DataCenter.Machines;
using Simulation.LocationStrategies;
using Simulation.LocationStrategies.WAshraf2017;

namespace Simulation.Modules.Scheduling
{

    public class FirstFitScheduler : BaseScheduler
    {
        public FirstFitScheduler(UtilizationTable holder,
            NetworkInterfaceCard communicationModule,
            IMachinePowerController powerContoller):base(holder,communicationModule,powerContoller)
        {
            
        }

        InOrderProping inOrderProping = null;
        protected override void AddContainer(Container container)
        {
            if (CurrentContainer != null)
                throw new NotImplementedException("How come");
            List<int> candidates = new List<int>( Holder.GetCandidateHosts(UtilizationStates.Normal, 0));
            var under = Holder.GetCandidateHosts(UtilizationStates.UnderUtilization, 0);
            candidates.AddRange(under);
            
            if (candidates.Count > 0)
            {
                CurrentContainer = container;
                int instanceId = Helpers.RandomNumberGenerator.GetInstanceRandomNumber();
                inOrderProping = new InOrderProping(instanceId, 0, candidates, StrategyActionType.Scheduling, container.GetContainerPredictedLoadInfo());
                TestHostForAContainer();
            }
            else
            {
                FailedScheduling();
            }
        }
        protected void FailedScheduling()
        {
            powerContoller.PowerOnHost();
            Containers.Enqueue(CurrentContainer);
            if (Containers.Count > 100)
            {

            }
            CurrentContainer = null;
            inOrderProping = null;
        }
        private void TestHostForAContainer()
        {
            var id = inOrderProping.GetNextCandidate();
            Message m = new CanHaveContainerRequest(id, 0, inOrderProping.InstanceId, CurrentContainer.GetContainerPredictedLoadInfo());
            CommunicationModule.SendMessage(m);
        }
        protected override void HandleCanHaveContainerResponce(CanHaveContainerResponce message)
        {
            if (message.Bid.AuctionId != inOrderProping.InstanceId)
                throw new NotImplementedException("How come");
            if (message.Bid.Valid)
            {
                AddContainerRequest request = new AddContainerRequest(message.SenderId, 0, CurrentContainer);
                CommunicationModule.SendMessage(request);
                CurrentContainer = null;
                inOrderProping = null;

            }
            else
            {
                if (inOrderProping.OpenSession)
                {
                    TestHostForAContainer();
                }
                else
                {
                    FailedScheduling();
                }
            }
            
        }

    }
}
