using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Checkpoint_Restore
{
    public class CheckpointValue
    {
        public CheckpointValue(int pcount,int tcount,int tMem,int ram,int swapped,double ctime,int csize,double rtime,double from,double to)
        {
            ProcessCount = pcount;
            TaskCount = tcount;
            TotalMemory = tMem;
            MemoryInRam = ram;
            MemorySwapped = swapped;
            CheckPointTime = ctime;
            CheckPointSize = csize;
            RestorationTime = rtime;
            CopyFromTime = from;
            CopyToTime = to;
        }
        public int ProcessCount { get; private set; }
        public int TaskCount { get; private set; }
        public int TotalMemory { get; private set; }
        public int MemoryInRam { get; private set; }
        public int MemorySwapped { get; private set; }
        public double CheckPointTime { get; private set; }
        public int CheckPointSize { get; private set; }
        public double RestorationTime { get; private set; }
        public double CopyFromTime { get; private set; }
        public double CopyToTime { get; private set; }


        //Data to be read
        public double GetTotalTime => CheckPointTime + RestorationTime + CopyFromTime + CopyToTime;
    }

    public class CheckpointValueHolder
    {
        private List<CheckpointValue> _checkpointValues = new List<CheckpointValue>(); 

        public void AddCheckpointValue(CheckpointValue value)
        {
            _checkpointValues.Add(value);
        }

        public CheckpointValue GetMinCheckpointValue()
        {
            int pcount = _checkpointValues.Select(x => x.ProcessCount).Min();
            int tcount = _checkpointValues.Select(x => x.TaskCount).Min();
            int tMem = _checkpointValues.Select(x => x.TotalMemory).Min();
            int ram = _checkpointValues.Select(x => x.MemoryInRam).Min();
            int swap = _checkpointValues.Select(x => x.MemorySwapped).Min();
            double ctime = _checkpointValues.Select(x => x.CheckPointTime).Min();
            int csize = _checkpointValues.Select(x => x.CheckPointSize).Min();
            double rtime = _checkpointValues.Select(x => x.RestorationTime).Min();
            double from = _checkpointValues.Select(x => x.CopyFromTime).Min();
            double to = _checkpointValues.Select(x => x.CopyToTime).Min();
            var min = new CheckpointValue(pcount, tcount, tMem, ram, swap, ctime, csize, rtime, from, to);
            return min;
        }

        public CheckpointValue GetMaxCheckpointValue()
        {
            int pcount = _checkpointValues.Select(x => x.ProcessCount).Max();
            int tcount = _checkpointValues.Select(x => x.TaskCount).Max();
            int tMem = _checkpointValues.Select(x => x.TotalMemory).Max();
            int ram = _checkpointValues.Select(x => x.MemoryInRam).Max();
            int swap = _checkpointValues.Select(x => x.MemorySwapped).Max();
            double ctime = _checkpointValues.Select(x => x.CheckPointTime).Max();
            int csize = _checkpointValues.Select(x => x.CheckPointSize).Max();
            double rtime = _checkpointValues.Select(x => x.RestorationTime).Max();
            double from = _checkpointValues.Select(x => x.CopyFromTime).Max();
            double to = _checkpointValues.Select(x => x.CopyToTime).Max();
            var max = new CheckpointValue(pcount, tcount, tMem, ram, swap, ctime, csize, rtime, from, to);
            return max;
        }
        public CheckpointValue GetAverageCheckpointValue()
        {
            int pcount = Convert.ToInt32(_checkpointValues.Select(x => x.ProcessCount).Average());
            int tcount = Convert.ToInt32(_checkpointValues.Select(x => x.TaskCount).Average());
            int tMem = Convert.ToInt32(_checkpointValues.Select(x => x.TotalMemory).Average());
            int ram = Convert.ToInt32(_checkpointValues.Select(x => x.MemoryInRam).Average());
            int swap = Convert.ToInt32(_checkpointValues.Select(x => x.MemorySwapped).Average());
            double ctime = _checkpointValues.Select(x => x.CheckPointTime).Average();
            int csize = Convert.ToInt32(_checkpointValues.Select(x => x.CheckPointSize).Average());
            double rtime = _checkpointValues.Select(x => x.RestorationTime).Average();
            double from = _checkpointValues.Select(x => x.CopyFromTime).Average();
            double to = _checkpointValues.Select(x => x.CopyToTime).Average();
            var average = new CheckpointValue(pcount, tcount, tMem, ram, swap, ctime, csize, rtime, from, to);
            return average;
        }
    }
}
