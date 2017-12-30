using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Configuration;
using Simulation.DataCenter;
using Simulation.DataCenter.InformationModules;
using Simulation.Helpers;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.LocationStrategies.Auctions;
using Simulation.LocationStrategies.WAshraf2017;
using Simulation.Messages;

namespace Simulation.Modules.Management.Master.WAshraf2017
{
    public class AuctionManagement : MineCommon
    {
        private Auction _currentAuction;
        private InOrderProping _currentProping;
        private readonly TestedHosts TestedHostsCount;

        public AuctionManagement(NetworkInterfaceCard communicationModule,
            IMachinePowerController powerController,
            UtilizationTable holder, TestedHosts testedHosts) 
            : base(communicationModule,powerController,holder)
        {
            TestedHostsCount = testedHosts;
        }

        protected override void HandlePushRequest(PushRequest message, List<int> candidates)
        {
            var ncandidates = candidates.Take((int)TestedHostsCount).ToList();
            int instanceId = Helpers.RandomNumberGenerator.GetInstanceRandomNumber();
            PushAuction pushAuction = new PushAuction(instanceId, message.SenderId, message.SelectedContainerLoadInfo.ContainerId, ncandidates);
            Console.WriteLine($"\tMaster: Initiate a Push Auction of Host#{message.SenderId} with #{instanceId}");

            foreach (var candidateHostId in ncandidates)
            {
                if (candidateHostId == 0)
                {
                    throw new NotImplementedException();
                }
                PushLoadAvailabilityRequest request = new PushLoadAvailabilityRequest(candidateHostId, this.MachineId, message.SelectedContainerLoadInfo, instanceId);
                CommunicationModule.SendMessage(request);
                //Console.WriteLine($"+\n\tSending Message for Host #{candidateHostId} and Auction #{auctionId}");
            }
            if (_currentAuction != null)
                throw new NotImplementedException();
            //Auctions.Add(pushAuction);
            Used = message.SenderId;
            _currentAuction = pushAuction;
        }

        protected override void HandlePullRequest(PullRequest message, List<int> candidates)
        {

            var ncandidates = candidates.Take((int)TestedHostsCount).ToList();

            int instanceId = RandomNumberGenerator.GetInstanceRandomNumber();
           // int count = candidates.Count();
            PullAuction pullAuction = new PullAuction(instanceId, message.SenderId, ncandidates);
            Console.WriteLine($"\tMaster: Initiate a Pull Auction of Host#{message.SenderId} with #{instanceId}");

            foreach (var candidateHostId in ncandidates)
            {
                if (candidateHostId == 0)
                {
                    throw new NotImplementedException();
                }
                PullLoadAvailabilityRequest request = new PullLoadAvailabilityRequest(candidateHostId, this.MachineId, instanceId);
                CommunicationModule.SendMessage(request);
            }
            if (_currentAuction != null)
                throw new NotImplementedException();
            //Auctions.Add(pushAuction);
            Used = message.SenderId;
            _currentAuction = pullAuction;
        }

        public override void HandleLoadAvailabilityResponce(LoadAvailabilityResponce message)
        {
            _currentAuction.AddBid(message.HostBid);
            if (!_currentAuction.OpenSession)
            {
                var winner = _currentAuction.GetWinnerBid();
                if (winner == null)
                {
                    Console.WriteLine($"\tMaster: No Winner ---------");

                    InitiateRejectAction(_currentAuction.Owner, _currentAuction.ActionType);
                }
                else
                {
                    Console.WriteLine($"\tMaster: Winner Is Host #{winner.BiddingHost}");
                    if (winner.Reason == BidReasons.ValidBid)
                    {
                        
                    }
                    else if (winner.Reason == BidReasons.Evacuate)
                    {
                        //EvacuatingHost = message.SenderId;
                        EvacuatingHosts.Add(message.SenderId);
                        DataHolder.SetUtilization(message.SenderId, UtilizationStates.Evacuating);
                    }
                    else
                        throw new NotImplementedException("from auction");

                    //Here The Difference Exist
                    if (_currentAuction.ActionType == StrategyActionType.PushAction)
                    {
                        InitiateMigration(_currentAuction.Owner, winner.BiddingHost, winner.ContainerId);
                    }
                    else
                    {
                        InitiateMigration(winner.BiddingHost, _currentAuction.Owner, winner.ContainerId);
                    }
                }
                //Cancel All Bids
                foreach (var candidate in _currentAuction.GetAllCandidates())
                {
                    if (winner==null ||candidate != winner.BiddingHost)
                    {
                        BidCancellationRequest request = new BidCancellationRequest(candidate, this.MachineId, _currentAuction.InstanceId);
                        CommunicationModule.SendMessage(request);
                        Console.WriteLine($"\tMaster: Send cancel to {candidate} on auction {_currentAuction.InstanceId}");
                    }
                }
                Console.WriteLine($"\t Master: Closing Auction #{_currentAuction.InstanceId}");
                //Auctions.Remove(currentAuction);

                _currentAuction = null;
                if (Used == 0)
                {
                    throw new NotImplementedException();
                }
                Used = 0;
            }
        }

    }
}
