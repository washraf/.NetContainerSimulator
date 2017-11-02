using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Simulation.Accounting;
using Simulation.DataCenter.InformationModules;
using Simulation.Loads;

namespace Simulation.DataCenter.InformationModules
{
    public interface ISwitchTable
    {
        bool ValidateMachineId(int id);
        Machine GetMachineById(int id);
        List<int> GetAllMachineIds(int skip);
    }
}
