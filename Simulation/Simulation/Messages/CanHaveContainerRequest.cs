using Simulation.Loads;

namespace Simulation.Messages
{

    public class CanHaveContainerRequest : Message
    {
        public CanHaveContainerRequest(int target, int sender, ContainerLoadInfo container) :
            base(target, sender, MessageTypes.CanHaveContainerRequest)
        {
            NewContainerLoadInfo = container;
        }

        public ContainerLoadInfo NewContainerLoadInfo { get; private set; }
    }
}
