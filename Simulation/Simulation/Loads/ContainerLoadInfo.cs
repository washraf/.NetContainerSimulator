namespace Simulation.Loads
{
    public class ContainerLoadInfo : LoadInfo
    {
        public ContainerLoadInfo(int containerId,int imageId, int migrationsCount,int migrationCost, Load currentLoad) : base(currentLoad)
        {
            ContainerId = containerId;
            ImageId = imageId;
            MigrationsCount = migrationsCount;
            MigrationCost = migrationCost;
        }

        public int ContainerId { get; private set; }
        public int ImageId { get; }
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
}