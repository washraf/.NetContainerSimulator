using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulation.Helpers;
using Simulation.Messages;
using Simulation.Auctions;
using Simulation.Configuration;
using Simulation.DataCenter.InformationModules;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.LocationStrategies.Auctions;
using Simulation.LocationStrategies.InOrderProping;
using Simulation.Modules.Management;
using Simulation.Modules.Management.Master;
using Simulation.Modules.Management.Master.Mine;
using Simulation.Modules.Management.Master.Other;

namespace Simulation.DataCenter
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
        public MasterMachine(NetworkSwitch networkSwitch, IMachinePowerController powerController, UtilizationTable holder,Strategies strategy) : base(0, networkSwitch)
        {
            switch (strategy)
            {
                case Strategies.Auction:
                    _handler = new AuctionManagement(CommunicationModule, powerController, holder);
                    break;
                case Strategies.InOrderProping:
                    _handler = new InorderPropingManagement(CommunicationModule, powerController, holder);
                    break;
                case Strategies.Zhao:
                    _handler = new OtherMaster2009(CommunicationModule);
                    break;
                case Strategies.ForsmanPush:
                    _handler = new OtherMaster2015(CommunicationModule);
                    break;
                case Strategies.ForsmanPull:
                    _handler = new OtherMaster2015(CommunicationModule);
                    break;

                default:

                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
            }
            StartMachine();
        }

        #region --Properties--
        public override sealed void StartMachine()
        {
            if (MachineId != 0)
            {
                throw new NotImplementedException();
            }
            Started = true;
            Thread hostThread = new Thread(() =>
            {
            });
            hostThread.Start();
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
        
        #endregion
    }
}
