using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Messages.Other
{
    public class WinnerAnnouncementMessage:Message
    {
        public int WinnerHostId { get; private set; }
        //public int WinnerContainerId { get; private set; }

        public WinnerAnnouncementMessage(int target, int sender,int winnerHostId) : base(target, sender,MessageTypes.WinnerAnnouncementMessage)
        {
            WinnerHostId = winnerHostId;
            //WinnerContainerId = winnerContainerId;
        }
    }
}
