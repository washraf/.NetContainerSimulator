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
            }
        }

        public UtilizationTable Holder { get; }

        public MasterMachine(NetworkSwitch networkSwitch, IMachinePowerController powerController,
            UtilizationTable holder, Strategies strategy, TestedHosts testedHosts) 
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

                    _handler = new ProposedMasterHandler(CommunicationModule,powerController,holder,testedHosts);
                    break;
                default:

                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
            }

            Holder = holder;
            //StartMachine();
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
            _handler.HandleMessage(message);
        }

        public override Message HandleRequestData(Message message)
        {
            throw new NotImplementedException();
        }

        public void AddContainer(Container container)
        {
            var candidates = Holder.GetCandidateHosts(UtilizationStates.Normal, 0);
            if (candidates.Count > 0)
            {
                var id = candidates.First();
                Message m = new AddContainerMessage(id,0,container);
                CommunicationModule.SendMessage(m);
            }
            else
            {
                //throw new NotImplementedException("Full Case");
            }
        }
        #endregion
    }
}
