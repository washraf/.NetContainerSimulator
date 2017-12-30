using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.DataCenter;
using Simulation.DataCenter.InformationModules;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.LocationStrategies.WAshraf2017;
using Simulation.Messages;

namespace Simulation.Modules.Management.Master.WAshraf2017
{
    public class InorderPropingManagement: MineCommon
    {
        private InOrderProping _currentProping;
        public InorderPropingManagement(NetworkInterfaceCard nic,
            IMachinePowerController powerController,
            UtilizationTable holder)
            :base(nic,powerController,holder)
        {
        }
        

        protected override void HandlePushRequest(PushRequest message, List<int> candidates)
        {
            int instanceId = Helpers.RandomNumberGenerator.GetInstanceRandomNumber();
            var inOrderInstance = new InOrderProping(instanceId, message.SenderId, candidates, StrategyActionType.PushAction, message.SelectedContainerLoadInfo);

            var candidateHostId = inOrderInstance.GetNextCandidate();
            PushLoadAvailabilityRequest request = new PushLoadAvailabilityRequest(candidateHostId, this.MachineId,
                    message.SelectedContainerLoadInfo, instanceId);
            CommunicationModule.SendMessage(request);
            if (_currentProping != null)
                throw new NotImplementedException();
            Used = message.SenderId;
            _currentProping = inOrderInstance;
            //throw new NotImplementedException();

        }

        protected override void HandlePullRequest(PullRequest message, List<int> candidates)
        {
            int instanceId = Helpers.RandomNumberGenerator.GetInstanceRandomNumber();
            var inOrderInstance = new InOrderProping(instanceId, message.SenderId, candidates, StrategyActionType.PullAction, null);
            var candidateHostId = inOrderInstance.GetNextCandidate();
            PullLoadAvailabilityRequest request = new PullLoadAvailabilityRequest(candidateHostId, this.MachineId,
                instanceId);
            CommunicationModule.SendMessage(request);
            if (_currentProping != null)
                throw new NotImplementedException();
            Used = message.SenderId;
            _currentProping = inOrderInstance;
        }

        public override void HandleLoadAvailabilityResponce(LoadAvailabilityResponce message)
        {
           // var currentInOrderPrope = _currentStrategyInstance as InOrderProping;
            if (!message.HostBid.Valid)
            {
                BidCancellationRequest brequest = new BidCancellationRequest(message.SenderId, this.MachineId, _currentProping.InstanceId);
                CommunicationModule.SendMessage(brequest);

                //Should try to find another one before rejection
                if (_currentProping.OpenSession)
                {
                    int candidateHostId = _currentProping.GetNextCandidate();
                    switch (_currentProping.ActionType)
                    {
                        case StrategyActionType.PushAction:
                            var aRequest1 = new PushLoadAvailabilityRequest(candidateHostId, this.MachineId,
                                _currentProping.ContainerLoadInfo, _currentProping.InstanceId);
                            CommunicationModule.SendMessage(aRequest1);

                            break;
                        case StrategyActionType.PullAction:
                            var aRequest2 = new PullLoadAvailabilityRequest(candidateHostId, this.MachineId,
                                _currentProping.InstanceId);
                            CommunicationModule.SendMessage(aRequest2);

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    return;
                }
                else
                {
                    InitiateRejectAction(_currentProping.Owner, _currentProping.ActionType);
                }
            }
            else
            {
                if (message.HostBid.Reason == BidReasons.ValidBid) { }
                else if (message.HostBid.Reason == BidReasons.Evacuate)
                {
                    //EvacuatingHost = message.SenderId;
                    EvacuatingHosts.Add(message.SenderId);
                    if (EvacuatingHosts.Count > 1)
                    {
                        
                    }
                    DataHolder.SetUtilization(message.SenderId, UtilizationStates.Evacuating);
                }
                else
                    throw new NotImplementedException("from in order");
                if (_currentProping.ActionType == StrategyActionType.PushAction)
                {
                    InitiateMigration(_currentProping.Owner, message.HostBid.BiddingHost, message.HostBid.ContainerId);
                }
                else
                {
                    InitiateMigration(message.HostBid.BiddingHost, _currentProping.Owner, message.HostBid.ContainerId);
                }

            }
            if (Used == 0 || _currentProping == null)
                throw new NotImplementedException("");
            Used = 0;
            _currentProping = null;
        }

    }
}
