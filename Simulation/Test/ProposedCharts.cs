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
    public partial class ProposedCharts : Form
    {
        public ProposedCharts()
        {
            InitializeComponent();
        }

        private void ProposedCharts_Load(object sender, EventArgs e)
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
            myPane.Title.Text = yAxis.ToString().Replace('_', ' ');
            myPane.XAxis.Title.Text = $"Host Selection Policy";
            myPane.YAxis.Title.Text = yAxis.ToString().Replace('_', ' ');
            Dictionary<string, PointPairList> all = new Dictionary<string, PointPairList>();
            //all.Add(Strategies.Proposed2018+"_"+1, new PointPairList());
            //all.Add(Strategies.Proposed2018 + "_" + 5, new PointPairList());
            //all.Add(Strategies.Proposed2018 + "_" + 10, new PointPairList());
            //all.Add(Strategies.Proposed2018 + "_" + 20, new PointPairList());
            all.Add(AuctionTypes.MostFull.ToString(), new PointPairList());
            all.Add(AuctionTypes.LeastFull.ToString(), new PointPairList());
            all.Add(AuctionTypes.Random.ToString(), new PointPairList());
            all.Add(AuctionTypes.LeastPulls.ToString(), new PointPairList());


            foreach (var trial in trials)
            {
                var st = (AuctionTypes)Enum.Parse(typeof(AuctionTypes), trial.PushAuctionType);


                //trials.Single(
                //    x => x.Size == trial.Size && x.Algorithm == Strategies.InOrderProping.ToString());
                var k = st.ToString();
                int tested = trial.TestedPercent;
                switch (yAxis)
                {

                    case FinalItems.Hosts:
                        all[k].Add(tested, trial.Hosts);
                        //all[k].Add(trial.Tested, trial.Hosts);
                        myPane.YAxis.Title.Text = "Number Of Used Hosts";
                        break;
                    case FinalItems.RMSE:
                        all[k].Add(tested, trial.RMSE);
                        myPane.YAxis.Title.Text = "RMSE";
                        break;
                    case FinalItems.RMSE_Ratio:
                        var rmseval = trials.Where(x => x.Size == trial.Size && x.Change == trial.Change).Max(x => x.RMSE);
                        var rmseMaxtrial = trials.First(x => x.Size == trial.Size && x.Change == trial.Change && x.RMSE == rmseval);
                        all[k].Add(tested, trial.RMSE / rmseMaxtrial.RMSE);
                        myPane.YAxis.Title.Text = "RMSE Ratio";
                        break;
                    case FinalItems.Power:
                        all[k].Add(tested, trial.Power);
                        myPane.YAxis.Title.Text = "Power Consumption";
                        break;
                    //case DrawItems.StdDev:
                    //    all[st].Add(trial.Size, trial.StdDev);
                    //    break;
                    case FinalItems.Average_Entropy:
                        all[k].Add(tested, trial.AverageEntropy);
                        myPane.YAxis.Scale.Min = 0.99;
                        myPane.YAxis.Scale.Max = 1.01;
                        break;
                    case FinalItems.Average_Entropy_Ratio:
                        var AEval = trials.Where(x => x.Size == trial.Size && x.Change == trial.Change).Max(x => x.AverageEntropy);
                        var AEMaxtrial = trials.First(x => x.Size == trial.Size && x.Change == trial.Change && x.AverageEntropy == AEval);
                        all[k].Add(tested, trial.AverageEntropy / AEMaxtrial.AverageEntropy);
                        myPane.YAxis.Title.Text = "Average Entropy Ratio";
                        break;
                    case FinalItems.Final_Entropy:
                        all[k].Add(tested, trial.FinalEntropy);
                        myPane.YAxis.Scale.Min = 0.99;
                        myPane.YAxis.Scale.Max = 1.01;
                        break;
                    case FinalItems.Migrations:
                        all[k].Add(tested, trial.Migrations);
                        myPane.YAxis.Title.Text = "Migrations Count Ratio";
                        break;
                    case FinalItems.SLA_Violations_Count:
                        all[k].Add(tested, trial.SlaViolations);
                        break;
                    case FinalItems.SLA_Violations_Percent:
                        all[k].Add(tested, trial.SlaViolationsPercent);
                        break;
                    case FinalItems.Messages:
                        myPane.YAxis.Title.Text = "Message Count";
                        all[k].Add(tested, trial.TotalMessages);
                        break;
                    case FinalItems.Total_Image_Pulls:
                        all[k].Add(tested, trial.ImagePullsTotal);
                        myPane.YAxis.Title.Text = "Total Image Pulls";
                        break;
                    case FinalItems.Average_Pulls_PerImage:
                        all[k].Add(tested, trial.ImagePullsRatio);
                        myPane.YAxis.Title.Text = "Average Pulls PerImage";
                        break;
                    case FinalItems.Containers_Average:
                        all[k].Add(tested, trial.ContainersAverage);
                        myPane.YAxis.Title.Text = "Containers Average";
                        break;
                    case FinalItems.Average_Down_Time:
                        all[k].Add(tested, trial.AverageDownTime);
                        myPane.YAxis.Title.Text = "Average DownTime";
                        break;
                    case FinalItems.Container_Density:
                        all[k].Add(tested, trial.AverageContainerPerHost);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(yAxis), yAxis, null);
                }
            }
            foreach (var a in all)
            {
                var st = (AuctionTypes)Enum.Parse(typeof(AuctionTypes), a.Key);
                myPane.AddBar(a.Key, null, a.Value.Select(x => x.Y).ToArray(), GetColor(st));
            }
            myPane.XAxis.MajorTic.IsBetweenLabels = true;
            myPane.XAxis.Scale.TextLabels = trials.Select(x => x.TestedPercent.ToString()).Distinct().ToArray();
            myPane.XAxis.Type = AxisType.Text;
            // Tell ZedGraph to reconfigure the
            // axes since the data have changed
            zgc.AxisChange();
            zgc.Refresh();

        }

        private DashStyle GetDash(AuctionTypes key)
        {
            switch (key)
            {
                case AuctionTypes.LeastFull:
                    return DashStyle.Solid;
                case AuctionTypes.MostFull:
                    return DashStyle.Solid;
                case AuctionTypes.Random:
                    return DashStyle.Dash;
                case AuctionTypes.LeastPulls:
                    return DashStyle.Dot;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }

        private Color GetColor(AuctionTypes auctionTypes)
        {
            switch (auctionTypes)
            {
                case AuctionTypes.LeastFull:
                    return Color.Red;
                case AuctionTypes.MostFull:
                    return Color.Yellow;
                case AuctionTypes.Random:
                    return Color.Blue;
                case AuctionTypes.LeastPulls:
                    return Color.Green;
                default:
                    throw new ArgumentOutOfRangeException(nameof(auctionTypes), auctionTypes, null);
            }
        }

        private List<TrialResult> GetValue()
        {
            SimulationContext context = new SimulationContext();
            var trials = context.TrialResults.AsQueryable();
            trials = trials.Where(x => x.Algorithm == "Proposed2018" & x.Size == 200);
            if (cb_StartUtil.Text != "All")
                trials = trials.Where(x => x.StartUtil == cb_StartUtil.Text);
            if (cb_Change.Text != "All")
                trials = trials.Where(x => x.Change == cb_Change.Text);

           var  ftrials = trials
                .ToList()
                .GroupBy(x => new { x.PushAuctionType,x.PullAuctionType,x.TestedPercent})
                .Select(x => new TrialResult()
                {
                    Algorithm = x.First().Algorithm,
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
                    TestedPercent = x.First().TestedPercent,
                    TotalContainers = x.Average(y => y.TotalContainers),
                    TotalMessages = x.Average(y => y.TotalMessages),
                    TotalCommunicatedData = x.Average(y=>y.TotalCommunicatedData),
                    TrialId = 0,
                    AverageContainerPerHost = x.Average(y=>y.AverageContainerPerHost),
                    PushAuctionType = x.Key.PushAuctionType,
                    PullAuctionType = x.Key.PullAuctionType,
                    AverageDownTime = x.Average(y=>y.AverageDownTime),
                    SlaViolationsPercent = x.Average(y=>y.SlaViolationsPercent),
                }).OrderBy(y => y.TestedPercent).ToList();
            return ftrials;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            zedGraphControl1.SaveAsBitmap();
        }
    }
}
