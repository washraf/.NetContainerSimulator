using Simulation.Messages;

namespace Simulation.DataCenter.Core
{
    public interface IMessageHandler
    {
        void HandleMessage(Message message);
        Message HandleRequestData(Message message);
    }
}