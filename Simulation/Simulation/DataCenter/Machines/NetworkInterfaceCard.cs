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

        public bool Started { get; set; }

        public void SendMessage(Message message)
        {
            //if (message.SenderId == message.TargetId)
            //    throw new NotImplementedException();
            if (message.SenderId != this.MachineId)
                throw new NotImplementedException();
            _networkSwitch.ReceiveMessage(message);
        }

        public bool ReceiveMessage(Message message)
        {
            if (!Started)
                return true;
            if (message.TargetId == this.MachineId || message.TargetId == -1)
            {
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

        public Message RequestData(Message message)
        {
            // if (!Started)
            //     return null;
            //Task<Message> t = new Task<Message>(() =>
            //{
                var result = _networkSwitch.RequestData(message);
                //if(result.MessageType == MessageTypes.ImagePullResponce)
                //{
                //    Thread.Sleep(Global.Second);
                //}
                return result;
            //});
            //t.Start();
            //t.Wait();
            //return t.Result;
        }

        public Message HandleRequestData(Message message)
        {
            return _handler.HandleRequestData(message);
        }
    }
}