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
using Simulation.Measure;
using Test.Charting;
using Test.Database;

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
            cb_Item.DataSource = Enum.GetValues(typeof(FinalItems)).Cast<FinalItems>();
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
            var y = (FinalItems)Enum.Parse(typeof(FinalItems), cb_Item.SelectedValue.ToString());

            CreateGraph(zedGraphControl1, y, trials);
        }

        private void CreateGraph(ZedGraphControl zgc, FinalItems yAxis, List<TrialResult> trials)
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
                    case FinalItems.AverageEntropy:
                        all[st].Add(trial.Size, trial.Entropy);
                        myPane.YAxis.Scale.Min = 0.99;
                        myPane.YAxis.Scale.Max = 1.01;
                        break;
                    case FinalItems.Power:
                        var val = trials.Where(x => x.Size == trial.Size && x.Change == trial.Change).Max(x => x.Power);
                        var mtrial = trials.Single(x => x.Size == trial.Size && x.Change == trial.Change && x.Power == val);
                        all[st].Add(trial.Size, trial.Power / mtrial.Power);
                        myPane.YAxis.Title.Text = "Power Consumption Ratio";
                        break;
                    //case DrawItems.StdDev:
                    //    all[st].Add(trial.Size, trial.StdDev);
                    //    break;
                    case FinalItems.Hosts:
                        all[st].Add(trial.Size, trial.Hosts);
                        myPane.YAxis.Title.Text = "Number Of Used Hosts";
                        break;
                    case FinalItems.Migrations:
                        val = trials.Where(x => x.Size == trial.Size && x.Change == trial.Change).Max(x => x.Migrations);
                        mtrial = trials.Single(x => x.Size == trial.Size && x.Change == trial.Change && x.Migrations == val);

                        all[st].Add(trial.Size, trial.Migrations / mtrial.Migrations);
                        myPane.YAxis.Title.Text = "Migrations Count Ratio";
                        break;
                    case FinalItems.SlaViolations:
                        all[st].Add(trial.Size, trial.SlaViolations);
                        break;
                    case FinalItems.Messages:
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

            
            if (yAxis== FinalItems.Messages || yAxis == FinalItems.Power || yAxis == FinalItems.Migrations)
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
                case Strategies.Zhao:
                    return DashStyle.Dash;
                case Strategies.ForsmanPush:
                    return DashStyle.Dot;
                case Strategies.ForsmanPull:
                    return DashStyle.DashDotDot;
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
                case Strategies.Zhao:
                    return Color.Blue;
                case Strategies.ForsmanPush:
                    return Color.Green;
                case Strategies.ForsmanPull:
                    return Color.Black;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }

        private List<TrialResult> GetValue()
        {
            SimulationContext context = new SimulationContext();
            var trials = context.TrialResults
                .Where(x => x.StartUtil == cb_StartUtil.Text 
                && x.Change == cb_Change.Text)
                .OrderBy(y => y.Size).ToList();
            return trials;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            zedGraphControl1.SaveAsBitmap();
        }
    }
}
