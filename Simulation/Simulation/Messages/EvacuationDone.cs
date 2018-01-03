namespace Simulation.Messages
{
    public class EvacuationDone : Message
    {
        public EvacuationDone(int target, int sender) :
            base(target, sender, MessageTypes.EvacuationDone)
        {
        }
    }
}
