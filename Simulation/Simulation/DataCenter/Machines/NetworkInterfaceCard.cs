using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Simulation.Messages;
using Simulation.DataCenter.Network;
using Simulation.DataCenter.Core;
using System.Threading;
using Simulation.Configuration;

namespace Simulation.DataCenter
{
    public class NetworkInterfaceCard
    {
        public NetworkInterfaceCard(int machineId, NetworkSwitch networkSwitch, IMessageHandler handler)
        {
            MachineId = machineId;
            _handler = handler;
            _networkSwitch = networkSwitch;
        }

        public int MachineId { get; private set; }
        private readonly IMessageHandler _handler;
        private readonly NetworkSwitch _networkSwitch;
        public double DataSizeOut { get; private set; } = 0;
        public double DataSizeIn { get; private set; } = 0;
        public bool Started { get; set; }

        public void SendMessage(Message message)
        {
            if (message.SenderId != this.MachineId)
                throw new NotImplementedException();
            DataSizeOut += message.MessageSize;
            _networkSwitch.ReceiveMessage(message);
        }

        public bool ReceiveMessage(Message message)
        {
            if (!Started)
                return true;
            if (message.TargetId == this.MachineId || message.TargetId == -1)
            {
                DataSizeIn += message.MessageSize;
                Task t = new Task(() =>
                {
                    //if(message.MessageType == MessageTypes.MigrateContainerRequest)
                    //    Thread.Sleep(2 * Global.Second);
                    _handler.HandleMessage(message);
                });
                t.Start();
                return true;
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        public async Task <Message> RequestData(Message message)
        {
            DataSizeOut += message.MessageSize;
            var result = await _networkSwitch.RequestData(message);
            DataSizeIn += result.MessageSize;
            return result;
        }

        public Message HandleRequestData(Message message)
        {
            DataSizeIn += message.MessageSize;
            var result =  _handler.HandleRequestData(message);
            DataSizeOut += result.MessageSize;
            return result;
        }


        public void ResetDataSize()
        {
            DataSizeIn = 0;
            DataSizeOut = 0;
        }
    }
}