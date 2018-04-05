using Simulation.Loads;

namespace Simulation.Messages.Forsman
{
    public class ForsmanPullRequest : Request
    {

        public ForsmanPullRequest(int target, int sender, HostLoadInfo hostLoad, int auctionId) : base(target, sender, hostLoad, MessageTypes.PullRequest)
        {
            AuctionId = auctionId;
        }

        public int AuctionId { get; set; }

    }
}
