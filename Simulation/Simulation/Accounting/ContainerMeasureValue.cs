using System;
using System.Xml;

namespace Simulation.Accounting
{
    public class ContainerMeasureValue
    {
        public double ConId { get; private set; }
        public double MigrationCount { get; private set; }
        public double Downtime { get; private set; }

        public ContainerMeasureValue(double conId,double migrationCount,double downtime)
        {
            ConId = conId;
            MigrationCount = migrationCount;
            Downtime = downtime;
        }

        public static ContainerMeasureValue operator +(ContainerMeasureValue first, ContainerMeasureValue second)
        {
            if (first.ConId != second.ConId)
            {
                throw new NotImplementedException("Container Measure Value");
            }
            return new ContainerMeasureValue(first.ConId,first.MigrationCount+second.MigrationCount,first.Downtime+second.Downtime);
        }
        public static ContainerMeasureValue operator /(ContainerMeasureValue first,double count)
        {
            return new ContainerMeasureValue(first.ConId, first.MigrationCount/count, first.Downtime/count);
        }
    }
}