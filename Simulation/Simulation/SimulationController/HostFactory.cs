using Simulation.DataCenter.Machines;
using Simulation.Configuration;
using Simulation.DataCenter.Network;
using Simulation.LocationStrategies;
using Simulation.Loads;
using Simulation.Helpers;

namespace Simulation.SimulationController
{
    public class HostFactory : MachineFactory
    {
        private Load hostLoad;
        private LoadPrediction loadPrediction;
        private Strategies currentStrategy;
        private ContainersType containerTypes;

        public HostFactory(Load hostLoad, NetworkSwitch networkSwitchObject, LoadPrediction loadPrediction, Strategies currentStrategy, ContainersType containerTypes):base(networkSwitchObject)
        {
            
            this.hostLoad = hostLoad;
            _networkSwitchObject = networkSwitchObject;
            this.loadPrediction = loadPrediction;
            this.currentStrategy = currentStrategy;
            this.containerTypes = containerTypes;
        }


        public override Machine GetMachine()
        {
            return new HostMachine(RandomNumberGenerator.GetHostRandomNumber(), hostLoad, _networkSwitchObject, loadPrediction, currentStrategy, containerTypes);
        }
    }
}
