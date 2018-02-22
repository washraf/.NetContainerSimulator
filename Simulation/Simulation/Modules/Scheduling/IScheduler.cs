using Simulation.DataCenter.Core;
using Simulation.DataCenter.Containers;

namespace Simulation.Modules.Scheduling
{
    public interface IScheduler:IMessageHandler
    {
        bool Started { get; set; }

        void ScheduleContainer(Container container);

    }
}
