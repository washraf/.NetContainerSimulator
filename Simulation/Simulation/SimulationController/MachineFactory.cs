using Simulation.DataCenter.Machines;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.DataCenter.Network;

namespace Simulation.SimulationController
{
    public abstract class MachineFactory
    {
        protected NetworkSwitch _networkSwitchObject;

        public MachineFactory(NetworkSwitch networkSwitch)
        {
            _networkSwitchObject = networkSwitch;
        }

        public abstract Machine GetMachine();
    }
}
