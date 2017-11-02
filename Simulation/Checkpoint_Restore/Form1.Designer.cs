namespace Checkpoint_Restore
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
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.btn_Load = new System.Windows.Forms.Button();
            this.cb_GraphItem = new System.Windows.Forms.ComboBox();
            this.cb_DrawExtra = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Location = new System.Drawing.Point(12, 63);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(865, 394);
            this.zedGraphControl1.TabIndex = 0;
            // 
            // btn_Load
            // 
            this.btn_Load.Location = new System.Drawing.Point(802, 12);
            this.btn_Load.Name = "btn_Load";
            this.btn_Load.Size = new System.Drawing.Size(75, 23);
            this.btn_Load.TabIndex = 1;
            this.btn_Load.Text = "Load Data";
            this.btn_Load.UseVisualStyleBackColor = true;
            this.btn_Load.Click += new System.EventHandler(this.btn_Load_Click);
            // 
            // cb_GraphItem
            // 
            this.cb_GraphItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_GraphItem.FormattingEnabled = true;
            this.cb_GraphItem.Location = new System.Drawing.Point(88, 14);
            this.cb_GraphItem.Name = "cb_GraphItem";
            this.cb_GraphItem.Size = new System.Drawing.Size(121, 21);
            this.cb_GraphItem.TabIndex = 2;
            this.cb_GraphItem.SelectedIndexChanged += new System.EventHandler(this.cb_GraphItem_SelectedIndexChanged);
            // 
            // cb_DrawExtra
            // 
            this.cb_DrawExtra.AutoSize = true;
            this.cb_DrawExtra.Checked = true;
            this.cb_DrawExtra.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_DrawExtra.Location = new System.Drawing.Point(88, 40);
            this.cb_DrawExtra.Name = "cb_DrawExtra";
            this.cb_DrawExtra.Size = new System.Drawing.Size(114, 17);
            this.cb_DrawExtra.TabIndex = 3;
            this.cb_DrawExtra.Text = "Draw Min and Max";
            this.cb_DrawExtra.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Draw Value";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 469);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cb_DrawExtra);
            this.Controls.Add(this.cb_GraphItem);
            this.Controls.Add(this.btn_Load);
            this.Controls.Add(this.zedGraphControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.Button btn_Load;
        private System.Windows.Forms.ComboBox cb_GraphItem;
        private System.Windows.Forms.CheckBox cb_DrawExtra;
        private System.Windows.Forms.Label label1;
    }
}

