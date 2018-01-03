namespace Simulation.Messages
{
    public class BidCancellationRequest : Message
    {
        public BidCancellationRequest(int target, int sender, int auctionId) :
            base(target, sender, MessageTypes.BidCancellationRequest)
        {
            AuctionId = auctionId;
        }

        public int AuctionId { get; private set; }
    }

}