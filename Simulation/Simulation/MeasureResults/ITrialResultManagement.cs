using Simulation.Measure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.MeasureResults
{
    public interface ITrialResultManagement
    {
        void Save(MeasureValueHolder holder);

    }
}
