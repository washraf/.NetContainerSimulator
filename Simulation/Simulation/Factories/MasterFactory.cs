using Simulation.DataCenter.Machines;
using Simulation.Configuration;
using Simulation.DataCenter.InformationModules;
using Simulation.DataCenter.Network;
using Simulation.LocationStrategies;

namespace Simulation.Factories
{
    public class MasterFactory: MachineFactory
    {
        private NetworkSwitch networkSwitchObject;
        private MachineController machineControllerObject;
        private UtilizationTable utilizationTable;
        private Strategies currentStrategy;
        private TestedHosts testedHostsCount;

        public MasterFactory(NetworkSwitch networkSwitchObject,
            MachineController machineControllerObject,
            UtilizationTable utilizationTable,
            Strategies currentStrategy, TestedHosts testedHostsCount):base(networkSwitchObject)
        {
            this.networkSwitchObject = networkSwitchObject;
            this.machineControllerObject = machineControllerObject;
            this.utilizationTable = utilizationTable;
            this.currentStrategy = currentStrategy;
            this.testedHostsCount = testedHostsCount;
        }

        public override Machine GetMachine()
        {
            return new MasterMachine(networkSwitchObject, machineControllerObject, utilizationTable,
                currentStrategy, testedHostsCount);
        }
    }
}
