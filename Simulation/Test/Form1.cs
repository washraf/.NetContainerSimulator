using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Simulation.Accounting;
using Simulation.Configuration;
using Simulation.DataCenter;
using Simulation.Helpers;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.SimulationController;
using ZedGraph;
using Simulation.Measure;
using Simulation.AccountingResults;
using Test.Charting;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cb_Strategy.DataSource = Enum.GetValues(typeof(Strategies)).Cast<Strategies>();
            cb_Strategy.SelectedIndex = 2;

            cb_Change.DataSource = Enum.GetValues(typeof(LoadChangeAction)).Cast<LoadChangeAction>();
            cb_Change.SelectedIndex = 0;

            cb_StartUtil.DataSource = Enum.GetValues(typeof(StartUtilizationPercent)).Cast<StartUtilizationPercent>();
            cb_StartUtil.SelectedIndex = 0;

            cb_GraphItem.DataSource = Enum.GetValues(typeof(BasicItems)).Cast<BasicItems>();
            cb_GraphItem.SelectedIndex = 0;

            cb_HostsCount.DataSource = Enum.GetValues(typeof(SimulationSize)).Cast<SimulationSize>();
            cb_HostsCount.SelectedIndex = 0;

            cb_Prediction.DataSource = Enum.GetValues(typeof(LoadPrediction)).Cast<LoadPrediction>();
            cb_Prediction.SelectedIndex = 1;


        }
        List<MeasureValueHolder> _measuredValueListsTrials = new List<MeasureValueHolder>();
        //AccountingModule _accountingModule = AccountingModule.GetAccountingModule();
        static IAccountingResultsManager accountingResultsManager = new AccountingResultsFileManager();

        private void btn_Start_Click(object sender, EventArgs e)
        {

            Thread t = new Thread(
                () =>
                {
                    var predictors = new List<LoadPrediction>()
                    {
                        LoadPrediction.None,
                        //LoadPrediction.Ewma,
                        //LoadPrediction.Arma,
                        //LoadPrediction.LinReg,
                    };

                    var sizes = new List<SimulationSize>()
                    {
                        //SimulationSize.Five,
                        //SimulationSize.Ten,
                        SimulationSize.Twenty,
                        SimulationSize.Fifty,
                        SimulationSize.Hundred,
                        SimulationSize.TwoHundred
                    };


                    var util = new List<StartUtilizationPercent>()
                    {
                        //StartUtilizationPercent.Thrity,
                        StartUtilizationPercent.Fifty,
                        StartUtilizationPercent.Seventy,
                    };

                    var algorithms = new List<Strategies>()
                    {
                       Strategies.Proposed2018,
                       Strategies.WAshraf2017,
                        //Strategies.Zhao,
                        //Strategies.ForsmanPush,
                        //Strategies.ForsmanPull,
                    };

                    var cactions = new List<LoadChangeAction>()
                    {
                        LoadChangeAction.Burst,
                        LoadChangeAction.Drain,
                        LoadChangeAction.Opposite
                    };
                    var testingRange = new List<TestedHosts>()
                    {
                        TestedHosts.One,
                        TestedHosts.Five,
                        TestedHosts.Ten,
                        TestedHosts.Twenty,
                    };


                    foreach (var u in util)
                    {
                        Global.StartUtilizationPercent = u;
                        foreach (var s in sizes)
                        {
                            Global.SetSimulationSize(s);
                            foreach (var action in cactions)
                            {
                                //if(action== LoadChangeAction.VeryHightBurst && u==StartUtilizationPercent.Seventy)
                                //    continue;
                                Global.ChangeAction = action;
                                foreach (var alg in algorithms)
                                {
                                    Global.SetCurrentStrategy(alg);
                                    foreach (var testing in testingRange)
                                    {
                                        Global.TestedItems = testing;
                                        #region --ALL--
                                        if (alg == Strategies.WAshraf2017 && testing != TestedHosts.One)
                                            continue;
                                        btn_Start.Invoke(new Action(() => { btn_Start.Enabled = false; }));
                                        List<MeasureValueHolder> internalValueListsTrials =
                                            new List<MeasureValueHolder>();
                                        //MeasureValueHolder holder = null;
                                        for (int i = 0; i < Global.NoOfTrials; i++)
                                        {
                                            //var stime = DateTime.Now;
                                            SimulationController controller =
                                                new SimulationController(Global.CurrentStrategy,
                                                    Global.SimulationSize,
                                                    Global.StartUtilizationPercent,
                                                    Global.LoadPrediction,
                                                    Global.ChangeAction,
                                                    Global.TestedItems,
                                                    ContainersType.D);
                                            controller.StartSimulation();
                                            //var etime = DateTime.Now;
                                            //MessageBox.Show((etime - stime).TotalSeconds.ToString());
                                            //Thread.Sleep(Global.GetSimulationTime);

                                            //controller.EndSimulation();
                                            //holder = controller.AccountingModuleObject.MeasureHolder;
                                            internalValueListsTrials.Add(controller.AccountingModuleObject.MeasureHolder);
                                            //await Task.Delay(5000);
                                            if (Global.NoOfTrials > 1)
                                            {
                                                accountingResultsManager.WriteDataToDisk(controller.AccountingModuleObject.MeasureHolder, i);
                                            }
                                            Thread.Sleep(5000);

                                        }
                                        //List<MeasuresValues> final = new List<MeasuresValues>();
                                        //MeasureValueHolder final = new MeasureValueHolder(Global.CurrentStrategy,
                                        //    Global.SimulationSize,
                                        //    Global.StartUtilizationPercent,
                                        //    Global.ChangeAction,
                                        //    Global.LoadPrediction);
                                        MeasureValueHolder final = internalValueListsTrials[0] / 1;
                                        foreach (var list in internalValueListsTrials.Skip(1))
                                        {
                                            final = final + list;
                                        }
                                        final = final / Global.NoOfTrials;
                                        accountingResultsManager.WriteDataToDisk(final, -1);

                                        btn_Start.Invoke(new Action(() =>
                                        {
                                            btn_Start.Enabled = true;
                                            AddDataHolder(final);
                                        }));

                                        #endregion
                                    }
                                }
                            }
                        }
                    }
                }
                );
            t.Priority = ThreadPriority.Highest;
            t.Start();
        }

        private void AddDataHolder(MeasureValueHolder holder)
        {
            var dbHelper = new DatabaseTrialResultManagement();
            dbHelper.Save(holder);
            _measuredValueListsTrials.Add(holder);
            var btn = new Button()
            {
                Text = holder.Name,
                Width = 200
            };
            btn.Click += Btn_Click;
            flowLayoutPanel1.Controls.Add(btn);
            CreateGraph(zedGraphControl1, cb_GraphItem.Text, (BasicItems)cb_GraphItem.SelectedValue);
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            for (int i = 0; i < _measuredValueListsTrials.Count; i++)
            {
                if (_measuredValueListsTrials[i].Name == btn.Text)
                {
                    _measuredValueListsTrials.RemoveAt(i);
                    flowLayoutPanel2.Controls.RemoveAt(i);
                    flowLayoutPanel1.Controls.RemoveAt(i);
                    break;
                }
            }
            CreateGraph(zedGraphControl1, cb_GraphItem.Text, (BasicItems)cb_GraphItem.SelectedValue);

        }
        // Build the Chart
        private void CreateGraph(ZedGraphControl zgc, string title, BasicItems yAxis)
        {
            // get a reference to the GraphPane
            GraphPane myPane = zgc.GraphPane;

            int unit = Global.AccountTime / Global.Second;
            myPane.CurveList.Clear();
            // Set the Titles
            myPane.Title.Text = title;
            myPane.XAxis.Title.Text = $"Time Unit is Second";
            myPane.YAxis.Title.Text = yAxis.ToString();
            flowLayoutPanel2.Controls.Clear();
            for (int t = 0; t < _measuredValueListsTrials.Count; t++)
            {
                List<PointPairList> list = new List<PointPairList>();
                list.Add(new PointPairList());

                List<string> labeList = new List<string>();
                labeList.Add($"{yAxis.ToString()} of Trial {_measuredValueListsTrials[t].Name}");
                //var measuredValueTrail = _measuredValueListsTrials[t];
                System.Windows.Forms.Label myLabel = new System.Windows.Forms.Label();
                myLabel.Width = 400;
                myLabel.BackColor = GetColor(t);
                switch (yAxis)
                {
                    case BasicItems.Entropy:

                        if (cb_DrawExtra.Checked)
                        {
                            list.Add(new PointPairList());
                            labeList.Add($"Predicted Entropy of Trial {_measuredValueListsTrials[t].Name}");
                        }
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].Entropy);
                            if (cb_DrawExtra.Checked)
                            {
                                list[1].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].PredictedEntropy);
                            }

                        }
                        myLabel.Text = $"Avegare entropy in All the trial = {_measuredValueListsTrials[t].AverageEntropy}";
                        break;
                    case BasicItems.PowerConsumption:
                        list[0].Add(0, _measuredValueListsTrials[t].PowerConsumption);
                        myLabel.Text = $"Power Consumption in All the trial = {_measuredValueListsTrials[t].PowerConsumption} Watts";

                        break;
                    case BasicItems.AverageNeededVolume:
                        if (cb_DrawExtra.Checked)
                        {
                            list.Add(new PointPairList());
                            labeList.Add($"Average Predicted Volume of Trial {_measuredValueListsTrials[t].Name}");
                            list.Add(new PointPairList());
                            labeList.Add($"Min Needed Volume of Trial {_measuredValueListsTrials[t].Name}");
                            list.Add(new PointPairList());
                            labeList.Add($"Max Needed Volume of Trial {_measuredValueListsTrials[t].Name}");
                        }
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].AvgNeededVolume);
                            if (cb_DrawExtra.Checked)
                            {
                                list[1].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].AvgPredictedVolume);
                                list[2].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].MinNeeded);
                                list[3].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].MaxNeeded);
                            }
                        }
                        myLabel.Text = $"Avegare Volume in All the trial vs Average predicted volume";

                        break;

                    case BasicItems.NeededLoadStandardDeviation:
                        for (int i = 0; i < _measuredValueListsTrials[t].LoadMeasureValueList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].LoadMeasureValueList[i].StandardDeviation);
                        }
                        myLabel.Text = $"Average Standard Deviation {_measuredValueListsTrials[t].AverageStdDeviation}";
                        break;
                    case BasicItems.NoHosts:
                        if (cb_DrawExtra.Checked)
                        {
                            list.Add(new PointPairList());
                            labeList.Add($"Ideal Hosts Count of Trial {_measuredValueListsTrials[t].Name}");
                        }
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].NoHosts);
                            if (cb_DrawExtra.Checked)
                            {
                                list[1].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].IdealHostsCount);
                            }
                        }
                        myLabel.Text = $"Average No of Hosts {_measuredValueListsTrials[t].AverageHosts}";

                        break;
                    case BasicItems.HostsStates:
                        //list.Clear()
                        list.Add(new PointPairList());
                        list.Add(new PointPairList());
                        list.Add(new PointPairList());
                        labeList.Clear();
                        labeList.Add($"Under of {_measuredValueListsTrials[t].Name}");
                        labeList.Add($"Over of {_measuredValueListsTrials[t].Name}");
                        labeList.Add($"Normal of {_measuredValueListsTrials[t].Name}");
                        labeList.Add($"Evacuating of {_measuredValueListsTrials[t].Name}");

                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].UnderHosts);
                            list[1].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].OverHosts);
                            list[2].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].NormalHosts);
                            list[3].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].EvacuatingHosts);
                        }
                        myLabel.Text = $"Under:{_measuredValueListsTrials[t].FinalUnderUtilized} " +
                            $"Over:{_measuredValueListsTrials[t].FinalOverUtilized} " +
                            $"Normal:{_measuredValueListsTrials[t].FinalNormalUtilized} " +
                            $"Evacuating:{_measuredValueListsTrials[t].FinalEvacuatingUtilized} ";

                        break;
                    case BasicItems.TotalMessages:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].TotalMessages);
                        }
                        myLabel.Text = $"Total Messages in All the trial = {_measuredValueListsTrials[t].TotalMessages}";

                        break;
                    case BasicItems.PushRequests:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].PushRequests);
                        }
                        myLabel.Text = $"Total Push Requests in All the trial = {_measuredValueListsTrials[t].TotalPushRequests}";

                        break;
                    case BasicItems.PullRequests:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].PullRequests);
                        }
                        myLabel.Text = $"Total Pull Requests in All the trial = {_measuredValueListsTrials[t].TotalPullRequests}";

                        break;

                    case BasicItems.Migrations:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].Migrations);
                        }

                        myLabel.Text = $"Total Migrations in All the trial = {_measuredValueListsTrials[t].TotalMigrations}";

                        break;
                    case BasicItems.MigrationCount:
                        foreach (var item in _measuredValueListsTrials[t].ContainerMigrationCount)
                        {
                            list[0].Add(item.Key, item.Value.MigrationCount);
                        }
                        myLabel.Text = $"Avarage Migration per container in All the trial = {_measuredValueListsTrials[t].AvgMigrations}";

                        break;
                    case BasicItems.PushLoadAvailabilityRequest:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit,
                                _measuredValueListsTrials[t].MeasuredValuesList[i].PushLoadAvailabilityRequest);
                        }
                        myLabel.Text = $"Total Push Availability Requests in All the trial = {_measuredValueListsTrials[t].TotalPushAvailabilityRequests}";

                        break;
                    case BasicItems.PullLoadAvailabilityRequest:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit,
                                _measuredValueListsTrials[t].MeasuredValuesList[i].PullLoadAvailabilityRequest);
                        }
                        myLabel.Text = $"Total Pull Availability Requests in All the trial = {_measuredValueListsTrials[t].TotalPullAvailabilityRequests}";

                        break;
                    case BasicItems.UtilizationSlaViolations:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit,
                                _measuredValueListsTrials[t].MeasuredValuesList[i].SlaViolations);
                        }
                        myLabel.Text = $"Total SLA Violations the trial = {_measuredValueListsTrials[t].TotalSlaViolations}";

                        break;
                    case BasicItems.ContainerDownTime:
                        foreach (var item in _measuredValueListsTrials[t].ContainerMigrationCount)
                        {
                            list[0].Add(item.Key, item.Value.Downtime);
                        }
                        myLabel.Text = $"Average Down time of all containers in All the trial = {_measuredValueListsTrials[t].AvgDownTime}";

                        break;

                    case BasicItems.ImagePulls:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit,
                                _measuredValueListsTrials[t].MeasuredValuesList[i].ImagePulls);
                        }
                        myLabel.Text = $"Total ImagePulls of trial = {_measuredValueListsTrials[t].ImagePulls}";
                        break;
                    case BasicItems.ImagePullsRatio:
                        list[0].Add(0, _measuredValueListsTrials[t].AveragePullPerImage);
                        myLabel.Text = $"Image Pull Ratio in All the trial = {_measuredValueListsTrials[t].AveragePullPerImage}";
                        break;
                    default:
                        throw new NotImplementedException();
                }
                // Generate a red curve with diamond
                // symbols, and "Porsche" in the legend
                if (yAxis == BasicItems.MigrationCount || yAxis == BasicItems.ContainerDownTime || yAxis == BasicItems.PowerConsumption || yAxis == BasicItems.ImagePullsRatio)// || yAxis == BasicItems.OutOfBoundHosts)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        myPane.AddBar(labeList[i], list[i], GetColor(t));
                    }
                }
                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        myPane.AddCurve(labeList[i], list[i], GetColor(t), GetSymbolType(i));
                    }
                }
                flowLayoutPanel2.Controls.Add(myLabel);




                //myPane.AddBar($"Trial no {t}", list1, GetColor(t));
                //myPane.AddStick($"Trial no {t}", list1,GetColor(t));
            }
            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zgc.AxisChange();
            zgc.Refresh();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Simulation.Program.EndSimulation();
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
                    return Color.Yellow;
                case 4:
                    return Color.DarkMagenta;
                case 5:
                    return Color.BurlyWood;
                default:
                    return Color.DarkOrange;

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
        private void cb_Strategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Global.SetCurrentStrategy((Strategies)cb_Strategy.SelectedValue);
                if (Global.CurrentStrategy == Strategies.WAshraf2017 || Global.CurrentStrategy == Strategies.WAshraf2017Auction)
                    cb_Prediction.SelectedIndex = 3;
                else if (Global.CurrentStrategy == Strategies.Zhao)
                {
                    cb_Prediction.SelectedIndex = 0;
                }
                else
                {
                    cb_Prediction.SelectedIndex = 1;
                }
            }
            catch (Exception)
            {
            }

        }


        private void cb_StartUtil_SelectedIndexChanged(object sender, EventArgs e)
        {
            Global.StartUtilizationPercent = (StartUtilizationPercent)cb_StartUtil.SelectedValue;
        }

        private void cb_GraphItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateGraph(zedGraphControl1, cb_GraphItem.Text, (BasicItems)cb_GraphItem.SelectedValue);
        }

        private void cb_HostsCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            Global.SetSimulationSize((SimulationSize)cb_HostsCount.SelectedValue);
        }

        private void cb_FirstWave_SelectedIndexChanged(object sender, EventArgs e)
        {
            Global.ChangeAction = (LoadChangeAction)cb_Change.SelectedValue;
        }

        private void cb_Prediction_SelectedIndexChanged(object sender, EventArgs e)
        {
            Global.LoadPrediction = (LoadPrediction)cb_Prediction.SelectedValue;
        }

        private void btn_AddData_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            var result = f.ShowDialog();
            if (result == DialogResult.OK)
            {
                var holder = accountingResultsManager.ReadDataFromDisk(f.FileName);
                AddDataHolder(holder);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show(this);
            f.FormClosing += (a, b) =>
            {
                f.Owner?.Show();
            };
            this.Hide();
        }
    }
}
