namespace Test
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_AddData = new System.Windows.Forms.Button();
            this.cb_Prediction = new System.Windows.Forms.ComboBox();
            this.cb_Change = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cb_HostsCount = new System.Windows.Forms.ComboBox();
            this.cb_DrawExtra = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cb_Strategy = new System.Windows.Forms.ComboBox();
            this.cb_GraphItem = new System.Windows.Forms.ComboBox();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.btn_Start = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.cb_StartUtil = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoScroll = true;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(14, 526);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(1225, 61);
            this.flowLayoutPanel2.TabIndex = 39;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(808, 10);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(431, 69);
            this.flowLayoutPanel1.TabIndex = 38;
            // 
            // btn_AddData
            // 
            this.btn_AddData.Location = new System.Drawing.Point(636, 50);
            this.btn_AddData.Name = "btn_AddData";
            this.btn_AddData.Size = new System.Drawing.Size(166, 23);
            this.btn_AddData.TabIndex = 37;
            this.btn_AddData.Text = "Add Data";
            this.btn_AddData.UseVisualStyleBackColor = true;
            this.btn_AddData.Click += new System.EventHandler(this.btn_AddData_Click);
            // 
            // cb_Prediction
            // 
            this.cb_Prediction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_Prediction.FormattingEnabled = true;
            this.cb_Prediction.Location = new System.Drawing.Point(532, 50);
            this.cb_Prediction.Name = "cb_Prediction";
            this.cb_Prediction.Size = new System.Drawing.Size(92, 21);
            this.cb_Prediction.TabIndex = 36;
            this.cb_Prediction.Visible = false;
            this.cb_Prediction.SelectedIndexChanged += new System.EventHandler(this.cb_Prediction_SelectedIndexChanged);
            // 
            // cb_Change
            // 
            this.cb_Change.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_Change.FormattingEnabled = true;
            this.cb_Change.Location = new System.Drawing.Point(359, 52);
            this.cb_Change.Name = "cb_Change";
            this.cb_Change.Size = new System.Drawing.Size(116, 21);
            this.cb_Change.TabIndex = 34;
            this.cb_Change.Visible = false;
            this.cb_Change.SelectedIndexChanged += new System.EventHandler(this.cb_FirstWave_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(481, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 32;
            this.label5.Text = "Prediction";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(481, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "Size";
            // 
            // cb_HostsCount
            // 
            this.cb_HostsCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_HostsCount.FormattingEnabled = true;
            this.cb_HostsCount.Location = new System.Drawing.Point(532, 10);
            this.cb_HostsCount.Name = "cb_HostsCount";
            this.cb_HostsCount.Size = new System.Drawing.Size(92, 21);
            this.cb_HostsCount.TabIndex = 31;
            this.cb_HostsCount.Visible = false;
            this.cb_HostsCount.SelectedIndexChanged += new System.EventHandler(this.cb_HostsCount_SelectedIndexChanged);
            // 
            // cb_DrawExtra
            // 
            this.cb_DrawExtra.AutoSize = true;
            this.cb_DrawExtra.Checked = true;
            this.cb_DrawExtra.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_DrawExtra.Location = new System.Drawing.Point(112, 37);
            this.cb_DrawExtra.Name = "cb_DrawExtra";
            this.cb_DrawExtra.Size = new System.Drawing.Size(77, 17);
            this.cb_DrawExtra.TabIndex = 30;
            this.cb_DrawExtra.Text = "DrawExtra";
            this.cb_DrawExtra.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(272, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Start Utilization";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(633, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "Strategy";
            // 
            // cb_Strategy
            // 
            this.cb_Strategy.DisplayMember = "Entropy";
            this.cb_Strategy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_Strategy.FormattingEnabled = true;
            this.cb_Strategy.Location = new System.Drawing.Point(688, 10);
            this.cb_Strategy.Name = "cb_Strategy";
            this.cb_Strategy.Size = new System.Drawing.Size(114, 21);
            this.cb_Strategy.TabIndex = 26;
            this.cb_Strategy.Visible = false;
            this.cb_Strategy.SelectedIndexChanged += new System.EventHandler(this.cb_Strategy_SelectedIndexChanged);
            // 
            // cb_GraphItem
            // 
            this.cb_GraphItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_GraphItem.FormattingEnabled = true;
            this.cb_GraphItem.Location = new System.Drawing.Point(112, 10);
            this.cb_GraphItem.Name = "cb_GraphItem";
            this.cb_GraphItem.Size = new System.Drawing.Size(152, 21);
            this.cb_GraphItem.TabIndex = 25;
            this.cb_GraphItem.SelectedIndexChanged += new System.EventHandler(this.cb_GraphItem_SelectedIndexChanged);
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Location = new System.Drawing.Point(5, 85);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(1234, 437);
            this.zedGraphControl1.TabIndex = 24;
            // 
            // btn_Start
            // 
            this.btn_Start.Location = new System.Drawing.Point(5, 6);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(101, 65);
            this.btn_Start.TabIndex = 23;
            this.btn_Start.Text = "Start ";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(272, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 41;
            this.label6.Text = "MidTimeChange";
            // 
            // cb_StartUtil
            // 
            this.cb_StartUtil.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_StartUtil.FormattingEnabled = true;
            this.cb_StartUtil.Location = new System.Drawing.Point(359, 10);
            this.cb_StartUtil.Name = "cb_StartUtil";
            this.cb_StartUtil.Size = new System.Drawing.Size(116, 21);
            this.cb_StartUtil.TabIndex = 42;
            this.cb_StartUtil.Visible = false;
            this.cb_StartUtil.SelectedIndexChanged += new System.EventHandler(this.cb_StartUtil_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(111, 55);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(153, 23);
            this.button1.TabIndex = 43;
            this.button1.Text = "Form2";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1246, 588);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cb_StartUtil);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.btn_AddData);
            this.Controls.Add(this.cb_Prediction);
            this.Controls.Add(this.cb_Change);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cb_HostsCount);
            this.Controls.Add(this.cb_DrawExtra);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cb_Strategy);
            this.Controls.Add(this.cb_GraphItem);
            this.Controls.Add(this.zedGraphControl1);
            this.Controls.Add(this.btn_Start);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btn_AddData;
        private System.Windows.Forms.ComboBox cb_Prediction;
        private System.Windows.Forms.ComboBox cb_Change;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cb_HostsCount;
        private System.Windows.Forms.CheckBox cb_DrawExtra;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cb_Strategy;
        private System.Windows.Forms.ComboBox cb_GraphItem;
        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cb_StartUtil;
        private System.Windows.Forms.Button button1;
    }
}

