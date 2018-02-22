using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.Modules.LoadManagement;

namespace Simulation.Configuration
{
    public static class Global
    {
        /// <summary>
        /// Each Step is ten seconds => 30 Min
        /// Ideal steps = 180
        /// </summary>
        public static int Steps = 180;
        public static int NoOfTrials { get; } = 3;

        public static int GetSimulationTime
        {
            get
            {
                //return 9000 * Global.Second;
                //return 21600 * Global.Second;
                //good with 50
                return 18000 * Global.Second;
            }
        }

        //for container source 5,5
        public static int LinerRegWindow = 10;
        public static int PredictionTime = 10;

        //best is 50
        public static int Second { get; private set; } = -1;

        public static void UpdateTime(SimulationSize simulationSize, Strategies strategy)
        {
            switch (simulationSize)
            {
                case SimulationSize.Twenty:
                    Second = strategy != Strategies.Zhao ? 5 : 10;
                    break;
                case SimulationSize.Fifty:
                    Second = strategy != Strategies.Zhao ? 10 : 20;
                    break;
                case SimulationSize.Hundred:
                    Second = strategy != Strategies.Zhao ? 20 : 40;
                    break;
                case SimulationSize.TwoHundred:
                    Second = strategy != Strategies.Zhao ? 30 : 60;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            AccountTime = 60 * Second;
            CheckRate = 20*Second;
        }

        static Global()
        {
            //Read From Test Configuration
        }

        public static LoadChangeStrategy LoadChangeStrategy { get; set; } = LoadChangeStrategy.Incremental;
        /// <summary>
        /// One Minute
        /// </summary>
        public static int AccountTime { get; private set; } = -1;
        

        //Test Default 0.7, 0.9
        //From energy paper its best to be from 0.7 to 0.8
        public static double MinUtilization { get; } = 0.7;
        public static double MaxUtilization { get; } = 0.9;
        public static double OtherMinUtilization { get; } = 0.3;
        public static double OtherMaxUtilization { get; } = 0.7;

        public static int CheckRate { get; private set; } = -1;

        public static double Alpha { get; } = 0.2;
        public static double Beta { get; } = 0.8;
        public static double Gamma { get; } = 0.15;

        public static PredictionSource PredictionSource { get; } = PredictionSource.Container;

        /// <summary>
        /// CPU = 100 unit
        /// Memory = 16GB
        /// IO = 100 unit
        /// </summary>
        public static Load DataCenterHostConfiguration { get; } = new Load(100, 32*1024, 100);


        ///// <summary>
        ///// Packet size / Bit rate
        ///// Needs Great Modification
        ///// </summary>
        ///// <param name="size"></param>
        ///// <param name="speed"></param>
        ///// <param name="unit"></param>
        ///// <returns></returns>
        //public static int GetNetworkDelay(int size, NetWorkSpeed speed, SizeUnit unit)
        //{
        //    decimal finalSize = size;
        //    switch (unit)
        //    {
        //        case SizeUnit.Bit:
        //            break;
        //        case SizeUnit.Byte:
        //            finalSize *= 8;
        //            break;
        //        case SizeUnit.KByte:
        //            finalSize *= 8;
        //            finalSize *= 1024;
        //            break;
        //        case SizeUnit.MByte:
        //            finalSize *= 8;
        //            finalSize *= 1024;
        //            finalSize *= 1024;
        //            break;
        //        case SizeUnit.GByte:
        //            finalSize *= 8;
        //            finalSize *= 1024;
        //            finalSize *= 1024;
        //            finalSize *= 1024;
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
        //    }
        //    decimal finalDelay = Convert.ToDecimal(Math.Pow(10, 6));
        //    switch (speed)
        //    {
        //        case NetWorkSpeed.TenG:
        //            finalDelay *= 10;
        //            break;
        //        case NetWorkSpeed.HundredG:
        //            finalDelay *= 100;
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(speed), speed, null);
        //    }

        //    var result = finalSize/finalDelay;
        //    return Convert.ToInt32(result);
        //}

        //Move to network resource
        public static CommonLoadManager CommonLoadManager = null;
    }
}
