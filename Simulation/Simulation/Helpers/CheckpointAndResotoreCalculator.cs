using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkpoint_Restore;
using Simulation.Configuration;

namespace Simulation.Helpers
{
    public class CheckpointAndResotoreCalculator
    {
        private Dictionary<int, CheckpointValueHolder> Data;

        public CheckpointAndResotoreCalculator()
        {
            var data = DataLoader.Load(@"D:\Normal_log");
            Data = DataCleaner.CleanData(data);
        }

        public int GetCheckpointTime(double size)
        {
            int v = DataCleaner.GetNearstFifty(Convert.ToInt32(size));
            while (!Data.ContainsKey(v))
            {
                v -= 50;
            }
            var ctime =  Convert.ToInt32(Data[v].GetAverageCheckpointValue().CheckPointTime * Global.Second);
            return ctime;
        }

        public int GetRestorationTime(double size)
        {
            int v = DataCleaner.GetNearstFifty(Convert.ToInt32(size));
            while (!Data.ContainsKey(v))
            {
                v -= 50;
            }
            int rtime= Convert.ToInt32(Data[v].GetAverageCheckpointValue().RestorationTime * Global.Second);
            return rtime;
        }
    }
}
