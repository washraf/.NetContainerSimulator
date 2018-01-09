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
using Simulation.DataCenter.Core;

namespace Simulation.Modules.Management.Host
{
    public abstract class HostHandlerModule:IMessageHandler
    {
        protected readonly ContainerTable ContainerTable;
        protected readonly ILoadManager LoadManager;
        public NetworkInterfaceCard CommunicationModule { get; set; }
        protected int MachineId { get; set; }
        protected int FailuresCount { get; set; }
        protected int BackOff { get; } = Global.CheckRate;
        public bool Started { get; set; }

        public double MinUtilization { set; get; }
        public double MaxUtilization { set; get; }

        public HostHandlerModule(NetworkInterfaceCard communicationModule,ContainerTable containerTable,ILoadManager loadManager)
        {
            MachineId = communicationModule.MachineId;
            ContainerTable = containerTable;
            LoadManager = loadManager;
            CommunicationModule = communicationModule;
            
        }
        public abstract void HandleMessage(Message message);

        public Message HandleRequestData(Message message)
        {
            throw new NotImplementedException();
        }

        #region --long running--
        public abstract void MachineAction();
        #endregion

        #region  --Back Off Control --
        protected void ResetBackOff()
        {
            FailuresCount = 0;
        }

        protected void IncreaseBackOffTime()
        {
            FailuresCount++;
            if (FailuresCount >= 10)
            {
                FailuresCount = 0;
            }
        }

        protected int GetBackOffTime()
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());
            int range = Convert.ToInt32(Convert.ToInt32(Math.Pow(2, FailuresCount)-1)*0.5);
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
        #endregion
    }
}
