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
        /// </summary>
        public static int Steps = 180;
        public static int NoOfTrials { get; } = 10;

        public static int GetSimulationTime
        {
            
            get
            {
                return 9000 * Global.Second;
                //return 21600 * Global.Second;
                //good with 50
                //return 18000 * Global.Second;
            }

        }

        //for container source 5,5
        public static int LinerRegWindow = 10;
        public static int PredictionTime = 10;

        //best is 50
        public static int Second { get; private set; } = -1;
        private static void UpdateTime()
        {
            switch (SimulationSize)
            {
                case SimulationSize.Twenty:
                    Second = CurrentStrategy != Strategies.Zhao ? 5 : 10;
                    break;
                case SimulationSize.Fifty:
                    Second = CurrentStrategy != Strategies.Zhao ? 10 : 20;
                    break;
                case SimulationSize.Hundred:
                    Second = CurrentStrategy != Strategies.Zhao ? 20 : 40;
                    break;
                case SimulationSize.TwoHundred:
                    Second = CurrentStrategy != Strategies.Zhao ? 30 :
                        StartUtilizationPercent==StartUtilizationPercent.Fifty? 60:100;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            AccountTime = 100 * Second;
            CheckRate = 20*Second;
        }

        static Global()
        {
            //Read From Test Configuration
        }

        public static LoadPrediction LoadPrediction { get; set; } = LoadPrediction.None;
        public static Strategies CurrentStrategy { get; private set; } = Strategies.InOrderProping;

        public static void SetCurrentStrategy(Strategies alg)
        {
            CurrentStrategy = alg;
            switch (alg)
            {
                case Strategies.InOrderProping:
                    //LoadPrediction = LoadPrediction.LinReg;
                    break;
                case Strategies.Zhao:
                    LoadPrediction = LoadPrediction.None;

                    break;
                case Strategies.ForsmanPush:
                case Strategies.ForsmanPull:
                    LoadPrediction = LoadPrediction.Ewma;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(alg), alg, null);
            }
            UpdateTime();
        }

        public static LoadChangeStrategy LoadChangeStrategy { get; set; } = LoadChangeStrategy.Incremental;
        //public static LoadCreationStrategy LoadCreationStrategy { get; set; } = LoadCreationStrategy.Selected;
        public static int AccountTime { get; private set; } = -1;
        //public static int AccountTime { get; } = Second;
        
        public static SimulationSize SimulationSize { get; private set; } = SimulationSize.Twenty;

        public static void SetSimulationSize(SimulationSize simulationSize)
        {
            SimulationSize = simulationSize;
            UpdateTime();
        }

        public static LoadChangeAction ChangeAction { get; set; } = LoadChangeAction.VeryHightBurst;

        //Test Default 0.7, 0.9
        //From energy paper its best to be from 0.7 to 0.8
        public static double MinUtilization { get; } = 0.7;
        public static double MaxUtilization { get; } = 0.9;
        public static double OtherMinUtilization { get; } = 0.3;
        public static double OtherMaxUtilization { get; } = 0.7;

        //Test Default 8
        public static int GetNoOfFailures
        {
            get
            {
                int n = ((int) SimulationSize)/10;
                n++;
                return n;
            }
        }

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

        public static Load ContainerLoadPreNormal { get; } = new Load(2.5, 384, 2.5);
        public static Load ContainerLoadNormal { get; } = new Load(2.5, 512, 2.5);
        public static Load ContainerLoadPostNormal { get; } = new Load(7.5, 640, 7.5);
        public static StrategyActionType OtherPushPullStrategy { get; set; } = StrategyActionType.PushAction;
        public static StartUtilizationPercent StartUtilizationPercent { get; set; } = StartUtilizationPercent.Fifty;

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

        public static CommonLoadManager CommonLoadManager = null;
    }
}
