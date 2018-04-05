using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulation.Helpers;
using Simulation.Messages;
using Simulation.Configuration;
using Simulation.DataCenter.InformationModules;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.Modules.Management.Master;
using Simulation.Modules.Management.Master.Proposed;
using Simulation.Modules.Management.Master.WAshraf2017;
using Simulation.DataCenter.Network;
using Simulation.DataCenter.Containers;
using Simulation.Modules.Scheduling;

namespace Simulation.DataCenter.Machines
{
    public class MasterMachine : Machine
    {
        private bool _started;
        private readonly MasterHandlerModule _handler;
        public override bool Started
        {
            get { return _started; }
            set
            {
                _started = value;
                CommunicationModule.Started = value;
                scheduler.Started = value;
            }
        }

        public UtilizationTable Holder { get; }
        private IScheduler scheduler;
        public MasterMachine(NetworkSwitch networkSwitch, IMachinePowerController powerController,
            UtilizationTable holder, Strategies strategy,AuctionTypes pushAuctionType,AuctionTypes pullAuctionType,
            SchedulingAlgorithm scheduling,
            TestedHosts testedHosts) 
            : base(0, networkSwitch)
        {
            switch (strategy)
            {
                case Strategies.WAshraf2017Auction:
                    _handler = new AuctionManagement(CommunicationModule, powerController, holder, testedHosts);
                    break;
                case Strategies.WAshraf2017:
                    _handler = new InorderPropingManagement(CommunicationModule, powerController, holder);
                    break;
                case Strategies.Zhao:
                    _handler = new NoMasterHandlerModule(CommunicationModule);
                    break;
                case Strategies.ForsmanPush:
                    _handler = new NoMasterHandlerModule(CommunicationModule);
                    break;
                case Strategies.ForsmanPull:
                    _handler = new NoMasterHandlerModule(CommunicationModule);
                    break;
                case Strategies.Proposed2018:

                    _handler = new ProposedMasterHandler(CommunicationModule,powerController,holder,testedHosts,pushAuctionType,pullAuctionType);
                    break;
                default:

                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
            }

            Holder = holder;
            //StartMachine();
            switch (scheduling)
            {
                case SchedulingAlgorithm.FF:
                    scheduler = new FirstFitScheduler(holder, CommunicationModule, powerController);
                    break;
                case SchedulingAlgorithm.MFull:
                case SchedulingAlgorithm.LFull:
                    scheduler = new AuctionScheduler(holder, CommunicationModule, powerController,scheduling);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scheduling), scheduling, null);
            }
        }

        #region --Properties--
        public override sealed void StartMachine()
        {
            if (MachineId != 0)
            {
                throw new NotImplementedException();
            }
            Started = true;
            //Thread hostThread = new Thread(() =>
            //{
            //});
            //hostThread.Start();
        }
        public override void StopMachine()
        {
            //lock (CommunicationLock)
            {
                Started = false;
                //_requests.Clear();    
            }
        }

        #endregion

        #region --Message Handlers--
        public override void HandleMessage(Message message)
        {
            switch (message.MessageType)
            {
                case MessageTypes.CanHaveContainerResponce:
                    scheduler.HandleMessage(message);
                    break;
                default:
                    _handler.HandleMessage(message);
                    break;
            }
        }

        public override Message HandleRequestData(Message message)
        {
            throw new NotImplementedException();
        }

        public void AddContainer(Container container)
        {
            scheduler.ScheduleContainer(container);
        }
        #endregion
    }
}
