using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Messages
{
    public class PullsCountRequest : Message
    {
        public PullsCountRequest(int target, int sender,int ImageId) : base(target, sender, MessageTypes.PullsCountRequest)
        {
            this.ImageId = ImageId;
        }

        public int ImageId { get; }
    }
    public class PullsCountResponce : Message
    {
        public PullsCountResponce(int target, int sender, int pullsCount) : base(target, sender, MessageTypes.PullsCountRequest)
        {
            this.PullsCount = pullsCount;
        }

        public int PullsCount { get; }
    }
}
