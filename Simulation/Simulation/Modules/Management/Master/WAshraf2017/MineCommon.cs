using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Configuration;
using Simulation.DataCenter;
using Simulation.DataCenter.InformationModules;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.Messages;

namespace Simulation.Modules.Management.Master.WAshraf2017
{
    public abstract class MineCommon : MasterHandlerModule
    {
        protected IMachinePowerController PowerController { get; set; }
        protected int Used { set; get; } = 0;
        private int _pushFailures = 0;
        /// <summary>
        /// This is a must to enable evacuation mode at multiple host
        /// Don't Change
        /// </summary>
        protected HashSet<int> EvacuatingHosts { get; set; } = new HashSet<int>();
        //protected int EvacuatingHost { get; set; } = 0;

        protected UtilizationTable DataHolder { get; set; }

        public MineCommon(NetworkInterfaceCard communicationModule, IMachinePowerController powerController, UtilizationTable dataHolder) : base(communicationModule)
        {
            PowerController = powerController;
            DataHolder = dataHolder;
        }

        #region --Message handlers--
        public override void HandleMessage(Message message)
        {
            if (message.MessageType == MessageTypes.UtilizationStateChange)
            {
                HandleUtilizationStateChange(message as HostStateChange);
                return;
            }
            lock (MasterLock)
            {
                var mt = message.MessageType;
                switch (mt)
                {
                    case MessageTypes.LoadAvailabilityResponse:
                        HandleLoadAvailabilityResponce(message as LoadAvailabilityResponce);
                        break;
                    case MessageTypes.PushRequest:
                        HandleRequest(message as Request);
                        break;
                    case MessageTypes.PullRequest:
                        HandleRequest(message as Request);
                        break;
                    case MessageTypes.EvacuationDone:
                        HandleEvacuationDone(message as EvacuationDone);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        private void HandleUtilizationStateChange(HostStateChange message)
        {
            DataHolder.SetUtilization(message.SenderId, message.State);
        }
        private void HandleEvacuationDone(EvacuationDone message)
        {
            PowerController.PowerOffHost(message.SenderId);
            //EvacuatingHost = 0;
            EvacuatingHosts.Remove(message.SenderId);

        }
        private void HandleRequest(Request message)
        {
            if (Used == 0)
            {
                //if (EvacuatingHost == 0
                //if (EvacuatingHosts.Count == 0|| 
                //    DataHolder.GetUtilization(message.SenderId) == UtilizationStates.OverUtilization)
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
                //else if (message.SenderId == EvacuatingHost)
                //else if (EvacuatingHosts.Contains(message.SenderId))
                //{
                //    if (message.MessageType == MessageTypes.PushRequest)
                //    {
                //        HandlePushRequest(message as PushRequest);
                //    }
                //    else
                //    {
                //        throw new Exception("Exception");
                //    }
                //}
                //else
                //{
                //    StrategyActionType atype;
                //    if (message.MessageType == MessageTypes.PushRequest)
                //        atype = StrategyActionType.PushAction;
                //    else if (message.MessageType == MessageTypes.PullRequest)
                //        atype = StrategyActionType.PullAction;
                //    else
                //    {
                //        throw new NotImplementedException();
                //    }
                //    RejectRequest request = new RejectRequest(message.SenderId, this.MachineId, atype, RejectActions.Nothing);
                //    CommunicationModule.SendMessage(request);
                //}
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
                RejectRequest request = new RejectRequest(message.SenderId, this.MachineId, atype, RejectActions.Nothing);
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
        private void HandlePullRequest(PullRequest message)
        {
            var candidates = DataHolder.GetCandidateHosts(UtilizationStates.OverUtilization, message.SenderId);

            if (!candidates.Any())
            {
                candidates = DataHolder.GetCandidateHosts(UtilizationStates.UnderUtilization, message.SenderId);
            }

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

        #endregion
        #region --Abstract--
        public abstract void HandleLoadAvailabilityResponce(LoadAvailabilityResponce message);
        protected abstract void HandlePushRequest(PushRequest message, List<int> candidates);
        protected abstract void HandlePullRequest(PullRequest message, List<int> candidates);

        #endregion
        #region --helpers--
        protected void InitiateMigration(int source, int target, int containerId)
        {
            //Console.WriteLine($"\n\n ------------------- Initiate Push from {source} to {target} on contianer {containerId} in auction Id = {auctionId}");
            //migratedIds.Add(containerId,source);

            var initMigration = new InitiateMigration(this.MachineId, source, target, containerId);
            CommunicationModule.SendMessage(initMigration);
            //Should Release All Locks
        }

        /// <summary>
        /// Should Be Optimized for the reasons to add new host or remove one
        /// </summary>
        /// <param name="hostId"></param>
        /// <param name="atype"></param>
        protected void InitiateRejectAction(int hostId, StrategyActionType atype)
        {
            //Implement Conditions for How to Add New Nodes or Remove 
            //Implement Add Host and Evacuation System Locks
            var action = RejectActions.Nothing;
            switch (atype)
            {
                case StrategyActionType.PushAction:
                    //Create New Host
                    //if (hostId != EvacuatingHost)
                    //if (EvacuatingHost == 0)
                    
                        if (EvacuatingHosts.Count == 0)
                        {
                        //if (_pushFailures > Global.GetNoOfFailures) 
                            if (!DataHolder.GetCandidateHosts(UtilizationStates.UnderUtilization, 0).Any())
                            {
                                PowerController.PowerOnHost();
                            }
                            //_pushFailures = 0;

                            //else
                            //{
                            //    //_pushFailures++;
                            //}
                        }
                        //else if (hostId == EvacuatingHost)
                        else if (EvacuatingHosts.Contains(hostId))
                        {
                            action = RejectActions.CancelEvacuation;
                            Console.WriteLine($"Cancel Evacuation for {hostId}");
                            //EvacuatingHost = 0;
                            EvacuatingHosts.Remove(hostId);
                            DataHolder.SetUtilization(hostId, UtilizationStates.UnderUtilization);
                        }
                        else
                        {
                            //throw new Exception("Evacuting Host with Push Request, What to Do");
                            //Options (Cancel Evacuation Mode @All Hosts)
                            foreach (var ehost in EvacuatingHosts)
                            {
                                DataHolder.SetUtilization(ehost, UtilizationStates.UnderUtilization);
                                var cem = new CancelEvacuation(ehost, this.MachineId);
                                CommunicationModule.SendMessage(cem);
                            }
                            EvacuatingHosts.Clear();
                        }
                    break;
                case StrategyActionType.PullAction:
                    //Evacuate Host
                    if (EvacuatingHosts.Contains(hostId))
                        throw new NotImplementedException();
                    if (//(EvacuatingHost == 0 && 
                        !DataHolder.GetCandidateHosts(UtilizationStates.OverUtilization, 0).Any())
                    {
                        DataHolder.SetUtilization(hostId, UtilizationStates.Evacuating);
                        //EvacuatingHost = hostId;
                        EvacuatingHosts.Add(hostId);
                        if (EvacuatingHosts.Count > 1)
                        {

                        }
                        action = RejectActions.Evacuate;
                        Console.WriteLine($"Start Evacuation for {hostId}");
                    }
                    //else if (hostId == EvacuatingHost)
                    //{
                    //    throw new NotImplementedException();
                    //}

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            RejectRequest request = new RejectRequest(hostId, this.MachineId, atype, action);
            CommunicationModule.SendMessage(request);
        }

        #endregion


    }
}
