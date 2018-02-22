using Simulation.Loads;

namespace Simulation.Messages
{

    public class CanHaveContainerRequest : Message
    {

        public CanHaveContainerRequest(int target, int sender, int auctionId, ContainerLoadInfo container) :
            base(target, sender, MessageTypes.CanHaveContainerRequest)
        {
            AuctionId = auctionId;
            NewContainerLoadInfo = container;
        }

        public int AuctionId { get; }
        public ContainerLoadInfo NewContainerLoadInfo { get; private set; }
    }
}
