namespace Simulation.Messages
{
    public class InitiateMigration : Message
    {
        public InitiateMigration(int sender, int sourceHost, int targetHost, int containerId) 
            : base(sourceHost, sender, MessageTypes.InitiateMigration)
        {
            SourceHost = sourceHost;
            TargetHost = targetHost;
            ContainerId = containerId;
        }
        public int SourceHost { get; private set; }
        public int TargetHost { get; private set; }
        public int ContainerId { get; private set; }
    }

}