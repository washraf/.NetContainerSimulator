using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkpoint_Restore
{
    
    public static class DataCleaner
    {
        public static Dictionary<int, CheckpointValueHolder> CleanData(List<CheckpointValue> checkpointValues)
        {
            Dictionary<int, CheckpointValueHolder> checkpointValueHolders = new Dictionary<int, CheckpointValueHolder>();
            
            foreach (var cvalue in checkpointValues)
            {
                int x = GetNearstFifty(cvalue.TotalMemory);
                if (!checkpointValueHolders.ContainsKey(x))
                {
                    checkpointValueHolders.Add(x,new CheckpointValueHolder());
                }
               checkpointValueHolders[x].AddCheckpointValue(cvalue);
            }
            return checkpointValueHolders;
        }
       
        public static int GetNearstFifty(int memorysize)
        {
            int n = memorysize / 50;
            //n--;
            //int r = memorysize%50;
            //if (r > 25)
            //{
            //    n++;
            //}
            return n*50;
        }
    }
}
