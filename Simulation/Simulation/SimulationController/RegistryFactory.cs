using Simulation.DataCenter.Machines;
using System;
using Simulation.DataCenter.Network;
using Simulation.LocationStrategies;
using Simulation.DataCenter.Images;
using Simulation.Configuration;

namespace Simulation.SimulationController
{
    public class RegistryFactory : MachineFactory
    {
        public RegistryFactory(NetworkSwitch networkSwitch, SimulationSize simulationSize) : base(networkSwitch)
        {
            SimulationSize = simulationSize;
        }

        public SimulationSize SimulationSize { get; }

        public override Machine GetMachine()
        {
            return new DockerRegistryMachine(int.MaxValue, _networkSwitchObject, SimulationSize);
        }
    }
}
