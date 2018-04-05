using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simulation.Messages;
using Simulation.DataCenter.Containers;
using Simulation.DataCenter.InformationModules;
using Simulation.DataCenter;
using Simulation.Configuration;
using Simulation.DataCenter.Machines;
using System.Linq;

namespace Simulation.Modules.Scheduling
{
    public abstract class BaseScheduler : IScheduler
    {
        protected readonly NetworkInterfaceCard CommunicationModule;
        protected readonly IMachinePowerController powerContoller;
        protected Queue<Container> Containers = new Queue<Container>();
        protected UtilizationTable Holder { get; }
        protected readonly object _lock = new object();

        private bool _started;
        public bool Started
        {
            get
            {
                return _started;
            }
            set
            {
                _started = value;
                if (value)
                {
                    StartScheduling();
                }
            }
        }

        protected Container CurrentContainer { get; set; } = null;

        public BaseScheduler(UtilizationTable holder, NetworkInterfaceCard communicationModule, IMachinePowerController powerContoller)
        {
            Holder = holder;
            this.CommunicationModule = communicationModule;
            this.powerContoller = powerContoller;
        }
        private void StartScheduling()
        {
            Task t = new Task(async () =>
            {
                while (Started)
                {
                    //Sleep to avoid dos
                    await Task.Delay(Global.Second);
                    lock (_lock)
                    {
                        if ( CurrentContainer== null && Containers.Any())
                        {
                            AddContainer(Containers.Dequeue());
                        }
                    }
                }
            });
            t.Start();
        }

        protected abstract void AddContainer(Container container);

        public void ScheduleContainer(Container container)
        {
            lock (_lock)
            {
                Containers.Enqueue(container);
            }
        }

        public Message HandleRequestData(Message message)
        {
            throw new NotImplementedException();
        }

        public void HandleMessage(Message message)
        {
            lock (_lock)
            {
                switch (message.MessageType)
                {
                    case MessageTypes.CanHaveContainerResponce:
                        HandleCanHaveContainerResponce(message as CanHaveContainerResponce);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected abstract void HandleCanHaveContainerResponce(CanHaveContainerResponce canHaveContainerResponce);
    }
}
