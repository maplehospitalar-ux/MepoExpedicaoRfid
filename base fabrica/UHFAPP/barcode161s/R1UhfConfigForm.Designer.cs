namespace UHFAPP
{
    partial class R1UhfConfigForm
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
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cbPower = new System.Windows.Forms.CheckBox();
            this.cmbPower_ANT1 = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.btnPowerGet_ANT1 = new System.Windows.Forms.Button();
            this.label23 = new System.Windows.Forms.Label();
            this.btnPowerSet_ANT1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.cmbRegion = new System.Windows.Forms.ComboBox();
            this.cbRegionSave = new System.Windows.Forms.CheckBox();
            this.btnRegionGet = new System.Windows.Forms.Button();
            this.btnRegionSet = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox6.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox6
            // 
            this.groupBox6.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.groupBox6.Controls.Add(this.cbPower);
            this.groupBox6.Controls.Add(this.cmbPower_ANT1);
            this.groupBox6.Controls.Add(this.label24);
            this.groupBox6.Controls.Add(this.btnPowerGet_ANT1);
            this.groupBox6.Controls.Add(this.label23);
            this.groupBox6.Controls.Add(this.btnPowerSet_ANT1);
            this.groupBox6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox6.ForeColor = System.Drawing.Color.Black;
            this.groupBox6.Location = new System.Drawing.Point(9, 9);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(406, 113);
            this.groupBox6.TabIndex = 31;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Power";
            // 
            // cbPower
            // 
            this.cbPower.AutoSize = true;
            this.cbPower.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbPower.Location = new System.Drawing.Point(308, 76);
            this.cbPower.Name = "cbPower";
            this.cbPower.Size = new System.Drawing.Size(59, 20);
            this.cbPower.TabIndex = 26;
            this.cbPower.Text = "Save";
            this.cbPower.UseVisualStyleBackColor = true;
            // 
            // cmbPower_ANT1
            // 
            this.cmbPower_ANT1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbPower_ANT1.FormattingEnabled = true;
            this.cmbPower_ANT1.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30"});
            this.cmbPower_ANT1.Location = new System.Drawing.Point(140, 33);
            this.cmbPower_ANT1.Name = "cmbPower_ANT1";
            this.cmbPower_ANT1.Size = new System.Drawing.Size(193, 24);
            this.cmbPower_ANT1.TabIndex = 6;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label24.Location = new System.Drawing.Point(31, 36);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(80, 16);
            this.label24.TabIndex = 14;
            this.label24.Text = "输出功率:";
            // 
            // btnPowerGet_ANT1
            // 
            this.btnPowerGet_ANT1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPowerGet_ANT1.ForeColor = System.Drawing.Color.Black;
            this.btnPowerGet_ANT1.Location = new System.Drawing.Point(95, 67);
            this.btnPowerGet_ANT1.Name = "btnPowerGet_ANT1";
            this.btnPowerGet_ANT1.Size = new System.Drawing.Size(90, 31);
            this.btnPowerGet_ANT1.TabIndex = 13;
            this.btnPowerGet_ANT1.Text = "Get";
            this.btnPowerGet_ANT1.UseVisualStyleBackColor = true;
            this.btnPowerGet_ANT1.Click += new System.EventHandler(this.btnPowerGet_Click);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label23.Location = new System.Drawing.Point(332, 38);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(32, 16);
            this.label23.TabIndex = 12;
            this.label23.Text = "dBm";
            // 
            // btnPowerSet_ANT1
            // 
            this.btnPowerSet_ANT1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPowerSet_ANT1.ForeColor = System.Drawing.Color.Black;
            this.btnPowerSet_ANT1.Location = new System.Drawing.Point(212, 67);
            this.btnPowerSet_ANT1.Name = "btnPowerSet_ANT1";
            this.btnPowerSet_ANT1.Size = new System.Drawing.Size(88, 31);
            this.btnPowerSet_ANT1.TabIndex = 11;
            this.btnPowerSet_ANT1.Text = "Set";
            this.btnPowerSet_ANT1.UseVisualStyleBackColor = true;
            this.btnPowerSet_ANT1.Click += new System.EventHandler(this.btnPowerSet_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel1.Controls.Add(this.groupBox6);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.groupBox11);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(943, 681);
            this.panel1.TabIndex = 32;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(205, 664);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(0, 12);
            this.label8.TabIndex = 60;
            // 
            // groupBox11
            // 
            this.groupBox11.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.groupBox11.Controls.Add(this.cmbRegion);
            this.groupBox11.Controls.Add(this.cbRegionSave);
            this.groupBox11.Controls.Add(this.btnRegionGet);
            this.groupBox11.Controls.Add(this.btnRegionSet);
            this.groupBox11.Controls.Add(this.label1);
            this.groupBox11.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox11.ForeColor = System.Drawing.Color.Black;
            this.groupBox11.Location = new System.Drawing.Point(9, 128);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(406, 93);
            this.groupBox11.TabIndex = 44;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Region";
            // 
            // cmbRegion
            // 
            this.cmbRegion.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbRegion.FormattingEnabled = true;
            this.cmbRegion.Items.AddRange(new object[] {
            "China1",
            "China2",
            "Europe",
            "USA",
            "Korea",
            "Japan",
            "Taiwan",
            "South Africa",
            "Peru(915-928 MHz)",
            "Russia(860MHz-867.6MHz)"});
            this.cmbRegion.Location = new System.Drawing.Point(140, 17);
            this.cmbRegion.Name = "cmbRegion";
            this.cmbRegion.Size = new System.Drawing.Size(193, 24);
            this.cmbRegion.TabIndex = 18;
            // 
            // cbRegionSave
            // 
            this.cbRegionSave.AutoSize = true;
            this.cbRegionSave.Checked = true;
            this.cbRegionSave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRegionSave.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbRegionSave.Location = new System.Drawing.Point(311, 55);
            this.cbRegionSave.Name = "cbRegionSave";
            this.cbRegionSave.Size = new System.Drawing.Size(59, 20);
            this.cbRegionSave.TabIndex = 25;
            this.cbRegionSave.Text = "Save";
            this.cbRegionSave.UseVisualStyleBackColor = true;
            // 
            // btnRegionGet
            // 
            this.btnRegionGet.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRegionGet.ForeColor = System.Drawing.Color.Black;
            this.btnRegionGet.Location = new System.Drawing.Point(95, 49);
            this.btnRegionGet.Name = "btnRegionGet";
            this.btnRegionGet.Size = new System.Drawing.Size(90, 31);
            this.btnRegionGet.TabIndex = 24;
            this.btnRegionGet.Text = "Get";
            this.btnRegionGet.UseVisualStyleBackColor = true;
            this.btnRegionGet.Click += new System.EventHandler(this.btnRegionGet_Click);
            // 
            // btnRegionSet
            // 
            this.btnRegionSet.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRegionSet.ForeColor = System.Drawing.Color.Black;
            this.btnRegionSet.Location = new System.Drawing.Point(212, 49);
            this.btnRegionSet.Name = "btnRegionSet";
            this.btnRegionSet.Size = new System.Drawing.Size(92, 31);
            this.btnRegionSet.TabIndex = 23;
            this.btnRegionSet.Text = "Set";
            this.btnRegionSet.UseVisualStyleBackColor = true;
            this.btnRegionSet.Click += new System.EventHandler(this.btnRegionSet_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(51, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 22;
            this.label1.Text = "Region:";
            // 
            // R1UhfConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(943, 681);
            this.Controls.Add(this.panel1);
            this.Name = "R1UhfConfigForm";
            this.Text = "ConfigForm";
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigForm_FormClosing);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button btnPowerSet_ANT1;
        private System.Windows.Forms.ComboBox cmbPower_ANT1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnPowerGet_ANT1;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.ComboBox cmbRegion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbRegionSave;
        private System.Windows.Forms.Button btnRegionGet;
        private System.Windows.Forms.Button btnRegionSet;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.CheckBox cbPower;
    }
}