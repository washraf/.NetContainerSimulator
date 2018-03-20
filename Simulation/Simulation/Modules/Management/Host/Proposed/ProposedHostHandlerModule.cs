using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulation.Configuration;
using Simulation.DataCenter;
using Simulation.DataCenter.InformationModules;
using Simulation.Loads;
using Simulation.Messages;
using Simulation.LocationStrategies;

namespace Simulation.Modules.Management.Host.Proposed
{
    public enum HostCurrentAction
    {
        None,
        Pulling,
        Pushing,
       // Evacuating,
        Bidding
    }

    public class HostState
    {
        public HostCurrentAction CurrentAction { get; set; } = HostCurrentAction.None;
        public int AuctionId { get; set; }
        private bool evacuationMode = false;
        public bool EvacuationMode
        {
            get { return evacuationMode; }
            set
            {
                if(!value)
                {

                }
                evacuationMode = value;
            }
        }

    }
    public class ProposedHostHandlerModule:PushPullHostHandler
    {

        private readonly object _hostLock = new object();
        private readonly HostState _hostState = new HostState();

        public ProposedHostHandlerModule(NetworkInterfaceCard communicationModule, ContainerTable containerTable, ILoadManager loadManager) : base(communicationModule, containerTable, loadManager)
        {
            MinUtilization = Global.MinUtilization;
            MaxUtilization = Global.MaxUtilization;
           
        }

        public override void MachineAction()
        {
            _hostState.CurrentAction = HostCurrentAction.None;
            _hostState.EvacuationMode = false;
            _hostState.AuctionId = 0;
            while (Started)
            {
                //change to time based on backoff algorithm 
                var bft = GetBackOffTime();
                Thread.Sleep(bft);
                lock (_hostLock)
                {
                    var s = LoadManager.CheckSystemState(true, MinUtilization, MaxUtilization); //can this be up
                    if (_hostState.CurrentAction == HostCurrentAction.None && !_hostState.EvacuationMode)
                    {
                        TryToChangeSystemState(s);

                    }
                    else if (_hostState.EvacuationMode && _hostState.CurrentAction == HostCurrentAction.None)
                    {
                        if (!SendPushRequest())
                        {
                            break;
                        }
                    }
                }
            }
            var message = new EvacuationDone(0, this.MachineId);
            CommunicationModule.SendMessage(message);

            //lock (_hostLock)
            //{
                
            //    if (Started)
            //    {
            //        if (ContainerTable.GetContainersCount() > 0)
            //        {

            //        }
            //        var message = new EvacuationDone(0, this.MachineId);
            //        CommunicationModule.SendMessage(message);
            //    }
            //}
        }

        protected override void TryToChangeSystemState(UtilizationStates hostState)
        {
            if (hostState == UtilizationStates.OverUtilization)
            {
                SendPushRequest();
            }
            else if (hostState == UtilizationStates.UnderUtilization)
            {
                SendPullRequest();
            }
        }
        protected override void SendPullRequest()
        {
            _hostState.CurrentAction = HostCurrentAction.Pulling;
            PullRequest pullRequest = new PullRequest(0, this.MachineId, LoadManager.GetPredictedHostLoadInfo());
            CommunicationModule.SendMessage(pullRequest);
        }
        protected override bool SendPushRequest()
        {
            var containerLoadInfo = GetToBeRemovedContainerLoadInfo();
            if (containerLoadInfo != null)
            {
                _hostState.CurrentAction = HostCurrentAction.Pushing;
                PushRequest m = new PushRequest(0, this.MachineId, LoadManager.GetPredictedHostLoadInfo(), containerLoadInfo);
                CommunicationModule.SendMessage(m);
                return true;
            }
            else
            {

                return false;
            }
        }

        #region -- Communication --
        public override void HandleMessage(Message message)
        {
            //lock (_hostLock)
            {
                switch (message.MessageType)
                {
                    //Pull
                    case MessageTypes.PullLoadAvailabilityRequest:
                        HandlePullLoadAvailabilityRequest(message as PullLoadAvailabilityRequest);
                        break;
                    //Push
                    case MessageTypes.PushLoadAvailabilityRequest:
                        HandlePushLoadAvailabilityRequest(message as PushLoadAvailabilityRequest);
                        break;
                    //All
                    case MessageTypes.InitiateMigration:
                        HandleInitiateMigration(message as InitiateMigration);
                        break;
                    case MessageTypes.MigrateContainerRequest:
                        HandleMigrateContainerRequest(message as MigrateContainerRequest);
                        break;
                    case MessageTypes.MigrateContainerResponse:
                        HandleMigrateContainerResponce(message as MigrateContainerResponse);
                        break;
                    case MessageTypes.RejectRequest:
                        HandleRejectRequest(message as RejectRequest);
                        break;
                    case MessageTypes.BidCancellationRequest:
                        HandleBidCancellationRequest(message as BidCancellationRequest);
                        break;
                    case MessageTypes.CanHaveContainerRequest:
                        HandlaCanHaveContainerRequest(message as CanHaveContainerRequest);
                            break;
                    case MessageTypes.AddContainerRequest:
                        HanleAddContainerRequest(message as AddContainerRequest);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();

                }
            }
        }

        private void HanleAddContainerRequest(AddContainerRequest addContainerRequest)
        {
            Task t = new Task(() => {
                ContainerTable.AddContainer(addContainerRequest.NewContainer.ContainerId, addContainerRequest.NewContainer);
            });
            t.Start();
        }

        private void HandlaCanHaveContainerRequest(CanHaveContainerRequest message)
        {
            //ContainerTable.AddContainer(message.ScheduledContainer.ContainerId, message.ScheduledContainer);
            var load = LoadManager.GetHostLoadInfoAfterContainer(message.NewContainerLoadInfo);
            var newState = load.CalculateTotalUtilizationState(MinUtilization, MaxUtilization);
            Bid bid= null;
            if (!_hostState.EvacuationMode
                && LoadManager.CanITakeLoad(message.NewContainerLoadInfo) && newState!= UtilizationStates.OverUtilization) {
                 bid = new Bid(this.MachineId, true, load, message.AuctionId, message.NewContainerLoadInfo.ContainerId, BidReasons.ValidBid);

            }
            else
            {
                 bid = new Bid(this.MachineId, false, null, message.AuctionId, message.NewContainerLoadInfo.ContainerId, BidReasons.CantBid);

            }
            CanHaveContainerResponce responce = new CanHaveContainerResponce(0, MachineId, bid);
            CommunicationModule.SendMessage(responce);
        }

        #endregion
        #region --Push Message Handlers--
        private void HandlePushLoadAvailabilityRequest(PushLoadAvailabilityRequest message)
        {
            
            Bid bid;
            if (_hostState.CurrentAction==HostCurrentAction.None 
                && !_hostState.EvacuationMode 
                && LoadManager.CanITakeLoad(message.NewContainerLoadInfo))
            {

                var load = LoadManager.GetHostLoadInfoAfterContainer(message.NewContainerLoadInfo);

                var newState = load.CalculateTotalUtilizationState(MinUtilization, MaxUtilization);
                if (newState == UtilizationStates.OverUtilization)
                {

                    bid = new Bid(MachineId, false, load, message.AuctionId,
                        message.NewContainerLoadInfo.ContainerId, BidReasons.FullLoad);
                }
                else
                {
                    bid = new Bid(MachineId, true, load, message.AuctionId, message.NewContainerLoadInfo.ContainerId,
                        BidReasons.ValidBid);
                    _hostState.CurrentAction = HostCurrentAction.Bidding;
                    _hostState.AuctionId = message.AuctionId;
                }
            }
            else
            {
                bid = new Bid(MachineId, false, null, message.AuctionId, message.NewContainerLoadInfo.ContainerId,
                    BidReasons.CantBid);
            }
            LoadAvailabilityResponce availabilityResponce =
                        new LoadAvailabilityResponce(message.SenderId, this.MachineId, message.AuctionId, bid);
            CommunicationModule.SendMessage(availabilityResponce);
        }
        #endregion
        #region --Pull Message Handlers --
        private void HandlePullLoadAvailabilityRequest(PullLoadAvailabilityRequest message)
        {
            Bid bid;
            if (_hostState.CurrentAction == HostCurrentAction.None && !_hostState.EvacuationMode)
            {
                ContainerLoadInfo selectedContainerload = GetToBeRemovedContainerLoadInfo();
                if (selectedContainerload != null)
                {
                    var oldstate = LoadManager.GetPredictedHostLoadInfo().CalculateTotalUtilizationState(MinUtilization, MaxUtilization);
                    var load = LoadManager.GetHostLoadInfoAWithoutContainer(selectedContainerload);
                    var newState = load.CalculateTotalUtilizationState(MinUtilization, MaxUtilization);

                    if (oldstate == UtilizationStates.Normal && newState == UtilizationStates.UnderUtilization)
                    {
                        bid = new Bid(MachineId, false, load, message.AuctionId, -1, BidReasons.MinimumLoad);
                    }
                    else
                    {
                        bid = new Bid(MachineId, true, load, message.AuctionId, selectedContainerload.ContainerId, BidReasons.ValidBid);
                        _hostState.CurrentAction = HostCurrentAction.Bidding;
                        _hostState.AuctionId = message.AuctionId;
                    }

                }
                else
                {
                    bid = new Bid(MachineId, false, null, message.AuctionId, -1, BidReasons.Empty);
                }

            }
            else
            {
                bid = new Bid(MachineId, false, null, message.AuctionId, -1, BidReasons.CantBid);
            }


            LoadAvailabilityResponce availabilityResponce =
                        new LoadAvailabilityResponce(message.SenderId, this.MachineId, message.AuctionId, bid);
            CommunicationModule.SendMessage(availabilityResponce);
        }
        #endregion
        private void HandleRejectRequest(RejectRequest message)
        {
            switch (_hostState.CurrentAction)
            {
                case HostCurrentAction.None:
                    break;
                case HostCurrentAction.Pulling:
                    break;
                case HostCurrentAction.Pushing:
                    break;
                case HostCurrentAction.Bidding:
                    break;
            }
            switch (message.RejectAction)
            {
                case RejectActions.Nothing:
                    if (_hostState.EvacuationMode)
                    {
                        throw new NotImplementedException("how come");
                    }
                    break;
                case RejectActions.Busy:
                    break;
                case RejectActions.Evacuate:
                    if (_hostState.EvacuationMode)
                    {
                        throw new NotImplementedException("how come");
                    }
                    _hostState.EvacuationMode = true;
                    break;
                case RejectActions.CancelEvacuation:
                    if (!_hostState.EvacuationMode)
                    {
                        throw new NotImplementedException("how come");
                    }
                    _hostState.EvacuationMode = false;
                    break;
                case RejectActions.TestWalid:
                    _hostState.EvacuationMode = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("How come");
            }
            IncreaseBackOffTime();
            _hostState.CurrentAction = HostCurrentAction.None;

        }

        private void HandleBidCancellationRequest(BidCancellationRequest message)
        {
            //if (_hostState.CurrentAction != HostCurrentAction.Bidding)
            //    throw new NotImplementedException("How come");
            if (_hostState.AuctionId == message.AuctionId)
            {
                //Console.WriteLine($"I am Host #{HostId} I am not bidding any more for AuctionId{bidCancellationRequest.AuctionId}");
                _hostState.CurrentAction = HostCurrentAction.None;
                _hostState.AuctionId = 0;

            }
            else
            {
                //throw new NotImplementedException("How come");
            }

        }

        #region -- Migration Management --
        /*
         * Migration Sequence:
         * 1 - Initiate Migration Function
         * 2 - Tell Host To Load Images
         * 3 - Finish Loading Images
         * 4 - Start Migration
         * 5 - Handle MigrateContainerRequest
         * 6 - Handle Migrate Container Response
         */
        private void HandleInitiateMigration(InitiateMigration message)
        {
            Task t = new Task(() =>
            {
                var container = ContainerTable.GetContainerById(message.ContainerId);
                ContainerTable.LockContainer(message.ContainerId);
                container.Checkpoint(this.MachineId);
                var size = (int)container.GetContainerNeededLoadInfo().CurrentLoad.MemorySize * 1024;
                MigrateContainerRequest request = 
                new MigrateContainerRequest(message.TargetHost, this.MachineId, container, size);
                CommunicationModule.SendMessage(request);
                ResetBackOff();
            });
            t.Start();
            //GlobalEnviromentVariables.ResetCheckRate();
        }

        private void HandleMigrateContainerRequest(MigrateContainerRequest message)
        {
            Task t = new Task(async () =>
            {
                ContainerTable.AddContainer(message.MigratedContainer.ContainerId, message.MigratedContainer);
                await Task.Delay(Global.Second);
                message.MigratedContainer.Restore(this.MachineId);
                var responce =
                    new MigrateContainerResponse(message.SenderId, this.MachineId, message.MigratedContainer.ContainerId,
                        true);
                CommunicationModule.SendMessage(responce);
                _hostState.CurrentAction = HostCurrentAction.None;
                ResetBackOff();
            });
            t.Start();
        }
        private void HandleMigrateContainerResponce(MigrateContainerResponse message)
        {
            if (message.Done)
            {
                ContainerTable.FreeLockedContainer();
                //_containersTable.Remove(sendContainerResponce.ContainerId);
            }
            else
            {
                ContainerTable.UnLockContainer();
            }
            //Release Lock
            _hostState.CurrentAction = HostCurrentAction.None;
        }
        #endregion


        /// <summary>
        /// container selection by condition
        /// </summary>
        /// <returns></returns>
        protected ContainerLoadInfo GetToBeRemovedContainerLoadInfo()
        {
            var r = ContainerTable.SelectContainerByCondition();
            if (r == null)
                return null;
            else
            {
                return r.GetContainerPredictedLoadInfo();
            }
        }

    }
}