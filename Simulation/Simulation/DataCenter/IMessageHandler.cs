using Simulation.Messages;

namespace Simulation.DataCenter
{
    public interface IMessageHandler
    {
        void HandleMessage(Message message);
    }
}