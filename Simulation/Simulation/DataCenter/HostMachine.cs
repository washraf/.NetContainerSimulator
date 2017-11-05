using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulation.Accounting;
using Simulation.Configuration;
using Simulation.DataCenter.InformationModules;
using Simulation.Helpers;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.Messages;
using Simulation.Modules.LoadManagement;
using Simulation.Modules.Management.Host;
using Simulation.Modules.Management.Host.Forsman2015;
using Simulation.Modules.Management.Host.Other;
using Simulation.Modules.Management.Host.WAshraf2017;

namespace Simulation.DataCenter
{
    public class HostMachine : Machine
    {
        private bool _started;
        private HostHandlerModule _handler;
        public override bool Started
        {
            get { return _started; }
            set
            {
                _started = value;
                CommunicationModule.Started = _started;
                _loadManager.Started = _started;
                _handler.Started = _started;
            }
        }

        #region --Parameters--
        private ContainerTable _containerTable;
        private ILoadManager _loadManager;
        #endregion
        public HostMachine(int id, Load maxLoad, NetworkSwitch networkSwitch, LoadPrediction currentLoadPrediction,Strategies strategy) : base(id, networkSwitch)
        {
            _containerTable = new ContainerTable(id);
            _loadManager = new HostLoadManager(this.MachineId, maxLoad, currentLoadPrediction, this.CommunicationModule, _containerTable);
            switch (strategy)
            {
                case Strategies.Auction:
                    _handler = new MyHostHandlerModule(CommunicationModule, _containerTable, _loadManager);

                    break;
                case Strategies.InOrderProping:
                    _handler = new MyHostHandlerModule(CommunicationModule, _containerTable, _loadManager);

                    break;
                case Strategies.Zhao:
                    _handler = new ZhaorHostHandler(CommunicationModule, _containerTable, _loadManager,Global.CommonLoadManager);

                    break;
                case Strategies.ForsmanPush:
                    _handler = new ForsmanHostHandler(CommunicationModule, _containerTable, _loadManager,StrategyActionType.PushAction);

                    break;
                case Strategies.ForsmanPull:
                    _handler = new ForsmanHostHandler(CommunicationModule, _containerTable, _loadManager,StrategyActionType.PullAction);

                    break;
                default:

                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
            }
        }

        #region --Long Running --

        public override void StartMachine()
        {
            if (MachineId == 0)
                throw new NotImplementedException();
            Started = true;
            _loadManager.Start();
            Thread hostThread = new Thread(_handler.MachineAction);
            hostThread.Start();
        }

        public override void StopMachine()
        {
            //lock (CommunicationLock)
            {
                foreach (var container in _containerTable.GetAllContainers())
                {
                    container.StopContainer();
                }
                Started = false;
            }
        }

        #endregion

        #region --Communication--

        public override void HandleMessage(Message message)
        {
            _handler.HandleMessage(message);
        }

        #endregion

        #region --Containers Control--

        public void ChangeAllContainersLoad(LoadChangeAction changeMethod, Func<Container, bool> condition)
        {
            //lock (_hostLock)
            {
                var containerslist = _containerTable.GetAllContainers().Where(condition);
                foreach (var container in containerslist)
                {
                    container.ChangeLoad(changeMethod);
                }
            }
        }

        public int GetContainersCount()
        {
            lock (CommunicationModule)
            {
                return _containerTable.GetContainersCount();
            }
        }

        public void AddContainer(Container container)
        {
            //lock (_hostLock)
            {
                _containerTable.AddContainer(container.ContainerId, container);
            }
        }

        public Dictionary<int, ContainerMeasureValue> CollectMigrationCounts()
        {
            //lock (_hostLock)
            {
                Dictionary<int, ContainerMeasureValue> conDictionary = new Dictionary<int, ContainerMeasureValue>();
                foreach (var container in _containerTable.GetAllContainers())
                {
                    conDictionary.Add(container.ContainerId, new ContainerMeasureValue(container.ContainerId, container.MigrationCount, container.DownTime));
                }
                return conDictionary;
            }
        }

        public int CalculateSlaViolations()
        {
            return _loadManager.CalculateSlaViolations();
        }

        #endregion

        #region --Load Calculation--

        public HostLoadInfo GetNeededHostLoadInfo()
        {
            return _loadManager.GetNeededHostLoadInfo();
        }

        public HostLoadInfo GetPredictedHostLoadInfo()
        {
            return _loadManager.GetPredictedHostLoadInfo();
        }

        public double MinUtilization
        {
            get { return _handler.MinUtilization; }
        }
        public double MaxUtilization
        {
            get { return _handler.MaxUtilization; }
        }

        #endregion
    }
}