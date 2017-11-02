using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Simulation.Messages;

namespace Simulation.DataCenter
{
    public class NetworkInterfaceCard
    {
        public NetworkInterfaceCard(int machineId, NetworkSwitch networkSwitch, IMessageHandler handler)
        {
            MachineId = machineId;
            Handler = handler;
            NetworkSwitch = networkSwitch;
        }

        public int MachineId { get; private set; }
        public IMessageHandler Handler { get; set; }
        public NetworkSwitch NetworkSwitch;

        public bool Started { get; set; }

        public void SendMessage(Message message)
        {
            //if (message.SenderId == message.TargetId)
            //    throw new NotImplementedException();
            if (message.SenderId != this.MachineId)
                throw new NotImplementedException();
            NetworkSwitch.ReceiveMessage(message);
        }

        public bool ReceiveMessage(Message message)
        {
            if (!Started)
                return true;
            if (message.TargetId == this.MachineId || message.TargetId == -1)
            {
                Task t = new Task(() =>
                {
                    if (MachineId == 0)
                    {

                    }
                    Handler.HandleMessage(message);
                });
                t.Start();
                return true;
            }
            else
            {
                throw new NullReferenceException();
            }
        }
    }
}