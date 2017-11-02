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
using Simulation.LocationStrategies;
using Simulation.Messages;

namespace Simulation.Modules.Management.Host.Mine
{
    public class MyHostHandlerModule:HostHandlerModule
    {
        
        #region --Properties--
        private bool EvacuateMode { get; set; }

        private int BidLock { get; set; } = -1;
        private object _hostLock = new object();
        #endregion

        public MyHostHandlerModule(NetworkInterfaceCard communicationModule, ContainerTable containerTable, ILoadManager loadManager) : base(communicationModule, containerTable, loadManager)
        {
            MinUtilization = Global.MinUtilization;
            MaxUtilization = Global.MaxUtilization;
        }
        #region -- Long Running --
        public override void MachineAction()
        {
            //if (BidLock == -1 && !EvacuateMode)
            //{
                
            //}
            //else
            //{
            //    throw new NotImplementedException();
            //}
            BidLock = -1;
            EvacuateMode = false;
            //inital sleep before push or pull
            //Thread.Sleep(10*Global.Second);
            while (Started)
            {
                //change to time based on backoff algorithm 
                var bft = GetBackOffTime();
                //if (EvacuateMode)
                //{
                //    bft = 5*Global.Second;
                //}
                Thread.Sleep(bft);
                lock (_hostLock)
                {
                    if (BidLock == -1 && !EvacuateMode)
                    {
                        var s = _loadManager.CheckSystemState(true, MinUtilization, MaxUtilization); //can this be up
                        TryToChangeSystemState(s);
                    }
                    else if (EvacuateMode && BidLock == -1)
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
            //Console.WriteLine("Hahahahha");


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
            BidLock = 0;
            PullRequest pullRequest = new PullRequest(0, this.MachineId, _loadManager.GetPredictedHostLoadInfo());
            CommunicationModule.SendMessage(pullRequest);
        }
        protected override bool SendPushRequest()
        {
            BidLock = 0;
            var containerLoadInfo = GetToBeRemovedContainerLoadInfo();
            //Console.WriteLine($"I'm Host #{HostId} and I am Pushing container #{key} and I have {GetContainersCount()} contatiners");
            if (containerLoadInfo != null)
            {
                PushRequest m = new PushRequest(0, this.MachineId, _loadManager.GetPredictedHostLoadInfo(), containerLoadInfo);
                CommunicationModule.SendMessage(m);
                return true;
            }
            else
            {

                return false;
            }

        }
        #endregion

        #region --Communication --
        public override void HandleMessage(Message message)
        {
            lock (_hostLock)
            {
                //Console.WriteLine($"Host #{HostId}: You've got message from #{message.SenderId} and type = {message.MessageType}");
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
                    case MessageTypes.CancelEvacuation:
                        HandleCancelEvacuation(message as CancelEvacuation);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();

                }
            }
        }

        

        #endregion

        private void HandleBidCancellationRequest(BidCancellationRequest bidCancellationRequest)
        {
            if (bidCancellationRequest.AuctionId == BidLock)
            {
                //Console.WriteLine($"I am Host #{HostId} I am not bidding any more for AuctionId{bidCancellationRequest.AuctionId}");

                BidLock = -1;

            }
            else
            {

            }

        }
        private void HandleRejectRequest(RejectRequest message)
        {
            if (EvacuateMode)
            {
                
            }
            if (message.Auctiontype == StrategyActionType.PullAction)
            {
                IncreaseBackOffTime();
            }

            if (EvacuateMode && message.Auctiontype == StrategyActionType.PullAction)
            {
                //EvacuateMode = false;
                throw new NotImplementedException();
            }

            if (message.RejectAction == RejectActions.Evacuate)
            {
                EvacuateMode = true;
                //SendPushRequest();
            }
            if (!EvacuateMode && message.RejectAction == RejectActions.CancelEvacuation)
            {
                throw new NotImplementedException();
            }
            if (message.RejectAction == RejectActions.CancelEvacuation)
            {
                EvacuateMode = false;
                //Console.WriteLine("Cancel Evacuation");
            }
            BidLock = -1;

        }
        private void HandleInitiateMigration(InitiateMigration message)
        {
            //Console.WriteLine($" \n\n\n\n \t\t\t From Push with Love{message.ContainerId}");
            Task t = new Task(() =>
            {
                var container = _containerTable.GetContainerById(message.ContainerId);
                _containerTable.LockContainer(message.ContainerId);
                container.Checkpoint(this.MachineId);
                var size = (int)container.GetContainerNeededLoadInfo().CurrentLoad.MemorySize * 1024;
                MigrateContainerRequest request = new MigrateContainerRequest(message.TargetHost, this.MachineId,
                    container, size);
                CommunicationModule.SendMessage(request);
                ResetBackOff();
            });
            t.Start();
            //GlobalEnviromentVariables.ResetCheckRate();
        }
        private void HandleMigrateContainerRequest(MigrateContainerRequest message)
        {
            Task t = new Task(() =>
            {
                _containerTable.AddContainer(message.MigratedContainer.ContainerId, message.MigratedContainer);
                message.MigratedContainer.Restore(this.MachineId);
                var responce =
                    new MigrateContainerResponse(message.SenderId, this.MachineId, message.MigratedContainer.ContainerId,
                        true);
                CommunicationModule.SendMessage(responce);
                BidLock = -1;
                ResetBackOff();
            });
            t.Start();
        }
        private void HandleMigrateContainerResponce(MigrateContainerResponse message)
        {
            if (message.Done)
            {
                _containerTable.FreeLockedContainer();
                //_containersTable.Remove(sendContainerResponce.ContainerId);
            }
            else
            {
                _containerTable.UnLockContainer();
            }
            //Release Lock
            BidLock = -1;
        }

        #region --Push Message Handlers--
        private void HandlePushLoadAvailabilityRequest(PushLoadAvailabilityRequest message)
        {
            //Console.WriteLine($"Push : I'm Host #{HostId}: I've got message from # {message.SenderId}" +
            //                  $" for auction # {message.AuctionId}");
            //if (EvacuateMode && BidLock == -1)
            //{
            //    throw new Exception("Should take load");
            //}
            Bid bid;
            if (BidLock == -1 && !EvacuateMode && _loadManager.CanITakeLoad(message.NewContainerLoadInfo))
            {

                var load = _loadManager.GetHostLoadInfoAfterContainer(message.NewContainerLoadInfo);

                var newState = load.CalculateTotalUtilizationState(MinUtilization,MaxUtilization);
                if (newState == UtilizationStates.OverUtilization)
                {

                    bid = new Bid(MachineId, false, load, message.AuctionId,
                        message.NewContainerLoadInfo.ContainerId, BidReasons.FullLoad);
                }
                else
                {

                    bid = new Bid(MachineId, true, load, message.AuctionId, message.NewContainerLoadInfo.ContainerId,
                        BidReasons.None);
                    BidLock = bid.AuctionId;
                }
                //Console.WriteLine($"I am Host #{HostId} I am bidding for AuctionId {bid.AuctionId}");
            }
            else
            {
                //Console.WriteLine($"I am Host #{HostId} I am Not I'm not Bidlocked {BidLock} and PushLock {PushPullLock}");

                bid = new Bid(MachineId, false, null, message.AuctionId, message.NewContainerLoadInfo.ContainerId,
                    BidReasons.CantBid);
            }


            //Console.WriteLine($"I am Host #{HostId} bidding Push Auction {message.AuctionId} with {bid.Valid}");

            LoadAvailabilityResponce availabilityResponce =
                        new LoadAvailabilityResponce(message.SenderId, this.MachineId, message.AuctionId, bid);
            // var responce = new GetHostLoadInfoResponce(this.HostId, load);
            CommunicationModule.SendMessage(availabilityResponce);

        }
        #endregion

        #region --Pull Message Handlers
        private void HandlePullLoadAvailabilityRequest(PullLoadAvailabilityRequest message)
        {
            //Console.WriteLine($"Pull : I'm Host #{HostId}: I've got message from # {message.SenderId}" +
            //                 $" for auction # {message.AuctionId}");

            Bid bid;
            if (BidLock == -1 && !EvacuateMode)
            {
                ContainerLoadInfo selectedContainerload = GetToBeRemovedContainerLoadInfo();
                if (selectedContainerload != null)
                {
                    var oldstate = _loadManager.GetPredictedHostLoadInfo().CalculateTotalUtilizationState(MinUtilization,MaxUtilization);
                    var load = _loadManager.GetHostLoadInfoAWithoutContainer(selectedContainerload);
                    var newState = load.CalculateTotalUtilizationState(MinUtilization, MaxUtilization);
                    if (oldstate == UtilizationStates.Normal && newState == UtilizationStates.UnderUtilization)
                    {
                        bid = new Bid(MachineId, false, load, message.AuctionId, selectedContainerload.ContainerId, BidReasons.MinimumLoad);
                    }
                    else
                    {
                        var breason = BidReasons.None;
                        if (oldstate == UtilizationStates.UnderUtilization)
                        {
                            //if (_toBeEvacuated > 1)
                            //{
                            EvacuateMode = true;
                            breason = BidReasons.Evacuate;
                            //}
                            //else
                            //{
                            //    _toBeEvacuated++;
                            //}
                        }
                        //else
                        //{
                        //    _toBeEvacuated = 0;
                        //}
                        bid = new Bid(MachineId, true, load, message.AuctionId, selectedContainerload.ContainerId, breason);
                        BidLock = bid.AuctionId;

                    }

                    //Console.WriteLine($"I am Host #{HostId} I am bidding for AuctionId {bid.AuctionId}");
                }
                else
                {
                    bid = new Bid(MachineId, false, null, message.AuctionId, -1, BidReasons.Empty);
                }

            }
            else
            {
                //Console.WriteLine($"I am Host #{HostId} I am Not I'm not Bidlocked {BidLock} and PushLock {PushPullLock}");

                bid = new Bid(MachineId, false, null, message.AuctionId, -1, BidReasons.CantBid);
            }

            //Console.WriteLine($"I am Host #{HostId} for Pull Auction {message.AuctionId} with {bid.Valid}");

            LoadAvailabilityResponce availabilityResponce =
                        new LoadAvailabilityResponce(message.SenderId, this.MachineId, message.AuctionId, bid);
            // var responce = new GetHostLoadInfoResponce(this.HostId, load);
            CommunicationModule.SendMessage(availabilityResponce);
        }
        #endregion
        private void HandleCancelEvacuation(CancelEvacuation message)
        {
            if (!EvacuateMode)
            {
                throw new NotImplementedException("Cancel what wasn't active");
            }
            
            EvacuateMode = true;
        }

    }
}
