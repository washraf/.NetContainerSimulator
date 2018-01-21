using Simulation.DataCenter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Messages;
using Simulation.DataCenter.Containers;
using Simulation.DataCenter.InformationModules;
using Simulation.DataCenter;
using Simulation.Loads;
using Simulation.Configuration;
using Simulation.DataCenter.Machines;

namespace Simulation.Modules.Scheduling
{
    public interface IScheduler:IMessageHandler
    {
        bool Started { get; set; }

        void ScheduleContainer(Container container);

    }

    public class FirstFitScheduler : IScheduler
    {
        private readonly NetworkInterfaceCard CommunicationModule;
        private readonly IMachinePowerController powerContoller;
        private bool busy;
        
        private Queue<Container> Containers = new Queue<Container>();
        private UtilizationTable Holder { get; }
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

        private readonly object _lock = new object();
        public FirstFitScheduler(UtilizationTable holder, NetworkInterfaceCard communicationModule, IMachinePowerController powerContoller)
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
                        if (!busy && Containers.Any())
                        {
                            busy = true;
                            AddContainer(Containers.Dequeue());
                        }
                    }
                }
            });
            t.Start();
        }
        Container currentContainer = null;
        Queue<int> candidates = null;
        private void AddContainer(Container container)
        {
            if (currentContainer != null)
                throw new NotImplementedException("How come");
            candidates = new Queue<int>( Holder.GetCandidateHosts(UtilizationStates.Normal, 0));
            var under = Holder.GetCandidateHosts(UtilizationStates.UnderUtilization, 0);
            foreach (var u in under)
            {
                candidates.Enqueue(u);
            }
            if (candidates.Count > 0)
            {
                currentContainer = container;

                TestHostForAContainer();
            }
            else
            {

                FailedScheduling();
            }
            //busy = false;
        }
        private void FailedScheduling()
        {
            powerContoller.PowerOnHost();
            Containers.Enqueue(currentContainer);
            currentContainer = null;
            candidates = null;
            busy = false;
        }

        private void TestHostForAContainer()
        {
            var id = candidates.Dequeue();
            Message m = new CanHaveContainerRequest(id, 0, currentContainer.GetContainerPredictedLoadInfo());
            CommunicationModule.SendMessage(m);
        }

        public void ScheduleContainer(Container container)
        {
            lock (_lock)
            {
                Containers.Enqueue(container);
            }
        }

        public void HandleMessage(Message message)
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

        private void HandleCanHaveContainerResponce(CanHaveContainerResponce message)
        {
            if (message.ContainerId != currentContainer.ContainerId)
                throw new NotImplementedException("How come");
            if (message.Responce)
            {
                AddContainerRequest request = new AddContainerRequest(message.SenderId, 0, currentContainer);
                CommunicationModule.SendMessage(request);
                currentContainer = null;
                candidates = null;
                busy = false;
            }
            else
            {
                if (candidates.Count > 0)
                {
                    TestHostForAContainer();

                }
                else
                {
                    FailedScheduling();
                }
            }
            
        }

        public Message HandleRequestData(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
