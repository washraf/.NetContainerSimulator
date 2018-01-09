using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.DataCenter;
using Simulation.DataCenter.InformationModules;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.Messages;
using Simulation.Configuration;
using Simulation.Helpers;
using Simulation.LocationStrategies.Auctions;
using Simulation.DataCenter.Machines;

namespace Simulation.Modules.Management.Master.Proposed
{
    //public enum MasterCurrentAction
    //{
    //    None,
    //    PullAuction,
    //    PushAuction
    //}

    public class MasterState
    {
        //public MasterCurrentAction CurrentAction { get; set; } = MasterCurrentAction.None;
        public int CurrentHost { get; set; } = 0;
        public Auction Auction { get; set; } = null;

    }

    public class ProposedMasterHandler:MasterHandlerModule
    {
        protected IMachinePowerController PowerController { get; set; }
        protected UtilizationTable DataHolder { get; set; }
        private readonly MasterState _masterState = new MasterState();
        private readonly TestedHosts TestedHostsCount;
        public ProposedMasterHandler(NetworkInterfaceCard communicationModule,IMachinePowerController powerController, UtilizationTable dataHolder, TestedHosts testedHosts) : base(communicationModule)
        {
            PowerController = powerController;
            DataHolder = dataHolder;
            TestedHostsCount = testedHosts;
        }

        public override void HandleMessage(Message message)
        {
            if (message.MessageType == MessageTypes.UtilizationStateChange)
            {
                HandleUtilizationStateChange(message as HostStateChange);
                return;
            }
            
            lock (MasterLock)
            {
                switch (message.MessageType)
                {
                    case MessageTypes.PushRequest:
                        HandleRequest(message as Request);
                        break;
                    case MessageTypes.PullRequest:
                        HandleRequest(message as Request);
                        break;
                    case MessageTypes.LoadAvailabilityResponse:
                        HandleLoadAvailabilityResponce(message as LoadAvailabilityResponce);
                        break;
                    case MessageTypes.EvacuationDone:
                        HandleEvacuationDone(message as EvacuationDone);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        //Dictionary<int, DateTime> evacuating = new Dictionary<int, DateTime>();

        private void HandleUtilizationStateChange(HostStateChange message)
        {
            if (DataHolder.GetUtilization(message.SenderId) == UtilizationStates.Evacuating)
            {
                //var time = DateTime.Now;
                //var was = evacuating[message.SenderId];
              //  throw new Exception("Should be impossible");
            }
            else
                DataHolder.SetUtilization(message.SenderId, message.State);

        }

        

        private void HandleRequest(Request message)
        {
            if (_masterState.CurrentHost == 0)
            {
                if (message.MessageType == MessageTypes.PushRequest)
                {
                    HandlePushRequest(message as PushRequest);
                }
                else if (message.MessageType == MessageTypes.PullRequest)
                {
                    HandlePullRequest(message as PullRequest);
                }
                else
                {
                    throw new Exception("Exception");
                }
            }
            else
            {
                StrategyActionType atype;
                if (message.MessageType == MessageTypes.PushRequest)
                    atype = StrategyActionType.PushAction;
                else if (message.MessageType == MessageTypes.PullRequest)
                    atype = StrategyActionType.PullAction;
                else
                {
                    throw new NotImplementedException();
                }
                RejectRequest request = new RejectRequest(message.SenderId, this.MachineId, atype, RejectActions.Busy);
                CommunicationModule.SendMessage(request);
            }
        }

        private void HandlePushRequest(PushRequest message)
        {
            var candidates = DataHolder.GetCandidateHosts(UtilizationStates.UnderUtilization, message.SenderId);
            if (!candidates.Any())
            {
                candidates = DataHolder.GetCandidateHosts(UtilizationStates.Normal, message.SenderId);
            }
            if (!candidates.Any())
            {
                InitiateRejectAction(message.SenderId, StrategyActionType.PushAction);
            }
            else
            {
                HandlePushRequest(message, candidates);
            }
        }

        private void HandlePushRequest(PushRequest message, List<int> candidates)
        {
            var ncandidates = candidates.Take((int)TestedHostsCount).ToList();
            int instanceId = Helpers.RandomNumberGenerator.GetInstanceRandomNumber();
            PushAuction pushAuction = new PushAuction(instanceId, message.SenderId, message.SelectedContainerLoadInfo.ContainerId, ncandidates);
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
            if (_masterState.Auction != null)
                throw new NotImplementedException();
            //Auctions.Add(pushAuction);
            //_masterState.CurrentAction = MasterCurrentAction.PushAuction;
            _masterState.CurrentHost = message.SenderId;
            _masterState.Auction = pushAuction;
        }

        private void HandlePullRequest(PullRequest message)
        {
            var candidates = DataHolder.GetCandidateHosts(UtilizationStates.OverUtilization, message.SenderId);

            //if (!candidates.Any())
            //{
            //    candidates = DataHolder.GetCandidateHosts(UtilizationStates.UnderUtilization, message.SenderId);
            //}

            if (!candidates.Any())
            {
                candidates = DataHolder.GetCandidateHosts(UtilizationStates.Normal, message.SenderId);
            }
            if (!candidates.Any())
            {
                InitiateRejectAction(message.SenderId, StrategyActionType.PullAction);
            }
            else
            {
                HandlePullRequest(message, candidates);
            }
        }

        private void HandlePullRequest(PullRequest message, List<int> candidates)
        {
            var ncandidates = candidates.Take((int)TestedHostsCount).ToList();

            int instanceId = RandomNumberGenerator.GetInstanceRandomNumber();
            // int count = candidates.Count();
            PullAuction pullAuction = new PullAuction(instanceId, message.SenderId, ncandidates);

            foreach (var candidateHostId in ncandidates)
            {
                if (candidateHostId == 0)
                {
                    throw new NotImplementedException();
                }
                PullLoadAvailabilityRequest request = new PullLoadAvailabilityRequest(candidateHostId, this.MachineId, instanceId);
                CommunicationModule.SendMessage(request);
            }
            if (_masterState.Auction != null)
            {
                throw new NotImplementedException();
            }
            //Auctions.Add(pushAuction);
            //_masterState.CurrentAction = MasterCurrentAction.PullAuction;
            _masterState.CurrentHost = message.SenderId;
            _masterState.Auction = pullAuction;
        }

        public  void HandleLoadAvailabilityResponce(LoadAvailabilityResponce message)
        {
            _masterState.Auction.AddBid(message.HostBid);
            if (!_masterState.Auction.OpenSession)
            {
                var winner = _masterState.Auction.GetWinnerBid();
                if (winner == null)
                {
                    //Reject the request of the requester
                    InitiateRejectAction(_masterState.Auction.Owner, _masterState.Auction.ActionType);
                }
                else
                {
                    //Here The Difference Exist
                    if (_masterState.Auction.ActionType == StrategyActionType.PushAction)
                    {
                        InitiateMigration(_masterState.Auction.Owner, winner.BiddingHost, winner.ContainerId);
                    }
                    else
                    {
                        InitiateMigration(winner.BiddingHost, _masterState.Auction.Owner, winner.ContainerId);
                    }
                }
                //Cancel All Bids
                foreach (var candidate in _masterState.Auction.GetAllCandidates())
                {
                    if (winner == null || candidate != winner.BiddingHost)
                    {
                        BidCancellationRequest request = new BidCancellationRequest(candidate, this.MachineId, _masterState.Auction.InstanceId);
                        CommunicationModule.SendMessage(request);
                    }
                }

                //_masterState.CurrentAction = MasterCurrentAction.None;
                _masterState.Auction = null;
                if (_masterState.CurrentHost == 0)
                {
                    throw new Exception("How come");
                }
                _masterState.CurrentHost = 0;
            }
            
        }

        /// <summary>
        /// Should Be Optimized for the reasons to add new host or remove one
        /// </summary>
        /// <param name="hostId"></param>
        /// <param name="atype"></param>
        protected void InitiateRejectAction(int hostId, StrategyActionType atype)
        {
            var action = RejectActions.Nothing;
            switch (atype)
            {
                case StrategyActionType.PushAction:
                    if(DataHolder.GetUtilization(hostId) == UtilizationStates.UnderUtilization
                        || DataHolder.GetUtilization(hostId) == UtilizationStates.Normal)
                    {
                        //throw new NotImplementedException("How come");
                        action = RejectActions.TestWalid;
                    }
                    else if (DataHolder.GetUtilization(hostId) == UtilizationStates.Evacuating)
                    {
                        action = RejectActions.CancelEvacuation;
                        //UpdateSate(hostId, UtilizationStates.UnderUtilization,"Master");
                        DataHolder.SetUtilization(hostId, UtilizationStates.UnderUtilization);
                    }
                    else if(DataHolder.GetCandidateHosts(UtilizationStates.UnderUtilization,hostId).Count==0
                        && DataHolder.GetCandidateHosts(UtilizationStates.Evacuating,hostId).Count == 0)
                    {
                        PowerController.PowerOnHost();
                    }
                    else
                    {
                        //Should I cancell all evacuations
                    }
                    break;
                case StrategyActionType.PullAction:
                    //Evacuate Host

                    if (DataHolder.GetCandidateHosts(UtilizationStates.OverUtilization, hostId).Count==0)
                    {
                        action = RejectActions.Evacuate;
                        //UpdateSate(hostId, UtilizationStates.Evacuating, "Master");
                        DataHolder.SetUtilization(hostId, UtilizationStates.Evacuating);
                        //evacuating.Add(hostId, DateTime.Now);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            RejectRequest request = new RejectRequest(hostId, this.MachineId, atype, action);
            CommunicationModule.SendMessage(request);
        }

        private void InitiateMigration(int source, int target, int containerId)
        {
            var initMigration = new InitiateMigration(this.MachineId, source, target, containerId);
            CommunicationModule.SendMessage(initMigration);
        }
        private void HandleEvacuationDone(EvacuationDone message)
        {
            if (DataHolder.GetUtilization(message.SenderId) != UtilizationStates.Evacuating)
            {
                //throw new NotImplementedException("Home come");

            }

            PowerController.PowerOffHost(message.SenderId);
            
            //EvacuatingHost = 0;
            //EvacuatingHosts.Remove(message.SenderId);

        }
    }
}
