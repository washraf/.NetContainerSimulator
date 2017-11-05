using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.DataCenter;

namespace Simulation.Messages
{
    public class MigrateContainerRequest : Message
    {
        public MigrateContainerRequest(int target, int sender, Container container,int messageSize) :
            base(target, sender, MessageTypes.MigrateContainerRequest,messageSize)
        {
            MigratedContainer = container;
        }

        public Container MigratedContainer { get; private set; }
    }

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
