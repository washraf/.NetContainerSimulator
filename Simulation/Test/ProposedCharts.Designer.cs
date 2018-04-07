namespace Test
{
    partial class ProposedCharts
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
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.cb_Item = new System.Windows.Forms.ComboBox();
            this.cb_StartUtil = new System.Windows.Forms.ComboBox();
            this.cb_Change = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Location = new System.Drawing.Point(12, 65);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(777, 412);
            this.zedGraphControl1.TabIndex = 0;
            this.zedGraphControl1.UseExtendedPrintDialog = true;
            // 
            // cb_Item
            // 
            this.cb_Item.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_Item.FormattingEnabled = true;
            this.cb_Item.Location = new System.Drawing.Point(45, 26);
            this.cb_Item.Name = "cb_Item";
            this.cb_Item.Size = new System.Drawing.Size(121, 21);
            this.cb_Item.TabIndex = 1;
            this.cb_Item.SelectedIndexChanged += new System.EventHandler(this.cb_SelectedIndexChanged);
            // 
            // cb_StartUtil
            // 
            this.cb_StartUtil.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_StartUtil.FormattingEnabled = true;
            this.cb_StartUtil.Location = new System.Drawing.Point(200, 26);
            this.cb_StartUtil.Name = "cb_StartUtil";
            this.cb_StartUtil.Size = new System.Drawing.Size(121, 21);
            this.cb_StartUtil.TabIndex = 1;
            this.cb_StartUtil.SelectedIndexChanged += new System.EventHandler(this.cb_SelectedIndexChanged);
            // 
            // cb_Change
            // 
            this.cb_Change.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_Change.FormattingEnabled = true;
            this.cb_Change.Location = new System.Drawing.Point(370, 26);
            this.cb_Change.Name = "cb_Change";
            this.cb_Change.Size = new System.Drawing.Size(121, 21);
            this.cb_Change.TabIndex = 1;
            this.cb_Change.SelectedIndexChanged += new System.EventHandler(this.cb_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Item";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(172, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Util";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(327, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Action";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(624, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Save To Disk";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 489);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cb_Change);
            this.Controls.Add(this.cb_StartUtil);
            this.Controls.Add(this.cb_Item);
            this.Controls.Add(this.zedGraphControl1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.ProposedCharts_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.ComboBox cb_Item;
        private System.Windows.Forms.ComboBox cb_StartUtil;
        private System.Windows.Forms.ComboBox cb_Change;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
    }
}