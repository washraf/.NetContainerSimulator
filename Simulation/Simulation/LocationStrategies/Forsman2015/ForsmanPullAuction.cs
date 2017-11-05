using System.Linq;
using Simulation.Helpers;
using Simulation.Loads;

namespace Simulation.LocationStrategies.Forsman2015
{
    public class ForsmanPullAuction : ForsmanAuction
    {
        public ForsmanPullAuction(HostLoadInfo predictedOwnerHostLoadInfo, int candidates) 
            : base(predictedOwnerHostLoadInfo, candidates)
        {
        }
        public override Bid GetWinnerBid()
        {
            if (Bids.Count == 0)
            {
                return null;
            }
            else
            {
                //Steps
                //1 : I only send push requests that are valid
                //2 : Find all containers migration costs average

                var avgCost =
                    Bids.Select(x => x.ContainerLoadInfo.MigrationCost).Average();
                var NewBids = Bids.Where(x => x.ContainerLoadInfo.MigrationCost >= avgCost).ToList();
                Bid winner = NewBids.First();
                var lds = ToBeHostLoadInfos(NewBids[0].BiddingHost, NewBids[0].NewLoadInfo);
                var bentropy = AccountingHelpers.CalculateEntropy(lds);
                //3: Find the new entropy after migration
                for (int i = 1; i < NewBids.Count; i++)
                {
                    lds = ToBeHostLoadInfos(NewBids[i].BiddingHost, NewBids[i].NewLoadInfo);
                    var entropy = AccountingHelpers.CalculateEntropy(lds);
                    if (entropy > bentropy)
                    {
                        bentropy = entropy;
                        winner = NewBids[i];
                    }
                }
                return winner;
            }
        }
    }
}