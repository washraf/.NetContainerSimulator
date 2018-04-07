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
            this.cb_DrawExtra = new System.Windows.Forms.CheckBox();
            this.cb_GraphItem = new System.Windows.Forms.ComboBox();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.btn_Start = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.DBE = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoScroll = true;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(6, 573);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(1225, 61);
            this.flowLayoutPanel2.TabIndex = 39;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(417, 10);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(740, 69);
            this.flowLayoutPanel1.TabIndex = 38;
            // 
            // btn_AddData
            // 
            this.btn_AddData.Location = new System.Drawing.Point(1163, 12);
            this.btn_AddData.Name = "btn_AddData";
            this.btn_AddData.Size = new System.Drawing.Size(76, 67);
            this.btn_AddData.TabIndex = 37;
            this.btn_AddData.Text = "Add Data";
            this.btn_AddData.UseVisualStyleBackColor = true;
            this.btn_AddData.Click += new System.EventHandler(this.btn_AddData_Click);
            // 
            // cb_DrawExtra
            // 
            this.cb_DrawExtra.AutoSize = true;
            this.cb_DrawExtra.Checked = true;
            this.cb_DrawExtra.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_DrawExtra.Location = new System.Drawing.Point(112, 37);
            this.cb_DrawExtra.Name = "cb_DrawExtra";
            this.cb_DrawExtra.Size = new System.Drawing.Size(75, 17);
            this.cb_DrawExtra.TabIndex = 30;
            this.cb_DrawExtra.Text = "DrawExtra";
            this.cb_DrawExtra.UseVisualStyleBackColor = true;
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
            this.zedGraphControl1.Size = new System.Drawing.Size(1234, 482);
            this.zedGraphControl1.TabIndex = 24;
            this.zedGraphControl1.UseExtendedPrintDialog = true;
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(288, 48);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(76, 23);
            this.button1.TabIndex = 43;
            this.button1.Text = "Old Charts";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // DBE
            // 
            this.DBE.Location = new System.Drawing.Point(288, 10);
            this.DBE.Name = "DBE";
            this.DBE.Size = new System.Drawing.Size(76, 23);
            this.DBE.TabIndex = 44;
            this.DBE.Text = "DBE";
            this.DBE.UseVisualStyleBackColor = true;
            this.DBE.Click += new System.EventHandler(this.DBE_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(112, 56);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(152, 23);
            this.button2.TabIndex = 45;
            this.button2.Text = "New Charts";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1243, 637);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.DBE);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.btn_AddData);
            this.Controls.Add(this.cb_DrawExtra);
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
        private System.Windows.Forms.CheckBox cb_DrawExtra;
        private System.Windows.Forms.ComboBox cb_GraphItem;
        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button DBE;
        private System.Windows.Forms.Button button2;
    }
}

