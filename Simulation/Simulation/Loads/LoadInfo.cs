namespace Simulation.Loads
{
    public abstract class LoadInfo
    {
        public LoadInfo(Load currentLoad)
        {
            CurrentLoad = currentLoad;
        }
        /// <summary>
        /// The Current Load of the Host in terms of CPU Usage, Memory and IO/S
        /// </summary>
        public Load CurrentLoad { get; private set; }


    }
}