using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace Checkpoint_Restore
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cb_GraphItem.DataSource = Enum.GetValues(typeof(CheckRestoreGraphItems)).Cast<CheckRestoreGraphItems>();
            cb_GraphItem.SelectedIndex = 0;
        }

        private List<Dictionary<int, CheckpointValueHolder>> _results = new List<Dictionary<int, CheckpointValueHolder>>();
        private void btn_Load_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            var r = f.ShowDialog();
            if (r == DialogResult.OK)
            {
                var data = DataLoader.Load(f.FileName);
                var cleanedData = DataCleaner.CleanData(data);
                _results.Add(cleanedData);
                CreateGraph(zedGraphControl1, cb_GraphItem.Text, (CheckRestoreGraphItems)cb_GraphItem.SelectedValue);
            }
        }

        private void CreateGraph(ZedGraphControl zgc, string title, CheckRestoreGraphItems yAxis)
        {
            // get a reference to the GraphPane
            GraphPane myPane = zgc.GraphPane;

            myPane.CurveList.Clear();
            // Set the Titles
            myPane.Title.Text = "Container Check point and restoration time";
            myPane.XAxis.Title.Text = "Memory size in Mega Bytes";
            myPane.YAxis.Title.Text = "time in seconds";//yAxis.ToString();
            for (int t = 0; t < _results.Count; t++)
            {
                List<PointPairList> list = new List<PointPairList>();
                list.Add(new PointPairList());

                List<string> labeList = new List<string>();
                labeList.Add($"Average {yAxis.ToString()} of Trial {t}");
                var keyList = _results[t].Select(x => x.Key);
                if (cb_DrawExtra.Checked && yAxis != CheckRestoreGraphItems.Both)
                {
                    list.Add(new PointPairList());
                    labeList.Add($"Min {yAxis.ToString()} of Trial {t}");
                    list.Add(new PointPairList());
                    labeList.Add($"Max {yAxis.ToString()} of Trial {t}");

                }
                switch (yAxis)
                {
                    case CheckRestoreGraphItems.CheckPointTime:

                        foreach (var key in keyList)
                        {
                            list[0].Add(key, _results[t][key].GetAverageCheckpointValue().CheckPointTime);
                            if (cb_DrawExtra.Checked)
                            {
                                list[1].Add(key, _results[t][key].GetMinCheckpointValue().CheckPointTime);
                                list[2].Add(key, _results[t][key].GetMaxCheckpointValue().CheckPointTime);
                            }
                        }
                        break;
                    case CheckRestoreGraphItems.RestorationTime:
                        foreach (var key in keyList)
                        {
                            list[0].Add(key, _results[t][key].GetAverageCheckpointValue().RestorationTime);
                            if (cb_DrawExtra.Checked)
                            {
                                list[1].Add(key, _results[t][key].GetMinCheckpointValue().RestorationTime);
                                list[2].Add(key, _results[t][key].GetMaxCheckpointValue().RestorationTime);
                            }
                        }
                        break;
                    case CheckRestoreGraphItems.TotalTime:
                        foreach (var key in keyList)
                        {
                            list[0].Add(key, _results[t][key].GetAverageCheckpointValue().GetTotalTime);
                            if (cb_DrawExtra.Checked)
                            {
                                list[1].Add(key, _results[t][key].GetMinCheckpointValue().GetTotalTime);
                                list[2].Add(key, _results[t][key].GetMaxCheckpointValue().GetTotalTime);
                            }
                        }
                        break;
                    case CheckRestoreGraphItems.CopyFromTime:
                        foreach (var key in keyList)
                        {
                            list[0].Add(key, _results[t][key].GetAverageCheckpointValue().CopyFromTime);
                            if (cb_DrawExtra.Checked)
                            {
                                list[1].Add(key, _results[t][key].GetMinCheckpointValue().CopyFromTime);
                                list[2].Add(key, _results[t][key].GetMaxCheckpointValue().CopyFromTime);
                            }
                        }
                        break;
                    case CheckRestoreGraphItems.CopyToTime:
                        foreach (var key in keyList)
                        {
                            list[0].Add(key, _results[t][key].GetAverageCheckpointValue().CopyToTime);
                            if (cb_DrawExtra.Checked)
                            {
                                list[1].Add(key, _results[t][key].GetMinCheckpointValue().CopyToTime);
                                list[2].Add(key, _results[t][key].GetMaxCheckpointValue().CopyToTime);
                            }
                        }
                        break;
                    case CheckRestoreGraphItems.Both:
                        labeList[0] = "Checkpoint";
                        list.Add(new PointPairList());
                        labeList.Add($"Restoration");
                        foreach (var key in keyList)
                        {
                            list[0].Add(key, _results[t][key].GetAverageCheckpointValue().CheckPointTime);
                            list[1].Add(key, _results[t][key].GetAverageCheckpointValue().RestorationTime);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
                // Generate a red curve with diamond
                // symbols, and "Porsche" in the legend

                for (int i = 0; i < list.Count; i++)
                {
                    LineItem myCurve1 =
                        new LineItem(labeList[i], list[i], GetColor(i), SymbolType.None, 3.0f);
                    myPane.CurveList.Add(myCurve1);
                    
                }
            }
            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zgc.BackColor = Color.White;
            zgc.AxisChange();
            zgc.Refresh();
        }

        private Color GetColor(int i)
        {
            switch (i)
            {
                case 0:
                    return Color.Red;
                case 1:
                    return Color.Blue;
                case 2:
                    return Color.Green;
                case 3:
                    return Color.Black;
                case 4:
                    return Color.DarkMagenta;
                default:
                    return Color.GreenYellow;
            }
        }
        private SymbolType GetSymbolType(int i)
        {
            switch (i)
            {
                case 0:
                    return SymbolType.Star;
                case 1:
                    return SymbolType.Diamond;
                case 2:
                    return SymbolType.Square;
                case 3:
                    return SymbolType.Circle;
                default:
                    throw new KeyNotFoundException();

            }
            //return Enum.GetValues(typeof(SymbolType)).Cast<SymbolType>().Reverse().ToList()[i];
        }

        private void cb_GraphItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateGraph(zedGraphControl1, cb_GraphItem.Text, (CheckRestoreGraphItems)cb_GraphItem.SelectedValue);
        }


    }
}
