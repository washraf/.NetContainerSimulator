using Simulation.LocationStrategies;

namespace Simulation.Messages
{
    public enum RejectActions
    {
        Nothing,
        Busy,
        Evacuate,
        CancelEvacuation,
        TestWalid
    }
    public class RejectRequest : Message
    {

        public RejectRequest(int target, int sender, StrategyActionType type, RejectActions action) : base(target, sender, MessageTypes.RejectRequest)
        {
            Auctiontype = type;
            RejectAction = action;
        }

        public StrategyActionType Auctiontype { get; set; }
        public RejectActions RejectAction { get; private set; }

    }
}