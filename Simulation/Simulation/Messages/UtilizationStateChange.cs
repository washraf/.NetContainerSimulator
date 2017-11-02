using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Loads;

namespace Simulation.Messages
{
    public class VolumeStateChange : Message
    {

        public VolumeStateChange(int target, int sender,UtilizationStates state,double volume) :
            base(target, sender, MessageTypes.UtilizationStateChange)
        {
            State = state;
            Volume = volume;
        }

        public double Volume { get; private set; }
        public UtilizationStates State { get; private set; }

    }
}
