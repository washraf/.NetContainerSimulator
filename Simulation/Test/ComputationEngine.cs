using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Test
{
    public class ComputationEngine
    {
        public static List<LoadedItem> ComputeAverage()
        {
            List<LoadedItem> myList = new List<LoadedItem>();
            using (var reader = new StreamReader(@"F:\Results\stats\all_stats.csv"))
            {
                var line = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    var values = line.Split(',').ToList();
                    if (values.Count > 41)
                        values.RemoveAt(41);
                    var nvalues = values.Select(x => x.Split('"')[0]).ToList();
                    var nitem = new LoadedItem()
                    {
                        ExperimentName = nvalues[0],
                        ExperimentId = nvalues[1],

                        hostSelectionPolicy = nvalues[2],
                        vmAllocationPolicy = nvalues[3],
                        OLThreshold = Double.Parse(nvalues[4]),
                        ULThreshold = Double.Parse(nvalues[5]),
                        VMSPolicy = nvalues[6],
                        ContainerSpolicy = nvalues[6],
                        ContainerPlacement = nvalues[8],
                        Percentile = Double.Parse(nvalues[9]),
                        numberOfHosts = Double.Parse(nvalues[10]),
                        numberOfVms = Double.Parse(nvalues[11]),
                        totalSimulationTime = Double.Parse(nvalues[12]),
                        slaOverall = Double.Parse(nvalues[13]),
                        slaAverage = Double.Parse(nvalues[14]),
                        slaTimePerActiveHost = Double.Parse(nvalues[15]),
                        meanTimeBeforeHostShutdown = Double.Parse(nvalues[16]),
                        stDevTimeBeforeHostShutdown = Double.Parse(nvalues[17]),
                        medTimeBeforeHostShutdown = Double.Parse(nvalues[18]),
                        meanTimeBeforeContainerMigration = Double.Parse(nvalues[19]),
                        stDevTimeBeforeContainerMigration = Double.Parse(nvalues[20]),
                        medTimeBeforeContainerMigration = Double.Parse(nvalues[21]),
                        meanActiveVm = Double.Parse(nvalues[22]),
                        stDevActiveVm = Double.Parse(nvalues[23]),
                        medActiveVm = Double.Parse(nvalues[24]),
                        meanActiveHosts = Double.Parse(nvalues[25]),
                        stDevActiveHosts = Double.Parse(nvalues[26]),
                        medActiveHosts = Double.Parse(nvalues[27]),
                        meanNumberOfContainerMigrations = Double.Parse(nvalues[28]),
                        stDevNumberOfContainerMigrations = Double.Parse(nvalues[29]),
                        medNumberOfContainerMigrations = Double.Parse(nvalues[30]),
                        meanDatacenterEnergy = Double.Parse(nvalues[31]),
                        stDevDatacenterEnergy = Double.Parse(nvalues[32]),
                        medDatacenterEnergy = Double.Parse(nvalues[33]),
                        totalContainerMigration = Double.Parse(nvalues[34]),
                        totalVmMigration = Double.Parse(nvalues[35]),
                        totalVmCreated = Double.Parse(nvalues[36]),
                        numberOfOverUtilization = Double.Parse(nvalues[37]),
                        energy = Double.Parse(nvalues[38]),
                        CreatedContainers = Double.Parse(nvalues[39]),
                        CreatedVms = Double.Parse(nvalues[40]),
                    };
                    using (var conMigReader = new StreamReader(@"F:\Results\ContainerMigration\"+nitem.ExperimentId.Substring(0,nitem.ExperimentId.Length-2) + "\\"+nitem.ExperimentId+".csv"))
                    {
                        line = conMigReader.ReadLine();
                        while (!reader.EndOfStream)
                        {
                            line = conMigReader.ReadLine();
                            if(line== null)
                                break;
                            values = line.Split(',').ToList();
                            nitem.Containermigrations.Add(Double.Parse(values[1]));
                        }
                    }
                        myList.Add(nitem);
                }
            }
            var avgList = new List<LoadedItem>();
            var exptypes = myList.Select(x => x.ExperimentName).Distinct();
            foreach (var exptype in exptypes)
            {
                var related = myList.Where(x => x.ExperimentName == exptype).ToList();
                var averageItem = new LoadedItem()
                {
                    ExperimentName = exptype,
                    hostSelectionPolicy = related[0].hostSelectionPolicy,
                    ContainerPlacement = related[0].ContainerPlacement,
                    ContainerSpolicy = related[0].ContainerSpolicy,
                    CreatedContainers = related.Average(x => x.CreatedContainers),
                    CreatedVms = related.Average(x => x.CreatedVms),
                    OLThreshold = related.Average(x => x.OLThreshold),
                    Percentile = related.Average(x => x.Percentile),
                    ULThreshold = related.Average(x => x.ULThreshold),
                    VMSPolicy = related[0].VMSPolicy,
                    energy = related.Average(x => x.energy),
                    meanActiveHosts = related.Average(x => x.meanActiveHosts),
                    meanActiveVm = related.Average(x => x.meanActiveVm),
                    meanDatacenterEnergy = related.Average(x => x.meanDatacenterEnergy),
                    meanNumberOfContainerMigrations = related.Average(x => x.meanNumberOfContainerMigrations),
                    meanTimeBeforeContainerMigration = related.Average(x => x.meanTimeBeforeContainerMigration),
                    meanTimeBeforeHostShutdown = related.Average(x => x.meanTimeBeforeHostShutdown),
                    medActiveHosts = related.Average(x => x.medActiveHosts),
                    medActiveVm = related.Average(x => x.medActiveVm),
                    medDatacenterEnergy = related.Average(x => x.medDatacenterEnergy),
                    medNumberOfContainerMigrations = related.Average(x => x.medNumberOfContainerMigrations),
                    medTimeBeforeContainerMigration = related.Average(x => x.medTimeBeforeContainerMigration),
                    medTimeBeforeHostShutdown = related.Average(x => x.medTimeBeforeHostShutdown),
                    numberOfHosts = related.Average(x => x.numberOfHosts),
                    numberOfOverUtilization = related.Average(x => x.numberOfOverUtilization),
                    numberOfVms = related.Average(x => x.numberOfVms),
                    slaAverage = related.Average(x => x.slaAverage),
                    slaOverall = related.Average(x => x.slaOverall),
                    slaTimePerActiveHost = related.Average(x => x.slaTimePerActiveHost),
                    stDevActiveHosts = related.Average(x => x.stDevActiveHosts),
                    stDevActiveVm = related.Average(x => x.stDevActiveVm),
                    stDevDatacenterEnergy = related.Average(x => x.stDevDatacenterEnergy),
                    stDevNumberOfContainerMigrations = related.Average(x => x.stDevNumberOfContainerMigrations),
                    stDevTimeBeforeContainerMigration = related.Average(x => x.stDevTimeBeforeContainerMigration),
                    stDevTimeBeforeHostShutdown = related.Average(x => x.stDevTimeBeforeHostShutdown),
                    totalContainerMigration = related.Average(x => x.totalContainerMigration),
                    totalSimulationTime = related.Average(x => x.totalSimulationTime),
                    totalVmCreated = related.Average(x => x.totalVmCreated),
                    totalVmMigration = related.Average(x => x.totalVmMigration),
                    vmAllocationPolicy = related[0].vmAllocationPolicy
                };
                avgList.Add(averageItem);
            }
            return avgList;
        }

    }
}