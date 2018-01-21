using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.DataCenter;
using Simulation.DataCenter.Containers;

namespace Simulation.Messages
{
    public class MigrateContainerRequest : Message
    {
        public MigrateContainerRequest(int target, int sender, Container container,int messageSize) :
            base(target, sender, MessageTypes.MigrateContainerRequest, messageSize)
        {
            MigratedContainer = container;
        }

        public Container MigratedContainer { get; private set; }
    }
}
