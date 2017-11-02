namespace Test
{
    partial class Form3
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cb_ContainerSelectionPolicy = new System.Windows.Forms.CheckedListBox();
            this.cb_containerAllocationPolicies = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Location = new System.Drawing.Point(47, 113);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(796, 345);
            this.zedGraphControl1.TabIndex = 1;
            // 
            // cb_Item
            // 
            this.cb_Item.FormattingEnabled = true;
            this.cb_Item.Location = new System.Drawing.Point(79, 12);
            this.cb_Item.Name = "cb_Item";
            this.cb_Item.Size = new System.Drawing.Size(234, 21);
            this.cb_Item.TabIndex = 2;
            this.cb_Item.SelectedIndexChanged += new System.EventHandler(this.cb_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Parameters";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "ContainerSelectionPolicy";
            // 
            // cb_ContainerSelectionPolicy
            // 
            this.cb_ContainerSelectionPolicy.FormattingEnabled = true;
            this.cb_ContainerSelectionPolicy.Location = new System.Drawing.Point(147, 58);
            this.cb_ContainerSelectionPolicy.MultiColumn = true;
            this.cb_ContainerSelectionPolicy.Name = "cb_ContainerSelectionPolicy";
            this.cb_ContainerSelectionPolicy.Size = new System.Drawing.Size(266, 19);
            this.cb_ContainerSelectionPolicy.TabIndex = 5;
            this.cb_ContainerSelectionPolicy.SelectedIndexChanged += new System.EventHandler(this.cb_SelectedIndexChanged);
            // 
            // cb_containerAllocationPolicies
            // 
            this.cb_containerAllocationPolicies.FormattingEnabled = true;
            this.cb_containerAllocationPolicies.Location = new System.Drawing.Point(147, 87);
            this.cb_containerAllocationPolicies.MultiColumn = true;
            this.cb_containerAllocationPolicies.Name = "cb_containerAllocationPolicies";
            this.cb_containerAllocationPolicies.Size = new System.Drawing.Size(416, 19);
            this.cb_containerAllocationPolicies.TabIndex = 7;
            this.cb_containerAllocationPolicies.SelectedIndexChanged += new System.EventHandler(this.cb_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "containerAllocationPolicies";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(779, 29);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 470);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cb_containerAllocationPolicies);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cb_ContainerSelectionPolicy);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cb_Item);
            this.Controls.Add(this.zedGraphControl1);
            this.Name = "Form3";
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.ComboBox cb_Item;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckedListBox cb_ContainerSelectionPolicy;
        private System.Windows.Forms.CheckedListBox cb_containerAllocationPolicies;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}