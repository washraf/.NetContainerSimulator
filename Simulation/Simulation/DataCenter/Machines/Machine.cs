using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Messages;
using Simulation.DataCenter.Network;
using Simulation.DataCenter.Core;

namespace Simulation.DataCenter.Machines
{
    public abstract class Machine : Component,IMessageHandler
    {
        public int MachineId { get; private set; }
        public NetworkInterfaceCard CommunicationModule;
        protected Machine(int machineId, NetworkSwitch networkSwitch)
        {
            MachineId = machineId;
            CommunicationModule = new NetworkInterfaceCard(this.MachineId, networkSwitch, this);
        }
        public abstract void StartMachine();
        public abstract void StopMachine();
        public abstract void HandleMessage(Message message);
        public abstract Message HandleRequestData(Message message);
    }


}

