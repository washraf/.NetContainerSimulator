using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

            SimulationController.SimulationController controller =
                new SimulationController.SimulationController(Global.CurrentStrategy,Global.SimulationSize, Global.StartUtilizationPercent,Global.LoadPrediction,Global.ChangeAction);
            //Create Hosts
            ConsolePrinting(controller);

            controller.StartSimulation();
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

                    foreach (var machine in hosts.Skip(1))
                    {
                        var host = machine as HostMachine;
                        Console.WriteLine(
                            $"\tHost #{host.MachineId} containers count is {host.GetContainersCount()}" +
                            $" CPU Load = {host.GetNeededHostLoadInfo().CalculateTotalUtilizationState(host.MinUtilization, host.MaxUtilization)}" +
                            $" and lamdba = {host.MaxUtilization}" +
                            $" and Volume = {host.GetPredictedHostLoadInfo().Volume}");
                    }
                    Console.WriteLine($"###Total is {controller.MachineControllerObject.GetMachinesCount()}");
                    Thread.Sleep(500);
                }
            });
            t.Start();
            Console.WriteLine("Baaasadjbcacbak");


        }

        #endregion

    }
}
