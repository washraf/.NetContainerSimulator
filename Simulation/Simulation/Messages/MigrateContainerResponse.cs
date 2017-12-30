namespace Simulation.Messages
{
    public class MigrateContainerResponse : Message
    {
        public MigrateContainerResponse(int target, int sender, int containerId, bool done) :
            base(target, sender, MessageTypes.MigrateContainerResponse)
        {
            ContainerId = containerId;
            Done = done;
        }
        public int ContainerId { get; private set; }
        public bool Done { get; private set; }
    }
}
