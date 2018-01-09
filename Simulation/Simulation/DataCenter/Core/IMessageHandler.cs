using Simulation.Messages;

namespace Simulation.DataCenter.Core
{
    public interface IMessageHandler
    {
        /// <summary>
        /// Handle Asyncrounous  Message
        /// </summary>
        /// <param name="message">Message Object</param>
        void HandleMessage(Message message);
        /// <summary>
        /// Handel Syncrounous Message
        /// </summary>
        /// <param name="message">Sent  Message Object</param>
        /// <returns>Reply Message Object</returns>
        Message HandleRequestData(Message message);
    }
}