using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Simulation.Accounting;
using Simulation.Configuration;
using Simulation.LocationStrategies;
using ZedGraph;

namespace Test
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            cb_Change.DataSource = Enum.GetValues(typeof(LoadChangeAction)).Cast<LoadChangeAction>();
            cb_Change.SelectedIndex = 0;
            cb_StartUtil.DataSource = Enum.GetValues(typeof(StartUtilizationPercent)).Cast<StartUtilizationPercent>();
            cb_StartUtil.SelectedIndex = 0;
            cb_Item.DataSource = Enum.GetValues(typeof(DrawItems)).Cast<DrawItems>();
            cb_Item.SelectedIndex = 0;
            Done = true;
            DrawNewItemsFromDataBase();
        }

        public bool Done { get; set; } = false;

        private void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(Done)
                DrawNewItemsFromDataBase();
        }

        private void DrawNewItemsFromDataBase()
        {
            var trials = GetValue();
            var y = (DrawItems)Enum.Parse(typeof(DrawItems), cb_Item.SelectedValue.ToString());

            CreateGraph(zedGraphControl1, y, trials);
        }

        private void CreateGraph(ZedGraphControl zgc, DrawItems yAxis, List<Trial> trials)
        {
            // get a reference to the GraphPane
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();
            myPane.YAxis.Scale.MinAuto = true;
            myPane.YAxis.Scale.MaxAuto = true;

            // Set the Titles
            myPane.Title.Text = yAxis.ToString();
            myPane.XAxis.Title.Text = $"Number of Availble Hosts (N)";
            myPane.YAxis.Title.Text = yAxis.ToString();
            Dictionary<Strategies, PointPairList> all = new Dictionary<Strategies, PointPairList>();
            all.Add(Strategies.WAshraf2017,new PointPairList());
            all.Add(Strategies.Zhao, new PointPairList());
            all.Add(Strategies.ForsmanPush, new PointPairList());
            all.Add(Strategies.ForsmanPull, new PointPairList());

            foreach (var trial in trials)
            {
                var st = (Strategies) Enum.Parse(typeof (Strategies), trial.Algorithm);
                
                
                            //trials.Single(
                            //    x => x.Size == trial.Size && x.Algorithm == Strategies.InOrderProping.ToString());

                switch (yAxis)
                {
                    case DrawItems.AverageEntropy:
                        all[st].Add(trial.Size, trial.Entropy);
                        myPane.YAxis.Scale.Min = 0.99;
                        myPane.YAxis.Scale.Max = 1.01;
                        break;
                    case DrawItems.Power:
                        var val = trials.Where(x => x.Size == trial.Size && x.Change == trial.Change).Max(x => x.Power);
                        var mtrial = trials.Single(x => x.Size == trial.Size && x.Change == trial.Change && x.Power == val);
                        all[st].Add(trial.Size, trial.Power / mtrial.Power);
                        myPane.YAxis.Title.Text = "Power Consumption Ratio";
                        break;
                    //case DrawItems.StdDev:
                    //    all[st].Add(trial.Size, trial.StdDev);
                    //    break;
                    case DrawItems.Hosts:
                        all[st].Add(trial.Size, trial.Hosts);
                        myPane.YAxis.Title.Text = "Number Of Used Hosts";
                        break;
                    case DrawItems.Migrations:
                        val = trials.Where(x => x.Size == trial.Size && x.Change == trial.Change).Max(x => x.Migrations);
                        mtrial = trials.Single(x => x.Size == trial.Size && x.Change == trial.Change && x.Migrations == val);

                        all[st].Add(trial.Size, trial.Migrations / mtrial.Migrations);
                        myPane.YAxis.Title.Text = "Migrations Count Ratio";
                        break;
                    case DrawItems.SlaViolations:
                        all[st].Add(trial.Size, trial.SlaViolations);
                        break;
                    case DrawItems.Messages:
                        val = trials.Where(x => x.Size == trial.Size && x.Change == trial.Change).Max(x => x.TotalMessages);

                        mtrial = trials.Single(x => x.Size == trial.Size && x.Change == trial.Change && x.TotalMessages == val);

                        myPane.YAxis.Title.Text = "Message Count Ratio";
                        all[st].Add(trial.Size, trial.TotalMessages/mtrial.TotalMessages);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(yAxis), yAxis, null);
                }
            }
            float d = 0;
            foreach (var a in all)
            {
                LineItem l = new LineItem
                    (a.Key==Strategies.WAshraf2017?"Proposed Algorithm":a.Key.ToString(),
                    a.Value, GetColor(a.Key), SymbolType.Square,3);
                l.Symbol.Size = 5;
                l.Symbol.Fill = new Fill(GetColor(a.Key));
                l.Line.Style = GetDash(a.Key);
                //l.Line.DashOn = d;
                //l.Line.DashOff = d*2;

                d++;
                myPane.CurveList.Add(l);
                
            }
            myPane.Legend.Position = LegendPos.BottomCenter;
            //myPane.Chart.Fill = new Fill(Color.White, Color.LightCyan, 360.0f);

            
            if (yAxis== DrawItems.Messages || yAxis == DrawItems.Power || yAxis == DrawItems.Migrations)
                myPane.YAxis.Scale.Format = "# %";
            else
            {
                myPane.YAxis.Scale.Format = "";
            }
            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zgc.AxisChange();
            zgc.Refresh();
            
        }

        private DashStyle GetDash(Strategies key)
        {
            switch (key)
            {
                case Strategies.WAshraf2017:
                    return DashStyle.Solid;
                    break;
                case Strategies.Zhao:
                    return DashStyle.Dash;
                    break;
                case Strategies.ForsmanPush:
                    return DashStyle.Dot;
                    break;
                case Strategies.ForsmanPull:
                    return DashStyle.DashDotDot;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }

        private Color GetColor(Strategies key)
        {
            switch (key)
            {
                case Strategies.WAshraf2017:
                    return Color.Red;
                    break;
                case Strategies.Zhao:
                    return Color.Blue;
                    break;
                case Strategies.ForsmanPush:
                    return Color.Green;
                    break;
                case Strategies.ForsmanPull:
                    return Color.Black;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }

        private List<Trial> GetValue()
        {
            SimulationEntities context = new SimulationEntities();
            var trials = context.Trials.Where(x => x.StartUtil == cb_StartUtil.Text && x.Change == cb_Change.Text).OrderBy(y => y.Size).ToList();
            return trials;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            zedGraphControl1.SaveAsBitmap();
        }
    }
}
