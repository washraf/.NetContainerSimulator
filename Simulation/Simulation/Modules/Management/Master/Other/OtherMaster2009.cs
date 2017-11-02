using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.DataCenter;
using Simulation.Messages;

namespace Simulation.Modules.Management.Master.Other
{
    public class OtherMaster2009: MasterHandlerModule
    {
        public OtherMaster2009(NetworkInterfaceCard communicationModule) : base(communicationModule)
        {
        }

        public override void HandleMessage(Message message)
        {
            throw new NotImplementedException("There is no master here");
        }
    }
}
