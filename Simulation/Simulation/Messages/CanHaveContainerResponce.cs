using Simulation.LocationStrategies;

namespace Simulation.Messages
{
    public class CanHaveContainerResponce : Message
    {
        public CanHaveContainerResponce(int target, int sender, Bid bid) :
            base(target, sender, MessageTypes.CanHaveContainerResponce)
        {
            Bid = bid;
        }

        public Bid Bid { get; }
    }
}
