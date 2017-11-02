using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Auctions;
using Simulation.DataCenter;
using Simulation.DataCenter.InformationModules;
using Simulation.Helpers;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.LocationStrategies.Auctions;
using Simulation.Messages;

namespace Simulation.Modules.Management.Master.Mine
{
    public class AuctionManagement : MineCommon
    {
        private Auction _currentAuction;
        public AuctionManagement(NetworkInterfaceCard communicationModule,
            IMachinePowerController powerController,
            UtilizationTable holder) 
            : base(communicationModule,powerController,holder)
        {
        }

       
        protected override void HandlePushRequest(PushRequest message, List<int> candidates)
        {
            int instanceId = Helpers.RandomNumberGenerator.GetInstanceRandomNumber();
            PushAuction pushAuction = new PushAuction(instanceId, message.SenderId, message.SelectedContainerLoadInfo.ContainerId, candidates);
            foreach (var candidateHostId in candidates)
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
            int instanceId = RandomNumberGenerator.GetInstanceRandomNumber();
            int count = candidates.Count();
            PullAuction pullAuction = new PullAuction(instanceId, message.SenderId, candidates);
            foreach (var candidateHostId in candidates)
            {
                if (candidateHostId == 0)
                {
                    throw new NotImplementedException();
                }
                PullLoadAvailabilityRequest request = new PullLoadAvailabilityRequest(candidateHostId, this.MachineId, instanceId);
                CommunicationModule.SendMessage(request);
                //Console.WriteLine($"+\n\tSending Message for Host #{candidateHostId} and Auction #{auctionId}");
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
                    InitiateRejectAction(_currentAuction.Owner, _currentAuction.ActionType);
                }
                else
                {
                    if (winner.Reason == BidReasons.None) { }
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
                    if (winner != null && candidate != winner.BiddingHost)
                    {
                        BidCancellationRequest request = new BidCancellationRequest(candidate, this.MachineId, _currentAuction.InstanceId);
                        CommunicationModule.SendMessage(request);
                        //Console.WriteLine($"send cancel to {bidder} on auction {currentAuction.AuctionId}");
                    }
                }
                //Console.WriteLine($"\t - Closing Auction #{currentAuction.AuctionId}");
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
