using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Auctions;
using Simulation.Configuration;
using Simulation.DataCenter;
using Simulation.DataCenter.InformationModules;
using Simulation.Helpers;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.LocationStrategies.Auctions;
using Simulation.LocationStrategies.InOrderProping;
using Simulation.Messages;

namespace Simulation.Modules.Management.Master
{
    public abstract class MasterHandlerModule:IMessageHandler
    {
        protected NetworkInterfaceCard CommunicationModule { get; set; }
        protected int MachineId { set; get; }
        protected object MasterLock { get; set; }= new object();

        public MasterHandlerModule(NetworkInterfaceCard communicationModule)
        {
            CommunicationModule = communicationModule;
            this.MachineId = communicationModule.MachineId;
        }

        public abstract void HandleMessage(Message message);
    }
}
