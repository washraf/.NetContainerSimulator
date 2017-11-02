using System.Collections.Generic;

namespace Test
{
    public class LoadedItem
    {
        public string ExperimentName { set; get; }
        public string ExperimentId { set; get; }
        public string hostSelectionPolicy { set; get; }
        public string vmAllocationPolicy { set; get; }
        public double OLThreshold { set; get; }
        public double ULThreshold { set; get; }
        public string VMSPolicy { set; get; }
        public string ContainerSpolicy { set; get; }
        public string ContainerPlacement { set; get; }
        public double Percentile { set; get; }
        public double numberOfHosts { set; get; }
        public double numberOfVms { set; get; }
        public double totalSimulationTime { set; get; }
        public double slaOverall { set; get; }
        public double slaAverage { set; get; }
        public double slaTimePerActiveHost { set; get; }
        public double meanTimeBeforeHostShutdown { set; get; }
        public double stDevTimeBeforeHostShutdown { set; get; }
        public double medTimeBeforeHostShutdown { set; get; }
        public double meanTimeBeforeContainerMigration { set; get; }
        public double stDevTimeBeforeContainerMigration { set; get; }
        public double medTimeBeforeContainerMigration { set; get; }
        public double meanActiveVm { set; get; }
        public double stDevActiveVm { set; get; }
        public double medActiveVm { set; get; }
        public double meanActiveHosts { set; get; }
        public double stDevActiveHosts { set; get; }
        public double medActiveHosts { set; get; }
        public double meanNumberOfContainerMigrations { set; get; }
        public double stDevNumberOfContainerMigrations { set; get; }
        public double medNumberOfContainerMigrations { set; get; }
        public double meanDatacenterEnergy { set; get; }
        public double stDevDatacenterEnergy { set; get; }
        public double medDatacenterEnergy { set; get; }
        public double totalContainerMigration { set; get; }
        public double totalVmMigration { set; get; }
        public double totalVmCreated { set; get; }
        public double numberOfOverUtilization { set; get; }
        public double energy { set; get; }
        public double CreatedContainers { set; get; }
        public double CreatedVms { set; get; }
        public List<double> Containermigrations { get; set; }

        public LoadedItem()
        {
            Containermigrations = new List<double>();
        }
    }
}