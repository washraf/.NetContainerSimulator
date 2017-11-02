using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simulation.DataCenter;
using Simulation.DataCenter.InformationModules;
using Simulation.Loads;

namespace Simulation.DataCenter.InformationModules
{
    public interface ILoadManager : IStart
    {
        HostLoadInfo GetNeededHostLoadInfo();
        HostLoadInfo GetPredictedHostLoadInfo();
        HostLoadInfo GetHostLoadInfoAWithoutContainer(ContainerLoadInfo selectedContainerload);
        HostLoadInfo GetHostLoadInfoAfterContainer(ContainerLoadInfo newContainerLoadInfo);
        bool CanITakeLoad(ContainerLoadInfo newContainerLoadInfo);
        UtilizationStates CheckSystemState(bool act,double min,double max);
        void Start();
        int CalculateSlaViolations();
    }
}
