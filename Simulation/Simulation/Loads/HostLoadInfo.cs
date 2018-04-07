using System;
using Simulation.Configuration;

namespace Simulation.Loads
{
    public class HostLoadInfo:LoadInfo
    {
        public HostLoadInfo(int hostId,Load currentLoad,int containersCount,
            double cpu,double mem,double io, double dataSizeOut, double dataSizeIn)
            :base(currentLoad)
        {
            HostId = hostId;
            ContainersCount = containersCount;
            CPUUtil = cpu;
            MemoryUtil = mem;
            IOUtil = io;
            DataSizeOut = dataSizeOut;
            DataSizeIn = dataSizeIn;
        }
        

        public int ContainersCount { get; set; }
        public int HostId { get; private set; }
        /// <summary>
        /// Ration between the Current Host CPU Load and The Max CPU Load
        /// </summary>
        public double CPUUtil { get; private set; }
        /// <summary>
        /// Ration between the Current Used Memory and The Max Memory
        /// </summary>
        public double MemoryUtil { get; private set;}
        /// <summary>
        /// Ration between the Current IO/S Load and The Max IO/S
        /// </summary>
        public double IOUtil { get; private set; }
        public double DataSizeOut { get; }
        public double DataSizeIn { get; }
        public double DataTotal { get { return DataSizeIn + DataSizeOut; } }

        private UtilizationStates GetFloatState(double v,double min,double max)
        {
            if (v > max)
                return UtilizationStates.OverUtilization;

            if (v < min)
                return UtilizationStates.UnderUtilization;
            return UtilizationStates.Normal;
        }

        /// <summary>
        /// Should Indicate the load of the host to be either High, Normal or Under
        /// Use Only CPU Time
        /// </summary>
        /// <returns></returns>
        public UtilizationStates CalculateTotalUtilizationState(double min, double max)
        {
            UtilizationStates cpu = GetFloatState(Volume,min,max);
            return cpu;
        }

        //Only CPU
        /// <summary>
        /// Should Always be in percentage form
        /// </summary>
        public double Volume
        {
            get
            {
                //double volume = 1/((1 - CPUUtil)*(1 - IOUtil)*(1 - MemoryUtil));
                var volume = CPUUtil;
                if (Double.IsNaN(volume) || volume<0)
                {
                    throw new NotImplementedException();
                }
                return volume;
            }
        }

        public override string ToString()
        {
            return $"{HostId}, {CurrentLoad},{ContainersCount},{CPUUtil},{MemoryUtil},{IOUtil},{DataSizeOut},{DataSizeIn}";
        }
    }
}