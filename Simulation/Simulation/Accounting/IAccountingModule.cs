using Simulation.Measure;
using Simulation.Messages;

namespace Simulation.Accounting
{
    public interface IAccountingModule
    {
        //void StartCounting();
        void StopCounting();
        void ReadCurrentState();
        void RequestCreated(MessageTypes messageType, double messageSize);
        MeasureValueHolder MeasureHolder { get; }
        
    }
}