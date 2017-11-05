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

namespace Simulation.Modules.Management.Host.WAshraf2017
{
    public class MyHostHandlerModule:PushPullHostHandler
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
            
            BidLock = -1;
            EvacuateMode = false;
            //inital sleep before push or pull
            //Thread.Sleep(10*Global.Second);
            while (Started)
            {
                //change to time based on backoff algorithm 
                var bft = GetBackOffTime();
                Thread.Sleep(bft);
                lock (_hostLock)
                {
                    if (BidLock == -1 && !EvacuateMode)
                    {
                        var s = LoadManager.CheckSystemState(true, MinUtilization, MaxUtilization); //can this be up
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

        /// <summary>
        /// Either send a push or pull request
        /// </summary>
        /// <param name="hostState"></param>
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

        protected override void SendPullRequest()
        {
            Console.WriteLine($"I'm Host #{MachineId} and I am pulling a container and I have {ContainerTable.GetContainersCount()} contatiners");

            BidLock = 0;
            PullRequest pullRequest = new PullRequest(0, this.MachineId, LoadManager.GetPredictedHostLoadInfo());
            CommunicationModule.SendMessage(pullRequest);
        }
        protected override bool SendPushRequest()
        {
            BidLock = 0;
            var containerLoadInfo = GetToBeRemovedContainerLoadInfo();
            Console.WriteLine($"I'm Host #{MachineId} and I am Pushing container #{containerLoadInfo} and I have {ContainerTable.GetContainersCount()} contatiners");
            if (containerLoadInfo != null)
            {
                PushRequest m = new PushRequest(0, this.MachineId, LoadManager.GetPredictedHostLoadInfo(), containerLoadInfo);
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
            Console.WriteLine($"Host #{MachineId} request was rejected");
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
                Console.WriteLine($"Cancel Evacuation By Rejection for Host# {MachineId}");
            }
            BidLock = -1;

        }
        private void HandleInitiateMigration(InitiateMigration message)
        {
            //Console.WriteLine($" \n\n\n\n \t\t\t From Push with Love{message.ContainerId}");
            Task t = new Task(() =>
            {
                var container = ContainerTable.GetContainerById(message.ContainerId);
                ContainerTable.LockContainer(message.ContainerId);
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
                ContainerTable.AddContainer(message.MigratedContainer.ContainerId, message.MigratedContainer);
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
                ContainerTable.FreeLockedContainer();
                //_containersTable.Remove(sendContainerResponce.ContainerId);
            }
            else
            {
                ContainerTable.UnLockContainer();
            }
            //Release Lock
            BidLock = -1;
        }

        #region --Push Message Handlers--
        private void HandlePushLoadAvailabilityRequest(PushLoadAvailabilityRequest message)
        {
            Console.WriteLine($"Push : I'm Host #{MachineId}: I've got message from # {message.SenderId}" +
                              $" for auction # {message.AuctionId}");
            Bid bid;
            if (BidLock == -1 && !EvacuateMode && LoadManager.CanITakeLoad(message.NewContainerLoadInfo))
            {

                var load = LoadManager.GetHostLoadInfoAfterContainer(message.NewContainerLoadInfo);

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
                Console.WriteLine($"I am Host #{MachineId} I am bidding for AuctionId {bid.AuctionId}");
            }
            else
            {
                Console.WriteLine($"I am Host #{MachineId} I am Not I'm not Bidlocked {BidLock}");
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
            Console.WriteLine($"Pull : I'm Host #{MachineId}: I've got message from # {message.SenderId}" +
                             $" for auction # {message.AuctionId}");

            Bid bid;
            if (BidLock == -1 && !EvacuateMode)
            {
                ContainerLoadInfo selectedContainerload = GetToBeRemovedContainerLoadInfo();
                if (selectedContainerload != null)
                {
                    var oldstate = LoadManager.GetPredictedHostLoadInfo().CalculateTotalUtilizationState(MinUtilization,MaxUtilization);
                    var load = LoadManager.GetHostLoadInfoAWithoutContainer(selectedContainerload);
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
                            EvacuateMode = true;
                            breason = BidReasons.Evacuate;
                        }

                        bid = new Bid(MachineId, true, load, message.AuctionId, selectedContainerload.ContainerId, breason);
                        BidLock = bid.AuctionId;

                    }

                    Console.WriteLine($"I am Host #{MachineId} I am bidding for AuctionId {bid.AuctionId}");
                }
                else
                {
                    bid = new Bid(MachineId, false, null, message.AuctionId, -1, BidReasons.Empty);
                }

            }
            else
            {
                Console.WriteLine($"I am Host #{MachineId} I am Not I'm not Bidlocked {BidLock}");

                bid = new Bid(MachineId, false, null, message.AuctionId, -1, BidReasons.CantBid);
            }

            Console.WriteLine($"I am Host #{MachineId} for Pull Auction {message.AuctionId} with {bid.Valid}");

            LoadAvailabilityResponce availabilityResponce =
                        new LoadAvailabilityResponce(message.SenderId, this.MachineId, message.AuctionId, bid);
            // var responce = new GetHostLoadInfoResponce(this.HostId, load);
            CommunicationModule.SendMessage(availabilityResponce);
        }
        #endregion

        private void HandleCancelEvacuation(CancelEvacuation message)
        {
            Console.WriteLine($"I Cancelling Evacuation I am Host #{MachineId} ");
            if (!EvacuateMode)
            {
                throw new NotImplementedException("Cancel what wasn't active");
            }
            
            EvacuateMode = true;
        }

    }
}
