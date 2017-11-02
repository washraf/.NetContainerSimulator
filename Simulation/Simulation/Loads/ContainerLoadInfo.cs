namespace Simulation.Loads
{
    public class ContainerLoadInfo : LoadInfo
    {
        public ContainerLoadInfo(int containerId, int migrationsCount,int migrationCost, Load currentLoad) : base(currentLoad)
        {
            ContainerId = containerId;
            MigrationsCount = migrationsCount;
            MigrationCost = migrationCost;
        }

        public int ContainerId { get; private set; }
        public int MigrationsCount { get; private set; }
        public int MigrationCost { get; private set; }

        /// <summary>
        /// CPU Only
        /// </summary>
        /// <returns></returns>
        public double VolumeToSizeRatioToMigrationsCount
        {
            //CurrentLoad.MemorySize *
            get { return CurrentLoad.CpuLoad/( (MigrationsCount + 1)); }
        }

        //

        //Add Volume
    }
    public abstract class LoadInfo
    {
        public LoadInfo(Load currentLoad)
        {
            CurrentLoad = currentLoad;
        }
        public Load CurrentLoad { get; private set; }


    }
}