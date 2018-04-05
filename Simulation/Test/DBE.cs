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
    public partial class DBE : Form
    {
        public DBE()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            var LoadChangeActionItems = 
                Enum.GetValues(typeof(LoadChangeAction)).Cast<LoadChangeAction>().Select(x=>x.ToString()).ToList();
            LoadChangeActionItems.Remove("None");
            LoadChangeActionItems.Add("All");
            cb_Change.DataSource = LoadChangeActionItems;
            cb_Change.SelectedIndex = 0;

            var startutilizationitems = Enum.GetValues(typeof(StartUtilizationPercent)).Cast<StartUtilizationPercent>().Select(x => x.ToString()).ToList();
            startutilizationitems.Add("All");

            cb_StartUtil.DataSource = startutilizationitems;
            cb_StartUtil.SelectedIndex = 0;


            var sizes = new List<string>() { "20", "50", "100", "200" };
            sizes.Add("All");
            cb_Size.DataSource = sizes;
            cb_Size.SelectedIndex = 0;
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

            CreateGraph(zedGraphControl1, trials);
        }
        public class DictionaryHolder {
            public int Count { get; set; }
            public double Total { get; set; }
        }

        private void CreateGraph(ZedGraphControl zgc, List<TrialResult> mainTrials)
        { 
            // get a reference to the GraphPane
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();
            myPane.YAxis.Scale.MinAuto = true;
            myPane.YAxis.Scale.MaxAuto = true;

            // Set the Titles
            myPane.Title.Text = "DBE";
            myPane.XAxis.Title.Text = $"Algorithms";
            myPane.YAxis.Title.Text = "DBE";

            Dictionary<string, DictionaryHolder> all 
                = new Dictionary<string, DictionaryHolder>();

            foreach (var trial in mainTrials)
            {
                var trials = mainTrials.Where(x =>
                x.Size == trial.Size &&
                x.StartUtil == trial.StartUtil &&
                x.Change == trial.Change);

                var maxDBE = new DBEHolder
                {
                    Power = trials.Max(x => x.Power),
                    Hosts = trials.Max(x => x.Hosts),
                    RMSE = trials.Max(x => x.RMSE),
                    Migrations = trials.Max(x => x.Migrations),
                    Messages = trials.Max(x => x.TotalMessages),
                    Entropy = trials.Max(x => x.AverageEntropy),
                    SLA = trials.Max(x => x.SlaViolations),
                };

                var currentDBE = new DBEHolder
                {
                    Power = trial.Power,
                    Hosts = trial.Hosts,
                    RMSE = trial.RMSE,
                    Migrations = trial.Migrations,
                    Messages = trial.TotalMessages,
                    Entropy = trial.AverageEntropy,
                    SLA = trial.SlaViolations,
                };

                var result = DBEHolder.GetDBE(currentDBE, maxDBE);
                if (!all.ContainsKey(trial.Algorithm))
                {
                    all.Add(trial.Algorithm, new DictionaryHolder { Count = 1, Total = result });
                }
                else
                {
                    all[trial.Algorithm].Count++;
                    all[trial.Algorithm].Total += result;
                }
            }

            var list = from row in all select new { Algorithm = row.Key, DBE = (row.Value.Total/row.Value.Count).ToString("#.###") };
            dataGridView1.DataSource = list.ToArray();

            foreach (var a in all)
            {
                var st = (Strategies) Enum.Parse(typeof(Strategies), a.Key.Split('_')[0]);
                TestedHosts t = TestedHosts.All;
                myPane.AddBar(a.Key == "WAshraf2017" ? "Proposed Algorithm" : a.Key.ToString()
                    , new PointPairList() {
                    new PointPair(0,a.Value.Total/a.Value.Count)}, GetColor(st, t));
            }

            myPane.Legend.Position = LegendPos.BottomCenter;
            //myPane.YAxis.Scale.Format = "#.### %";
            myPane.XAxis.Type = AxisType.Text;
            zgc.AxisChange();
            zgc.Refresh();

        }

        private DashStyle GetDash(Strategies key, TestedHosts tested)
        {
            switch (key)
            {
                case Strategies.Proposed2018:
                    return DashStyle.Solid;
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

        private Color GetColor(Strategies key, TestedHosts tested)
        {
            switch (key)
            {
                case Strategies.Proposed2018:
                    switch (tested)
                    {
                        case TestedHosts.Ten:
                            return Color.Indigo;
                        case TestedHosts.Twenty:
                            return Color.CadetBlue;
                        case TestedHosts.All:
                            return Color.DarkViolet;
                        default:
                            throw new NotImplementedException();
                    }
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
            var trials = context.TrialResults.AsQueryable();
            if (cb_StartUtil.Text != "All")
                trials = trials.Where(x => x.StartUtil == cb_StartUtil.Text);
            if (cb_Change.Text != "All")
                trials = trials.Where(x => x.Change == cb_Change.Text);
            if (cb_Size.Text != "All")
            {
                int t = int.Parse(cb_Size.Text);
                trials = trials.Where(x => x.Size == t);

            }

            var  ftrials = trials
                .ToList()
                //.GroupBy(x => new { x.Algorithm})
                //.Select(x => new TrialResult()
                //{
                //    Algorithm = x.Key.Algorithm,
                //    AverageEntropy = x.Average(y => y.AverageEntropy),
                //    Change = x.First().Change,
                //    ContainersAverage = x.Average(y => y.ContainersAverage),
                //    FinalEntropy = x.Average(y => y.FinalEntropy),
                //    Hosts = x.Average(y => y.Hosts),
                //    ImagePullsRatio = x.Average(y => y.ImagePullsRatio),
                //    ImagePullsTotal = x.Average(y => y.ImagePullsTotal),
                //    Migrations = x.Average(y => y.Migrations),
                //    Power = x.Average(y => y.Power),
                //    PredictionAlg = x.First().PredictionAlg,
                //    RMSE = x.Average(y => y.RMSE),
                //    SchedulingAlgorithm = x.First().SchedulingAlgorithm,
                //    Size = x.First().Size,
                //    SlaViolations = x.Average(y => y.SlaViolations),
                //    StartUtil = x.First().StartUtil,
                //    StdDev = x.Average(y => y.StdDev),
                //    Tested = x.First().Tested,
                //    TotalContainers = x.Average(y => y.TotalContainers),
                //    TotalMessages = x.Average(y => y.TotalMessages),
                //    TrialId = 0,
                //})
                .OrderBy(y => y.Size).ToList();
            return ftrials;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            zedGraphControl1.SaveAsBitmap();
        }
    }
}
