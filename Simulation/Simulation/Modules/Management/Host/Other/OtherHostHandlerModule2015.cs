using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulation.Configuration;
using Simulation.DataCenter;
using Simulation.DataCenter.InformationModules;
using Simulation.Helpers;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.LocationStrategies.Auctions;
using Simulation.LocationStrategies.Other2015;
using Simulation.Messages;
using Simulation.Messages.Other;

namespace Simulation.Modules.Management.Host.Other
{
    public class OtherHostHandlerModule2015:HostHandlerModule
    {
        private readonly StrategyActionType _currentActionType;
        public int BidLock { get; set; } = -1;
        private Auction2015 _currentAuction;
        private object _hostLock = new object();


        public OtherHostHandlerModule2015(NetworkInterfaceCard communicationModule, ContainerTable containerTable, ILoadManager loadManager, StrategyActionType currentActionType) : base(communicationModule, containerTable, loadManager)
        {
            _currentActionType = currentActionType;
            MaxUtilization = Global.OtherMaxUtilization;
            MinUtilization = Global.OtherMinUtilization;

        }

        #region --Long Running --

        public override void MachineAction()
        {
            Task t = new Task(async () =>
            {
                double lastDelay = 10;

                Random r = new Random();

                while (Started)
                {
                    await Task.Delay((int)lastDelay * Global.Second);

                    double d = 5+r.NextGaussian()*2;
                    d += lastDelay;
                    lastDelay = d;
                    if (lastDelay > 120)
                        lastDelay = 10;
                    if (_currentActionType == StrategyActionType.PushAction)
                    {
                        MaxUtilization *= 1 - (r.Next(1, 5)/100.0);
                    }
                    else
                    {
                        MinUtilization *= 1 + (r.Next(1, 5) / 100.0);
                    }
                    //if (MaxUtilization < 0.5)
                    //{
                    //    MaxUtilization = 0.7;
                    //    MinUtilization = 0.3;
                    //    //throw new NotImplementedException("Goal Arrived");
                    //}
                }
            });
            t.Start();
            //inital sleep before push or pull
            //Thread.Sleep(10*Global.Second);
            while (Started)
            {
                Thread.Sleep(GetBackOffTime());
                //lock (_hostLock)
                {
                    if (BidLock == -1)
                    {
                        var s = _loadManager.CheckSystemState(false, MinUtilization, MaxUtilization); //can this be up
                        TryToChangeSystemState(s);
                    }
                }
            }
            //var message = new EvacuationDone(0, this.MachineId);
            //CommunicationModule.SendMessage(message);
            //Console.WriteLine("Hahahahha");
        }
        protected override void TryToChangeSystemState(UtilizationStates hostState)
        {
            if (hostState == UtilizationStates.OverUtilization && _currentActionType == StrategyActionType.PushAction)
            {
                SendPushRequest();
            }
            else if (hostState == UtilizationStates.UnderUtilization && _currentActionType == StrategyActionType.PullAction)
            {
                SendPullRequest();
            }
        }
        protected override void SendPullRequest()
        {
            int total = (int)Global.SimulationSize - 1;
            var load = _loadManager.GetPredictedHostLoadInfo();
            _currentAuction = new PullAuction2015(load, total);
            BidLock = 0;
            OtherPullRequest m = new OtherPullRequest(-1, this.MachineId, load, this.MachineId);
            CommunicationModule.SendMessage(m);
        }
        protected override bool SendPushRequest()
        {
            var list = _containerTable.GetAllContainersLoadInfo();
                //.Where(x=>
                //_loadManager.GetHostLoadInfoAWithoutContainer(x)
                //.CalculateTotalUtilizationState(MinUtilization,MaxUtilization)!= UtilizationStates.UnderUtilization )
                //.ToList();
            int total = (int)Global.SimulationSize - 1;
            var load = _loadManager.GetPredictedHostLoadInfo();
            _currentAuction = new PushAuction2015(load, list, total);
            BidLock = 0;
            OtherPushRequest m = new OtherPushRequest(-1, this.MachineId, load, this.MachineId, list);
            CommunicationModule.SendMessage(m);
            Console.WriteLine($"Host No{MachineId} send Push Request");
            return false;
        }
        #endregion

        #region --Communication--
        public override void HandleMessage(Message message)
        {
            lock (_hostLock)
            {
                var mt = message.MessageType;
                switch (mt)
                {
                    case MessageTypes.PushRequest:
                        HandlePushRequest(message as OtherPushRequest);
                        break;
                        case MessageTypes.PullRequest:
                        HandlePullRequest(message as OtherPullRequest);
                        break;
                    case MessageTypes.LoadAvailabilityResponse:
                        HandleLoadAvailabilityResponse(message as OtherLoadAvailabilityResponce);
                        break;
                    case MessageTypes.WinnerAnnouncementMessage:
                        HandleWinnerAnnouncementMessage(message as WinnerAnnouncementMessage);
                        break;
                    case MessageTypes.MigrateContainerRequest:
                        HandleMigrateContainerRequest(message as MigrateContainerRequest);
                        break;
                    case MessageTypes.MigrateContainerResponse:
                        HandleMigrateContainerResponse(message as MigrateContainerResponse);
                        break;
                    case MessageTypes.InitiateMigration:
                        HandleInitiateMigrationRequest(message as InitiateMigration);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void HandleInitiateMigrationRequest(InitiateMigration message)
        {
            var con = _containerTable.GetContainerById(message.ContainerId);
            var size = (int)con.GetContainerNeededLoadInfo().CurrentLoad.MemorySize * 1024;
            _containerTable.LockContainer(con.ContainerId);
            con.Checkpoint(this.MachineId);
            MigrateContainerRequest request =
                new MigrateContainerRequest(message.SenderId, this.MachineId, con, size);
            CommunicationModule.SendMessage(request);
        }


        private void HandleMigrateContainerResponse(MigrateContainerResponse message)
        {
            if (message.Done)
            {
                _containerTable.FreeLockedContainer();
                //ResetBackOff();

                //_containersTable.Remove(sendContainerResponce.ContainerId);
            }
            else
            {
                _containerTable.UnLockContainer();
            }
            //Release Lock
            BidLock = -1;
        }
        private void HandleMigrateContainerRequest(MigrateContainerRequest message)
        {
            message.MigratedContainer.Restore(this.MachineId);
            _containerTable.AddContainer(message.MigratedContainer.ContainerId, message.MigratedContainer);
            var responce =
                new MigrateContainerResponse(message.SenderId, this.MachineId, message.MigratedContainer.ContainerId,
                    true);
            CommunicationModule.SendMessage(responce);
            BidLock = -1;
            //_lastDelay = 5;
            //ResetBackOff();
        }
        private void HandleWinnerAnnouncementMessage(WinnerAnnouncementMessage message)
        {
            if (message.WinnerHostId == this.MachineId)//I am the winner
            {
                //throw new NotImplementedException();
            }
            else // I am not the winner
            {
                if (BidLock == message.SenderId)
                {
                    BidLock = -1;
                }
            }
        }
        private void HandleLoadAvailabilityResponse(OtherLoadAvailabilityResponce message)
        {
            _currentAuction.EndWaitFor(message.SenderId,message.OldLoadInfo);
            if (message.Valid)
            {
                foreach (var bid in message.Bids)
                {
                    _currentAuction.AddBid(bid,MinUtilization,MaxUtilization);
                }
            }

            if (!_currentAuction.OpenSession)
            {
                var winner = _currentAuction.GetWinnerBid();
                if (winner != null)
                {
                    WinnerAnnouncementMessage responce = new WinnerAnnouncementMessage(-1, this.MachineId, winner.BiddingHost);
                    CommunicationModule.SendMessage(responce);
                    if (_currentActionType == StrategyActionType.PushAction)
                    {
                        var con = _containerTable.GetContainerById(winner.ContainerId);
                        var size = (int)con.GetContainerNeededLoadInfo().CurrentLoad.MemorySize * 1024;
                        _containerTable.LockContainer(con.ContainerId);
                        con.Checkpoint(this.MachineId);
                        MigrateContainerRequest request =
                            new MigrateContainerRequest(winner.BiddingHost, this.MachineId, con, size);
                        CommunicationModule.SendMessage(request);
                    }
                    else
                    {
                        InitiateMigration request = new InitiateMigration(this.MachineId,winner.BiddingHost,winner.BiddingHost,winner.ContainerId);
                        CommunicationModule.SendMessage(request);
                    }
                    ResetBackOff();
                    //_lastDelay = 5;
                }
                else //No winner
                {
                    WinnerAnnouncementMessage responce = new WinnerAnnouncementMessage(-1, this.MachineId, 0);
                    CommunicationModule.SendMessage(responce);
                    MaxUtilization = Global.OtherMaxUtilization;
                    MinUtilization = Global.OtherMinUtilization;
                    IncreaseBackOffTime();
                    BidLock = -1;
                    //_lastDelay = 5;
                }
                _currentAuction = null;
            }
        }
        private void HandlePushRequest(OtherPushRequest message)
        {
            OtherLoadAvailabilityResponce responce;

            if (BidLock == -1)
            {
                BidLock = message.SenderId;
                List<Bid> bids = new List<Bid>();
                var load = _loadManager.GetPredictedHostLoadInfo();

                foreach (var conload in message.ContainerLoads)
                {
                    var nload = _loadManager.GetHostLoadInfoAfterContainer(conload);
                    if (_loadManager.CanITakeLoad(conload))
                        //&& nload.CalculateTotalUtilizationState(MinUtilization,MaxUtilization)
                        //!=UtilizationStates.OverUtilization)
                    {
                        Bid bid = new Bid(MachineId, true, nload, message.AuctionId, conload.ContainerId,
                            BidReasons.None);
                        bids.Add(bid);
                    }
                }
                responce = new OtherLoadAvailabilityResponce(message.SenderId,this.MachineId, load, true,bids);

            }
            else
            {
                responce = new OtherLoadAvailabilityResponce(message.SenderId,this.MachineId,null,false,null);
                //Resources are locked in the first place   
            }
            CommunicationModule.SendMessage(responce);
        }
        private void HandlePullRequest(OtherPullRequest message)
        {
            OtherLoadAvailabilityResponce responce;

            if (BidLock == -1)
            {
                BidLock = message.SenderId;
                List<Bid> bids = new List<Bid>();
                var load = _loadManager.GetPredictedHostLoadInfo();

                foreach (var cont in _containerTable.GetAllContainers())
                {
                    var  conload = cont.GetContainerPredictedLoadInfo();
                    var nload = _loadManager.GetHostLoadInfoAWithoutContainer(conload);
                    if (_loadManager.CanITakeLoad(conload))
                        //&& nload.CalculateTotalUtilizationState(MinUtilization, MaxUtilization) != UtilizationStates.UnderUtilization)
                    {
                        Bid bid = new Bid(MachineId, true, nload, message.AuctionId, conload.ContainerId,
                            BidReasons.None,conload);
                        bids.Add(bid);
                    }
                }
                responce = new OtherLoadAvailabilityResponce(message.SenderId, this.MachineId, load, true, bids);
            }
            else
            {
                responce = new OtherLoadAvailabilityResponce(message.SenderId, this.MachineId, null, false, null);
                //Resources are locked in the first place   
            }
            CommunicationModule.SendMessage(responce);
        }
        #endregion   
    }
}
