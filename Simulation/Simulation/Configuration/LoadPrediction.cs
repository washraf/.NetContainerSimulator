using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Configuration
{
    public enum LoadPrediction
    {
        None,
        /// <summary>
        /// Not good
        /// </summary>
        Ewma,
        Arma,
        /// <summary>
        /// Liner Regression 
        /// </summary>
        LinReg
    }
}
