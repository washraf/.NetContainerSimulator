﻿using System;
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
            cb_GraphItem.DataSource = Enum.GetValues(typeof(BasicItems)).Cast<BasicItems>();
            cb_GraphItem.SelectedIndex = 0;
        }
        List<MeasureValueHolder> _measuredValueListsTrials = new List<MeasureValueHolder>();
        //AccountingModule _accountingModule = AccountingModule.GetAccountingModule();
        static IAccountingResultsManager accountingResultsManager = new AccountingResultsFileManager();
        private void btn_Start_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(
                () =>
                {
                    int x = 0;
                    foreach (var configuration in RunConfigurationFactory.GetConfigurations())
                    {
                        btn_Start.Invoke(new Action(() => { btn_Start.Enabled = false; }));
                        Global.UpdateTime(configuration.SimulationSize, configuration.Strategy);
                        SimulationController controller = new SimulationController(configuration);
                        controller.StartSimulation();
                        accountingResultsManager.WriteDataToDisk(controller.AccountingModuleObject.MeasureHolder);
                        if (x != configuration.TrialId)
                        {
                            x = configuration.TrialId;
                            WriteLastIndex(x);
                        }
                        Thread.Sleep(5000);
                        btn_Start.Invoke(new Action(() =>
                        {
                            AddDataHolder(controller.AccountingModuleObject.MeasureHolder);
                            btn_Start.Enabled = true;
                        }));
                    }
                    
                });
            t.Priority = ThreadPriority.Highest;
            t.Start();
        }

        private void WriteLastIndex(int x)
        {
            try
            {
                StreamWriter r = new StreamWriter("D:/Last.txt",false);
                r.WriteLine(x);
                r.Flush();
                r.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private void AddDataHolder(MeasureValueHolder holder)
        {
            var dbHelper = new DatabaseTrialResultManagement();
            dbHelper.Save(holder);
            _measuredValueListsTrials.Add(holder);
            if (_measuredValueListsTrials.Count > 10)
                _measuredValueListsTrials.RemoveAt(0);
            var btn = new Button()
            {
                Text = holder.Name,
                Width = 200
            };
            btn.Click += Btn_Click;
            flowLayoutPanel1.Controls.Add(btn);
            flowLayoutPanel1.Controls.RemoveAt(0);
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
            myPane.Title.Text = title.ToString().Replace('_', ' ');
            myPane.XAxis.Title.Text = $"Time Unit is Second";
            myPane.YAxis.Title.Text = yAxis.ToString().Replace('_',' ');
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
                        myLabel.Text = $"Average entropy in All the trial = {_measuredValueListsTrials[t].AverageEntropy}";
                        break;
                    case BasicItems.Power_Consumption:
                        list[0].Add(0, _measuredValueListsTrials[t].PowerConsumption);
                        myLabel.Text = $"Power Consumption in All the trial = {_measuredValueListsTrials[t].PowerConsumption} Watts";

                        break;
                    case BasicItems.Average_Needed_Volume:
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
                        myLabel.Text = $"Average Volume in All the trial vs Average predicted volume";

                        break;

                    case BasicItems.Needed_Load_Standard_Deviation:
                        for (int i = 0; i < _measuredValueListsTrials[t].HostMeasureValuesList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].HostMeasureValuesList[i].StandardDeviation);
                        }
                        myLabel.Text = $"Average Standard Deviation {_measuredValueListsTrials[t].AverageStdDeviation}";
                        break;
                    case BasicItems.No_of_Hosts:
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
                    
                    case BasicItems.Hosts_States:
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
                    case BasicItems.Total_Messages:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].TotalMessages);
                        }
                        myLabel.Text = $"Total Messages in All the trial = {_measuredValueListsTrials[t].TotalMessages}";

                        break;
                    case BasicItems.Total_Communicated_Size:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].CommunicatedSize);
                        }
                        myLabel.Text = $"Total Communicated Size = {(int)_measuredValueListsTrials[t].TotalCommunicatedSize/1024} GB ";
                        break;
                    case BasicItems.Average_Host_Data_Out_and_In:

                        list.Add(new PointPairList());
                        labeList[0] = ($"Data Out of Trial {_measuredValueListsTrials[t].Name}");
                        labeList.Add($"Data In of Trial {_measuredValueListsTrials[t].Name}");
                        for (int i = 0; i < _measuredValueListsTrials[t].HostMeasureValuesList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].HostMeasureValuesList[i].AverageDataOut);
                            list[1].Add(i * unit, _measuredValueListsTrials[t].HostMeasureValuesList[i].AverageDataIn);
                        }
                        myLabel.Text = $"Average data Out/In = {_measuredValueListsTrials[t].AverageDataOut } / { _measuredValueListsTrials[t].AverageDataIn} MB";

                        break;
                    case BasicItems.Containers:
                        for (int i = 0; i < _measuredValueListsTrials[t].HostMeasureValuesList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].HostMeasureValuesList[i].Containers);
                        }
                        myLabel.Text = $"Average Containers in All the trial = {_measuredValueListsTrials[t].AverageContainers}";

                        break;
                    case BasicItems.Push_Requests:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].MeasuredValuesList[i].PushRequests);
                        }
                        myLabel.Text = $"Total Push Requests in All the trial = {_measuredValueListsTrials[t].TotalPushRequests}";

                        break;
                    case BasicItems.Pull_Requests:
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
                    case BasicItems.Migration_Count:
                        foreach (var item in _measuredValueListsTrials[t].ContainerMeasureValuesList)
                        {
                            list[0].Add(item.Key, item.Value.MigrationCount);
                        }
                        myLabel.Text = $"Average Migration per container in All the trial = {_measuredValueListsTrials[t].AvgMigrations}";

                        break;
                    case BasicItems.Push_Load_Availability_Request:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit,
                                _measuredValueListsTrials[t].MeasuredValuesList[i].PushLoadAvailabilityRequest);
                        }
                        myLabel.Text = $"Total Push Availability Requests in All the trial = {_measuredValueListsTrials[t].TotalPushAvailabilityRequests}";

                        break;
                    case BasicItems.Pull_Load_Availability_Request:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit,
                                _measuredValueListsTrials[t].MeasuredValuesList[i].PullLoadAvailabilityRequest);
                        }
                        myLabel.Text = $"Total Pull Availability Requests in All the trial = {_measuredValueListsTrials[t].TotalPullAvailabilityRequests}";

                        break;
                    case BasicItems.Utilization_SLA_Violations_Count:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit,
                                _measuredValueListsTrials[t].MeasuredValuesList[i].SlaViolationsCount);
                        }
                        myLabel.Text = $"Total SLA Violations the trial = {_measuredValueListsTrials[t].TotalSlaViolationsCount}";

                        break;
                    case BasicItems.SLA_Violations_Percent:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit,
                                _measuredValueListsTrials[t].MeasuredValuesList[i].SlaViolationsPercentage);
                        }
                        myLabel.Text = $"Average SLA Violations % = {_measuredValueListsTrials[t].AverageSlaViolationsPercent}";

                        break;
                    case BasicItems.Container_DownTime:
                        foreach (var item in _measuredValueListsTrials[t].ContainerMeasureValuesList)
                        {
                            list[0].Add(item.Key, item.Value.Downtime);
                        }
                        myLabel.Text = $"Average Down time of all containers in All the trial = {_measuredValueListsTrials[t].AvgDownTime}";

                        break;

                    case BasicItems.Image_Pulls:
                        for (int i = 0; i < _measuredValueListsTrials[t].MeasuredValuesList.Count; i++)
                        {
                            list[0].Add(i * unit,
                                _measuredValueListsTrials[t].MeasuredValuesList[i].ImagePulls);
                        }
                        myLabel.Text = $"Total ImagePulls of trial = {_measuredValueListsTrials[t].ImagePulls}";
                        break;
                    case BasicItems.Image_Pulls_Ratio:
                        list[0].Add(0, _measuredValueListsTrials[t].AveragePullPerImage);
                        myLabel.Text = $"Image Pull Ratio in All the trial = {_measuredValueListsTrials[t].AveragePullPerImage}";
                        break;
                    case BasicItems.Containers_Per_Host:
                        for (int i = 0; i < _measuredValueListsTrials[t].HostMeasureValuesList.Count; i++)
                        {
                            list[0].Add(i * unit, _measuredValueListsTrials[t].HostMeasureValuesList[i].ContainersPerHost);
                        }
                        myLabel.Text = $"Average Containers in All the trial = {_measuredValueListsTrials[t].AverageContainersPerHost}";
                        break;
                    default:
                        throw new NotImplementedException();
                }
                // Generate a red curve with diamond
                // symbols, and "Porsche" in the legend
                if (yAxis == BasicItems.Migration_Count || yAxis == BasicItems.Container_DownTime || yAxis == BasicItems.Power_Consumption || yAxis == BasicItems.Image_Pulls_Ratio)// || yAxis == BasicItems.OutOfBoundHosts)
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
        

        private void cb_GraphItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateGraph(zedGraphControl1, cb_GraphItem.Text, (BasicItems)cb_GraphItem.SelectedValue);
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
            OldCharts f = new OldCharts();
            f.Show(this);
            f.FormClosing += (a, b) =>
            {
                f.Owner?.Show();
            };
            this.Hide();
        }

        private void DBE_Click(object sender, EventArgs e)
        {
            DBE f = new DBE();
            f.Show(this);
            f.FormClosing += (a, b) =>
            {
                f.Owner?.Show();
            };
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProposedCharts f = new ProposedCharts();
            f.Show(this);
            f.FormClosing += (a, b) =>
            {
                f.Owner?.Show();
            };
            this.Hide();
        }
    }
}
