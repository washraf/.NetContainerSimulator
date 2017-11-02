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

namespace Simulation.DataCenter
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
                    //lock (CommunicationLock)
                    {
                        if (_switchTable.ValidateMachineId(message.TargetId))
                        {
                            _accountingModule.RequestCreated(message.MessageType);

                            //await Task.Delay(Global.GetNetworkDelay(message.MessageSize,
                            //    NetWorkSpeed.HundredG, SizeUnit.Byte));
                            HandleMessage(message);

                        }
                        else
                        {
                            //throw new NotImplementedException();
                            //if(message.MessageType==MessageTypes.RejectRequest||message.MessageType == MessageTypes.BidCancellationRequest)
                            //    return false;
                            //Must be re added
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
                    //throw new NotImplementedException("I don't broadcast");
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
    }
}