using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulation.Accounting;
using Simulation.Configuration;
using Simulation.DataCenter;
using Simulation.Helpers;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.SimulationController;
namespace Simulation
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var predictors = new List<LoadPrediction>()
            {
                //LoadPrediction.None,
                //LoadPrediction.Ewma,
                LoadPrediction.Arma,
                //LoadPrediction.LinReg,
            };

            var sizes = new List<SimulationSize>()
            {
                SimulationSize.Five,
                //SimulationSize.Ten,
                //SimulationSize.Twenty,
                //SimulationSize.Fifty,
                //SimulationSize.Hundred,
                //SimulationSize.TwoHundred
            };


            var util = new List<StartUtilizationPercent>()
            {
                //StartUtilizationPercent.Thrity,
                StartUtilizationPercent.Fifty,
                //StartUtilizationPercent.Seventy,
            };

            var algorithms = new List<Strategies>()
            {
                Strategies.WAshraf2017Auction,
                Strategies.WAshraf2017,
                //Strategies.Zhao,
                //Strategies.ForsmanPush,
                //Strategies.ForsmanPull,
            };

            var cactions = new List<LoadChangeAction>()
            {
                LoadChangeAction.Burst,
                //LoadChangeAction.VeryHightDrain,
                //LoadChangeAction.VreyHighOpposite
            };


            foreach (var u in util)
            {
                Global.StartUtilizationPercent = u;
                foreach (var s in sizes)
                {
                    Global.SetSimulationSize(s);
                    foreach (var action in cactions)
                    {
                        //if(action== LoadChangeAction.VeryHightBurst && u==StartUtilizationPercent.Seventy)
                        //    continue;
                        Global.ChangeAction = action;
                        foreach (var alg in algorithms)
                        {
                            Global.SetCurrentStrategy(alg);
                            foreach (var predictor in predictors)
                            {
                                if (alg == Strategies.WAshraf2017 || alg == Strategies.WAshraf2017Auction)
                                {
                                    Global.LoadPrediction = predictor;
                                }
                                else if (predictor == LoadPrediction.None)
                                {

                                }
                                else
                                    continue;

                                #region --ALL--

                                List<MeasureValueHolder> internalValueListsTrials =
                                    new List<MeasureValueHolder>();
                                //MeasureValueHolder holder = null;
                                for (int i = 0; i < Global.NoOfTrials; i++)
                                {
                                    //var stime = DateTime.Now;
                                    SimulationController.SimulationController controller =
                                        new SimulationController.SimulationController(Global.CurrentStrategy,
                                            Global.SimulationSize,
                                            Global.StartUtilizationPercent,
                                            Global.LoadPrediction,
                                            Global.ChangeAction,
                                            Global.TestedItems,
                                            ContainersType.N);
                                    ConsolePrinting(controller);
                                    controller.StartSimulation();
                                    //var etime = DateTime.Now;
                                    //MessageBox.Show((etime - stime).TotalSeconds.ToString());
                                    //Thread.Sleep(Global.GetSimulationTime);

                                    //controller.EndSimulation();
                                    //holder = controller.AccountingModuleObject.MeasureHolder;
                                    internalValueListsTrials.Add(controller.AccountingModuleObject.MeasureHolder);
                                    //await Task.Delay(5000);
                                    if (Global.NoOfTrials > 1)
                                    {
                                        controller.AccountingModuleObject.MeasureHolder.WriteDataToDisk(i);
                                    }
                                    Thread.Sleep(5000);

                                }
                                //List<MeasuresValues> final = new List<MeasuresValues>();
                                //MeasureValueHolder final = new MeasureValueHolder(Global.CurrentStrategy,
                                //    Global.SimulationSize,
                                //    Global.StartUtilizationPercent,
                                //    Global.ChangeAction,
                                //    Global.LoadPrediction);
                                MeasureValueHolder final = internalValueListsTrials[0] / 1;
                                foreach (var list in internalValueListsTrials.Skip(1))
                                {
                                    final = final + list;
                                }
                                final = final / Global.NoOfTrials;
                                final.WriteDataToDisk(-1);

                                #endregion
                            }
                        }
                    }
                }
            }
        //Thread.Sleep(Global.GetSimulationTime);
            Console.WriteLine("I am Done Press any key to Go Away !!!");
            Console.ReadLine();
            //controller.EndSimulation();
        }




        #region --Console Printing--

        public static void ConsolePrinting(SimulationController.SimulationController controller)
        {
            Task t = new Task(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                for (int i = 0; i < 10000; i++)
                {
                    var hosts = controller.MachineTableObject.GetAllMachines();

                    //foreach (var machine in hosts.Skip(1))
                    //{
                    //    var host = machine as HostMachine;
                    //    Console.WriteLine(
                    //        $"\tHost #{host.MachineId} containers count is {host.GetContainersCount()}" +
                    //        $" CPU Load = {host.GetNeededHostLoadInfo().CalculateTotalUtilizationState(host.MinUtilization, host.MaxUtilization)}" +
                    //        $" and lamdba = {host.MaxUtilization}" +
                    //        $" and Volume = {host.GetPredictedHostLoadInfo().Volume}");
                    //}
                    //Console.WriteLine($"###Total is {controller.MachineControllerObject.GetMachinesCount()}");
                    Thread.Sleep(500);
                }
            });
            t.Start();
            Console.WriteLine("Baaasadjbcacbak");


        }

        #endregion

    }
}
