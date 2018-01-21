using Simulation.DataCenter.Containers;

namespace Simulation.Messages
{
    public class AddContainerRequest : Message
    {
        public AddContainerRequest(int target, int sender, Container container) :
            base(target, sender, MessageTypes.AddContainerRequest)
        {
            NewContainer = container;
        }

        public Container NewContainer { get; private set; }
    }
}
