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
                Task t = new Task(() =>
                {
                    {
                        if (_switchTable.ValidateMachineId(message.TargetId))
                        {
                            _accountingModule.RequestCreated(message.MessageType);
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

        protected void HandleMessage(Message message)
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

        public Message RequestData(Message message)
        {
            if(Started)
            {
                if (message == null)
                    throw new NullReferenceException();
                _accountingModule.RequestCreated(message.MessageType);
                var machine = _switchTable.GetMachineById(message.TargetId);
                return machine.CommunicationModule.HandleRequestData(message);
            }
            return null;
        }
    }
}