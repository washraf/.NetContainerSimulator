using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulation.Messages;

namespace Simulation.DataCenter
{
    public abstract class Component
    {
        //protected object CommunicationLock = new object();    
        public abstract bool Started { get; set; }
    }
}
