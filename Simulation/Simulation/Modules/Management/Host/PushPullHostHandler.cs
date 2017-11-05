using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.DataCenter;
using Simulation.DataCenter.InformationModules;
using Simulation.Loads;

namespace Simulation.Modules.Management.Host
{
   public  abstract class PushPullHostHandler : HostHandlerModule
    {
        protected abstract void TryToChangeSystemState(UtilizationStates hostState);
        protected abstract void SendPullRequest();
        protected abstract bool SendPushRequest();

        protected PushPullHostHandler(NetworkInterfaceCard communicationModule, ContainerTable containerTable, ILoadManager loadManager)
            : base(communicationModule, containerTable, loadManager)
        {
        }
    }
}
