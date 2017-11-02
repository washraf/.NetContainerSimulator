using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Messages
{
    public class CancelEvacuation:Message
    {
        public CancelEvacuation(int target, int sender) : base(target, sender, MessageTypes.CancelEvacuation)
        {
        }
    }
}
