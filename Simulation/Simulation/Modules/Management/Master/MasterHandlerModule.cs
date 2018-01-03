using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Configuration;
using Simulation.DataCenter;
using Simulation.Messages;
using Simulation.DataCenter.Core;

namespace Simulation.Modules.Management.Master
{
    /// <summary>
    /// Master Message Handler Base Module
    /// </summary>
    public abstract class MasterHandlerModule:IMessageHandler
    {
        protected NetworkInterfaceCard CommunicationModule { get; set; }
        protected int MachineId { set; get; }
        protected object MasterLock { get; set; }= new object();

        protected MasterHandlerModule(NetworkInterfaceCard communicationModule)
        {
            CommunicationModule = communicationModule;
            this.MachineId = communicationModule.MachineId;
        }

        public abstract void HandleMessage(Message message);
        public Message HandleRequestData(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
