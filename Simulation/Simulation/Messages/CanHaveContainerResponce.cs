namespace Simulation.Messages
{
    public class CanHaveContainerResponce : Message
    {
        public CanHaveContainerResponce(int target, int sender, int containerId, bool responce) :
            base(target, sender, MessageTypes.CanHaveContainerResponce)
        {
            ContainerId = containerId;
            Responce = responce;
        }

        public int ContainerId { get; }
        public bool Responce { get; }
    }
}
