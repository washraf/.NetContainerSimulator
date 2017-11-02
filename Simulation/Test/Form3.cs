using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Simulation.LocationStrategies;
using ZedGraph;

namespace Test
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private List<LoadedItem> Average;

        #region -- LoadData and Average--

        private void Form3_Load(object sender, EventArgs e)
        {

            Average = ComputationEngine.ComputeAverage();
            cb_Item.DataSource = Enum.GetValues(typeof (ResultItem)).Cast<ResultItem>();
            cb_Item.SelectedIndex = 0;
            cb_ContainerSelectionPolicy.DataSource =
                Enum.GetValues(typeof (ContainerSelectionPolicies)).Cast<ContainerSelectionPolicies>();
            cb_ContainerSelectionPolicy.SelectedIndex = 0;
            cb_containerAllocationPolicies.DataSource =
                Enum.GetValues(typeof (containerAllocationPolicies)).Cast<containerAllocationPolicies>();
            cb_containerAllocationPolicies.SelectedIndex = 0;
            Done = true;
            DrawLoadedItems();
        }
        

        #endregion

        public bool Done { get; set; } = false;

        private void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Done)
                DrawLoadedItems();
        }

        private void DrawLoadedItems()
        {
            var containerPolicies = cb_ContainerSelectionPolicy.CheckedItems.Cast<object>()
                .Select(x => this.cb_ContainerSelectionPolicy.GetItemText(x));
            var containerAllocationPolicies = cb_containerAllocationPolicies.CheckedItems.Cast<object>()
                .Select(x => this.cb_containerAllocationPolicies.GetItemText(x));

            var final = Average.Where(x => containerPolicies.Contains(x.ContainerSpolicy)).ToList();
            final = final.Where(x => containerAllocationPolicies.Contains(x.ContainerPlacement)).ToList();



            var y = (ResultItem) Enum.Parse(typeof (ResultItem), cb_Item.SelectedValue.ToString());
            CreateGraph(zedGraphControl1, y, final);
        }

        private void CreateGraph(ZedGraphControl zgc, ResultItem yAxis, List<LoadedItem> trials)
        {
            // get a reference to the GraphPane
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();
            myPane.YAxis.Scale.MinAuto = true;
            myPane.YAxis.Scale.MaxAuto = true;

            // Set the Titles
            myPane.Title.Text = yAxis.ToString();
            myPane.XAxis.Title.Text = $"Trial Name";
            myPane.YAxis.Title.Text = yAxis.ToString();



            //trials.Single(
            //    x => x.Size == trial.Size && x.Algorithm == Strategies.InOrderProping.ToString());
            List<double> vvvv;
            switch (yAxis)
            {
                case ResultItem.energy:
                    vvvv = trials.Select(x => x.energy).ToList();

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(yAxis), yAxis, null);
            }


            BarItem myBar = myPane.AddBar(yAxis.ToString(), null, vvvv.ToArray(),
                Color.Red);
            myBar.Bar.Fill = new Fill(Color.Red, Color.White, Color.Red);

            myPane.Legend.Position = LegendPos.BottomCenter;
            //myPane.Chart.Fill = new Fill(Color.White, Color.LightCyan, 360.0f);

            // Set the XAxis labels
            myPane.XAxis.Scale.TextLabels = trials.Select(x => x.ExperimentName).ToArray();
            // Set the XAxis to Text type
            myPane.XAxis.Type = AxisType.Text;

            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zgc.AxisChange();
            zgc.Refresh();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var w = new StreamWriter("F:\\Result.csv"))
            {
                var line = $"hostSelectionPolicy,VMSPolicy,ContainerSpolicy,ContainerPlacement,energy,slaOverall,meanActiveHosts,meanNumberOfContainerMigrations,totalContainerMigration,totalVmCreated,numberOfOverUtilization,";
                w.WriteLine(line);
                w.Flush();
                foreach (var item in Average)
                {
                    line = $"{item.hostSelectionPolicy},{item.VMSPolicy},{item.ContainerSpolicy},{item.ContainerPlacement},"
                        + $"{item.energy},{item.slaOverall},{item.meanActiveHosts},{item.meanNumberOfContainerMigrations},{item.totalContainerMigration},{item.totalVmCreated},{item.numberOfOverUtilization}";
                    w.WriteLine(line);
                    w.Flush();
                }

            }
        }
    }

    public enum ContainerSelectionPolicies
    {
        Cor,
        MaxUsage
    }

    public enum ResultItem
    {
        energy,
        slaOverall,
        meanActiveHosts,
        meanNumberOfContainerMigrations,
        totalContainerMigration,
        totalVmCreated,
        numberOfOverUtilization,
    }

    public enum containerAllocationPolicies
    {
        MostFull,
        FirstFit,
        LeastFull,
        Simple,
        Random
    }
}


