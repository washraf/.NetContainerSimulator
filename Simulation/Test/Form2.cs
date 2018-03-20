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
            var LoadChangeActionItems = Enum.GetValues(typeof(LoadChangeAction)).Cast<LoadChangeAction>().Select(x=>x.ToString()).ToList();
            LoadChangeActionItems.Add("All");
            cb_Change.DataSource = LoadChangeActionItems;
            cb_Change.SelectedIndex = 0;

            var startutilizationitems = Enum.GetValues(typeof(StartUtilizationPercent)).Cast<StartUtilizationPercent>().Select(x => x.ToString()).ToList();
            startutilizationitems.Add("All");

            cb_StartUtil.DataSource = startutilizationitems;
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
        struct TrialStruct
        {
            public TrialStruct(Strategies strategies, int tested)
            {
                Strategies = strategies;
                this.Tested = tested;
            }
            public Strategies Strategies { get; }
            public int Tested { get; }
            public override string ToString()
            {
                return Strategies.ToString() + "_" + Tested;
            }
        }
        private void CreateGraph(ZedGraphControl zgc, FinalItems yAxis, List<TrialResult> trials)
        {
            // get a reference to the GraphPane
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();
            myPane.YAxis.Scale.MinAuto = true;
            myPane.YAxis.Scale.MaxAuto = true;

            // Set the Titles
            myPane.Title.Text = yAxis.ToString().Replace('_',' ');
            myPane.XAxis.Title.Text = $"Number of Hosts (N)";
            myPane.YAxis.Title.Text = yAxis.ToString().Replace('_', ' ');
            Dictionary<string, PointPairList> all = new Dictionary<string, PointPairList>();
            //all.Add(Strategies.Proposed2018+"_"+1, new PointPairList());
            //all.Add(Strategies.Proposed2018 + "_" + 5, new PointPairList());
            //all.Add(Strategies.Proposed2018 + "_" + 10, new PointPairList());
            //all.Add(Strategies.Proposed2018 + "_" + 20, new PointPairList());
            all.Add(Strategies.WAshraf2017.ToString(),new PointPairList());
            all.Add(Strategies.Zhao.ToString(), new PointPairList());
            all.Add(Strategies.ForsmanPush.ToString(), new PointPairList());
            all.Add(Strategies.ForsmanPull.ToString(), new PointPairList());
            

            foreach (var trial in trials)
            {
                var st = (Strategies) Enum.Parse(typeof (Strategies), trial.Algorithm);


                //trials.Single(
                //    x => x.Size == trial.Size && x.Algorithm == Strategies.InOrderProping.ToString());
                var k = st == Strategies.Proposed2018 ? Strategies.Proposed2018 + "_" + trial.Tested : st.ToString();
                switch (yAxis)
                {
                    
                    case FinalItems.Hosts:
                        all[k].Add(trial.Size, trial.Hosts);
                        myPane.YAxis.Title.Text = "Number Of Used Hosts";
                        break;
                    case FinalItems.RMSE:
                        all[k].Add(trial.Size, trial.RMSE);
                        myPane.YAxis.Title.Text = "RMSE";
                        break;
                    case FinalItems.RMSE_Ratio:
                        var rmseval = trials.Where(x => x.Size == trial.Size && x.Change == trial.Change).Max(x => x.RMSE);
                        var rmseMaxtrial = trials.First(x => x.Size == trial.Size && x.Change == trial.Change && x.RMSE == rmseval);
                        all[k].Add(trial.Size, trial.RMSE/rmseMaxtrial.RMSE);
                        myPane.YAxis.Title.Text = "RMSE Ratio";
                        break;
                    case FinalItems.Power:
                        var val = trials.Where(x => x.Size == trial.Size && x.Change == trial.Change).Max(x => x.Power);
                        var mtrial = trials.Single(x => x.Size == trial.Size && x.Change == trial.Change && x.Power == val);
                        all[k].Add(trial.Size, trial.Power / mtrial.Power);
                        myPane.YAxis.Title.Text = "Power Consumption Ratio";
                        break;
                    //case DrawItems.StdDev:
                    //    all[st].Add(trial.Size, trial.StdDev);
                    //    break;
                    case FinalItems.Average_Entropy:
                        all[k].Add(trial.Size, trial.AverageEntropy);
                        myPane.YAxis.Scale.Min = 0.99;
                        myPane.YAxis.Scale.Max = 1.01;
                        break;
                    case FinalItems.Average_Entropy_Ratio:
                        var AEval = trials.Where(x => x.Size == trial.Size && x.Change == trial.Change).Max(x => x.AverageEntropy);
                        var AEMaxtrial = trials.First(x => x.Size == trial.Size && x.Change == trial.Change && x.AverageEntropy == AEval);
                        all[k].Add(trial.Size, trial.AverageEntropy / AEMaxtrial.AverageEntropy);
                        myPane.YAxis.Title.Text = "Average Entropy Ratio";
                        break;
                    case FinalItems.Final_Entropy:
                        all[k].Add(trial.Size, trial.FinalEntropy);
                        myPane.YAxis.Scale.Min = 0.99;
                        myPane.YAxis.Scale.Max = 1.01;
                        break;
                    case FinalItems.Migrations:
                        val = trials.Where(x => x.Size == trial.Size && x.Change == trial.Change).Max(x => x.Migrations);
                        mtrial = trials.First(x => x.Size == trial.Size && x.Change == trial.Change && x.Migrations == val);

                        all[k].Add(trial.Size, trial.Migrations / mtrial.Migrations);
                        myPane.YAxis.Title.Text = "Migrations Count Ratio";
                        break;
                    case FinalItems.SLA_Violations:
                        all[k].Add(trial.Size, trial.SlaViolations);
                        break;
                    case FinalItems.Messages:
                        val = trials.Where(x => x.Size == trial.Size && x.Change == trial.Change).Max(x => x.TotalMessages);

                        mtrial = trials.Single(x => x.Size == trial.Size && x.Change == trial.Change && x.TotalMessages == val);

                        myPane.YAxis.Title.Text = "Message Count Ratio";
                        all[k].Add(trial.Size, trial.TotalMessages / mtrial.TotalMessages);
                        break;
                    case FinalItems.Total_Image_Pulls:
                        all[k].Add(trial.Size, trial.ImagePullsTotal);
                        myPane.YAxis.Title.Text = "Total Image Pulls";
                        break;
                    case FinalItems.Average_Pulls_PerImage:
                        all[k].Add(trial.Size, trial.ImagePullsRatio);
                        myPane.YAxis.Title.Text = "Average Pulls PerImage";
                        break;
                    case FinalItems.Containers_Average:
                        all[k].Add(trial.Size, trial.ContainersAverage);
                        myPane.YAxis.Title.Text = "Containers Average";
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException(nameof(yAxis), yAxis, null);
                }
            }
            float d = 0;
            foreach (var a in all)
            {
                var st = (Strategies) Enum.Parse(typeof(Strategies), a.Key.Split('_')[0]);
                TestedHosts t = TestedHosts.Infinity;
                if (st == Strategies.Proposed2018)
                    t = (TestedHosts)int.Parse(a.Key.Split('_')[1]);
                
                LineItem l = new LineItem
                    (a.Key == "WAshraf2017" ? "Proposed Algorithm" : a.Key.ToString(),
                    a.Value, GetColor(st,t), SymbolType.Square,3);
                l.Symbol.Size = 5;
                l.Symbol.Fill = new Fill(GetColor(st,t));
                l.Line.Style = GetDash(st,t);
                //l.Line.DashOn = d;
                //l.Line.DashOff = d*2;

                d++;
                myPane.CurveList.Add(l);
                
            }
            myPane.Legend.Position = LegendPos.BottomCenter;
            //myPane.Chart.Fill = new Fill(Color.White, Color.LightCyan, 360.0f);

            
            if (yAxis== FinalItems.Messages || 
                yAxis == FinalItems.Power || 
                yAxis == FinalItems.Migrations || 
                yAxis == FinalItems.RMSE_Ratio ||
                yAxis == FinalItems.Average_Entropy_Ratio)
                myPane.YAxis.Scale.Format = "#.### %";
            else
            {
                myPane.YAxis.Scale.Format = "";
            }
            // Tell ZedGraph to reconfigure the
            // axes since the data have changed
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
                        case TestedHosts.Infinity:
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

           var  ftrials = trials
                .ToList()
                .GroupBy(x => new { x.Algorithm, x.Size})
                .Select(x => new TrialResult()
                {
                    Algorithm = x.Key.Algorithm,
                    AverageEntropy = x.Average(y => y.AverageEntropy),
                    Change = x.First().Change,
                    ContainersAverage = x.Average(y => y.ContainersAverage),
                    FinalEntropy = x.Average(y => y.FinalEntropy),
                    Hosts = x.Average(y => y.Hosts),
                    ImagePullsRatio = x.Average(y => y.ImagePullsRatio),
                    ImagePullsTotal = x.Average(y => y.ImagePullsTotal),
                    Migrations = x.Average(y => y.Migrations),
                    Power = x.Average(y => y.Power),
                    PredictionAlg = x.First().PredictionAlg,
                    RMSE = x.Average(y => y.RMSE),
                    SchedulingAlgorithm = x.First().SchedulingAlgorithm,
                    Size = x.First().Size,
                    SlaViolations = x.Average(y => y.SlaViolations),
                    StartUtil = x.First().StartUtil,
                    StdDev = x.Average(y => y.StdDev),
                    Tested = x.First().Tested,
                    TotalContainers = x.Average(y => y.TotalContainers),
                    TotalMessages = x.Average(y => y.TotalMessages),
                    TrialId = 0,
                }).OrderBy(y => y.Size).ToList();
            return ftrials;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            zedGraphControl1.SaveAsBitmap();
        }
    }
}
