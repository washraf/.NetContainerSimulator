using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulation.Accounting;
using Simulation.Configuration;
using Simulation.DataCenter.InformationModules;
using Simulation.Messages;
using Simulation.DataCenter.Core;

namespace Simulation.DataCenter.Network
{
    public class NetworkSwitch : Component
    {

        public NetworkSwitch(ISwitchTable switchTable, IAccountingModule accountingModule)
        {
            _switchTable = switchTable;
            _accountingModule = accountingModule;
        }

        private readonly ISwitchTable _switchTable;
        private readonly IAccountingModule _accountingModule;
        public override bool Started { get; set; } = true;

        public bool ReceiveMessage(Message message)
        {
            if (Started)
            {
                Task t = new Task(async () =>
                {
                    {
                        if (_switchTable.ValidateMachineId(message.TargetId))
                        {
                            var nd = Global.GetNetworkDelay(message.MessageSize);
                            await Task.Delay(nd * Global.Second);
                            _accountingModule.RequestCreated(message.MessageType,message.MessageSize);
                            HandleMessage(message);

                        }
                        else
                        {
                        }
                    }
                });
                t.Start();
            }
            return true;
        }

        private void HandleMessage(Message message)
        {
            if (Started)
            {
                if (message == null)
                    throw new NullReferenceException();
                if (message.TargetId == -1)
                {
                    List<int> idList = _switchTable.GetAllMachineIds(message.SenderId);
                    foreach (var id in idList)
                    {
                        _switchTable.GetMachineById(id).CommunicationModule.ReceiveMessage(message);
                    }
                }
                else
                {
                    _switchTable.GetMachineById(message.TargetId).CommunicationModule.ReceiveMessage(message);
                }

            }
        }

        public async Task<Message> RequestData(Message message)
        {
            if(Started)
            {
                if (message == null)
                    throw new NullReferenceException();
                //Dangerous is commented
                var nd = Global.GetNetworkDelay(message.MessageSize);
                await Task.Delay(nd * Global.Second);
                _accountingModule.RequestCreated(message.MessageType,message.MessageSize);

                var machine = _switchTable.GetMachineById(message.TargetId);
                var result =  machine.CommunicationModule.HandleRequestData(message);
                nd = Global.GetNetworkDelay(message.MessageSize);
                await Task.Delay(nd * Global.Second);
                _accountingModule.RequestCreated(result.MessageType, result.MessageSize);
                return result;
            }
            return null;
        }
    }
}