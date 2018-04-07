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
        public static int NoOfTrials { get; } = 10;

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
                    Second = strategy != Strategies.Zhao ? 8 : 20;
                    break;
                case SimulationSize.Hundred:
                    Second = strategy != Strategies.Zhao ? 15 : 40;
                    break;
                case SimulationSize.TwoHundred:
                    Second = strategy != Strategies.Zhao ? 20 : 60;
                    break;
                case SimulationSize.ThreeHundred:
                    Second = strategy != Strategies.Zhao ? 30 : 70;
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


        /// <summary>
        /// https://www.incapsula.com/blog/mtu-mss-explained.html
        /// https://blog.thousandeyes.com/a-very-simple-model-for-tcp-throughput/
        /// https://www.netcraftsmen.com/tcp-performance-and-the-mathis-equation/
        /// Paper = PTP-Mesh
        /// //Loss Percentage (p) = 10^-4
        /// MaxTrasmissionUnit (MTU)= 1460 byte
        /// MaxSegmentsize = MTU + 40 = 1500 byte
        /// TCPHeader = 24 byte
        /// MSS = 1524
        /// RTT = 1 m sec
        /// C = 1.22 = root 1.5
        /// T = MSS*C/RTT*root(p)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static int GetNetworkDelay(double size, NetworkSpeed speed = NetworkSpeed.HundredG)
        {
            if (size > 1)
            {

            }
            var p = 0.0001;
            var mss = 1524;
            var RTT = 400 / Math.Pow(10,6);
            var c = Math.Sqrt(1.5);
            var throuputinByte = mss * c / (RTT*Math.Sqrt(p));
            //var T = throuputinByte / (1024 * 1024);
            var sizeinByte = size * 1024 * 1024;
            var noofPackets = Math.Ceiling(sizeinByte / 1460.0);
            var finalSize = sizeinByte + 64 * noofPackets;
            var delay =  finalSize/throuputinByte;
            return Convert.ToInt32(delay);
            //decimal finalSize = size;
            //finalSize *= 8;
            //finalSize *= 1024;
            //finalSize *= 1024;
            //decimal finalDelay = Convert.ToDecimal(Math.Pow(10, 6));
            //switch (speed)
            //{
            //    case NetworkSpeed.TenG:
            //        finalDelay *= 10;
            //        break;
            //    case NetworkSpeed.HundredG:
            //        finalDelay *= 100;
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException(nameof(speed), speed, null);
            //}

            //var result = finalSize / finalDelay;
            //return Convert.ToInt32(result);
        }

        //Move to network resource
        public static CommonLoadManager CommonLoadManager = null;
    }
}
