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
using Simulation.Modules.LoadManagement;

namespace Simulation.Modules.Management.Host
{
    public abstract class HostHandlerModule:IMessageHandler
    {
        protected readonly ContainerTable _containerTable;
        protected readonly ILoadManager _loadManager;
        public NetworkInterfaceCard CommunicationModule { get; set; }
        protected int MachineId;
        protected int _failuresCount;
        protected int BackOff { get; } = Global.CheckRate;
        public bool Started { get; set; }

        public double MinUtilization { set; get; }
        public double MaxUtilization { set; get; }

        public HostHandlerModule(NetworkInterfaceCard communicationModule,ContainerTable containerTable,ILoadManager loadManager)
        {
            MachineId = communicationModule.MachineId;
            _containerTable = containerTable;
            _loadManager = loadManager;
            CommunicationModule = communicationModule;
            
        }
        public abstract void HandleMessage(Message message);

        #region --long running--
        public abstract void MachineAction();
        protected abstract void TryToChangeSystemState(UtilizationStates hostState);
        protected abstract void SendPullRequest();
        protected abstract bool SendPushRequest();
        #endregion

        protected void ResetBackOff()
        {
            _failuresCount = 0;
        }

        protected void IncreaseBackOffTime()
        {
            _failuresCount++;
            if (_failuresCount >= 8)
            {
                _failuresCount = 0;
            }
        }

        protected int GetBackOffTime()
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());
            int range = Convert.ToInt32(Convert.ToInt32(Math.Pow(2, _failuresCount)-1)*0.5);
            var random = r.Next(0, range);
            var t  = (random + 1) * Global.CheckRate;
            if (random <= 23)
            return t;
            else
            {
                return Global.CheckRate*24;
            }
            //Random r = new Random();
            //var num = r.Next(-1 * Global.CheckRate / 2, Global.CheckRate / 2);
            //return BackOff + num;
        }
        protected ContainerLoadInfo GetToBeRemovedContainerLoadInfo()
        {
            var r = _containerTable.SelectContainerByCondition();
            if (r == null)
                return null;
            else
            {
                return r.GetContainerPredictedLoadInfo();
            }
        }
    }
}
