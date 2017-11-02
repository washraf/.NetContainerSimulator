using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkpoint_Restore
{
    public static class DataLoader
    {
        public static List<CheckpointValue> Load(string file)
        {
            
            List<CheckpointValue> items = new List<CheckpointValue>();
            using (StreamReader reader = new StreamReader(file))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    int pcount = Convert.ToInt32(values[1]);
                    int tcount = Convert.ToInt32(values[2]);
                    int tMem = Convert.ToInt32(values[3]);
                    int ram = Convert.ToInt32(values[4]);
                    int swap = Convert.ToInt32(values[5]);
                    double ctime = Convert.ToDouble(values[6]);
                    int csize = Convert.ToInt32(values[7]);
                    double rtime = Convert.ToDouble(values[8]);
                    double from = 0;
                    double to = 0;
                    if (values.Count() > 9)
                    {
                        from = Convert.ToDouble(values[9]);
                        to = Convert.ToDouble(values[10]);
                    }
                    var item = new CheckpointValue(pcount, tcount, tMem, ram, swap, ctime, csize, rtime, from, to);
                    items.Add(item);
                }
            }
            return items;
        } 

    }
}
