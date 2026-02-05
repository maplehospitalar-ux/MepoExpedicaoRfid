namespace UHFAPP.multidevice
{
    partial class MainForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnStart1 = new System.Windows.Forms.Button();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.btnDisConn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnStart2 = new System.Windows.Forms.Button();
            this.btnDisConn2 = new System.Windows.Forms.Button();
            this.txtPort2 = new System.Windows.Forms.TextBox();
            this.btnConn2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtIP2 = new System.Windows.Forms.TextBox();
            this.lvEPC = new System.Windows.Forms.ListView();
            this.columnHeaderID = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderEPC = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderRssi = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderCount = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderANT = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderIP = new System.Windows.Forms.ColumnHeader();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cbPower = new System.Windows.Forms.CheckBox();
            this.cmbPower = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.btnPowerGet = new System.Windows.Forms.Button();
            this.label23 = new System.Windows.Forms.Label();
            this.btnPowerSet = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.btnStart1);
            this.groupBox1.Controls.Add(this.txtPort);
            this.groupBox1.Controls.Add(this.btnDisConn);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnConn);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtIP);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(530, 54);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IP-1";
            // 
            // btnStart1
            // 
            this.btnStart1.Enabled = false;
            this.btnStart1.Location = new System.Drawing.Point(459, 13);
            this.btnStart1.Name = "btnStart1";
            this.btnStart1.Size = new System.Drawing.Size(51, 35);
            this.btnStart1.TabIndex = 11;
            this.btnStart1.Text = "Start";
            this.btnStart1.UseVisualStyleBackColor = true;
            this.btnStart1.Click += new System.EventHandler(this.btnStart1_Click);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(220, 20);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(70, 21);
            this.txtPort.TabIndex = 7;
            this.txtPort.Text = "8002";
            // 
            // btnDisConn
            // 
            this.btnDisConn.Enabled = false;
            this.btnDisConn.Location = new System.Drawing.Point(377, 13);
            this.btnDisConn.Name = "btnDisConn";
            this.btnDisConn.Size = new System.Drawing.Size(75, 35);
            this.btnDisConn.TabIndex = 10;
            this.btnDisConn.Text = "DisConnect";
            this.btnDisConn.UseVisualStyleBackColor = true;
            this.btnDisConn.Click += new System.EventHandler(this.btnDisConn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(185, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "Port";
            // 
            // btnConn
            // 
            this.btnConn.Location = new System.Drawing.Point(296, 13);
            this.btnConn.Name = "btnConn";
            this.btnConn.Size = new System.Drawing.Size(75, 35);
            this.btnConn.TabIndex = 9;
            this.btnConn.Text = "Connect";
            this.btnConn.UseVisualStyleBackColor = true;
            this.btnConn.Click += new System.EventHandler(this.btnConn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "IP";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(30, 20);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(124, 21);
            this.txtIP.TabIndex = 4;
            this.txtIP.Text = "192.168.99.66";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.btnStart2);
            this.groupBox2.Controls.Add(this.btnDisConn2);
            this.groupBox2.Controls.Add(this.txtPort2);
            this.groupBox2.Controls.Add(this.btnConn2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtIP2);
            this.groupBox2.Location = new System.Drawing.Point(565, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(541, 54);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "IP-2";
            // 
            // btnStart2
            // 
            this.btnStart2.Enabled = false;
            this.btnStart2.Location = new System.Drawing.Point(459, 13);
            this.btnStart2.Name = "btnStart2";
            this.btnStart2.Size = new System.Drawing.Size(60, 35);
            this.btnStart2.TabIndex = 13;
            this.btnStart2.Text = "Start";
            this.btnStart2.UseVisualStyleBackColor = true;
            this.btnStart2.Click += new System.EventHandler(this.btnStart2_Click);
            // 
            // btnDisConn2
            // 
            this.btnDisConn2.Enabled = false;
            this.btnDisConn2.Location = new System.Drawing.Point(377, 13);
            this.btnDisConn2.Name = "btnDisConn2";
            this.btnDisConn2.Size = new System.Drawing.Size(75, 35);
            this.btnDisConn2.TabIndex = 12;
            this.btnDisConn2.Text = "DisConnect";
            this.btnDisConn2.UseVisualStyleBackColor = true;
            this.btnDisConn2.Click += new System.EventHandler(this.btnDisConn2_Click);
            // 
            // txtPort2
            // 
            this.txtPort2.Location = new System.Drawing.Point(220, 20);
            this.txtPort2.Name = "txtPort2";
            this.txtPort2.Size = new System.Drawing.Size(70, 21);
            this.txtPort2.TabIndex = 7;
            this.txtPort2.Text = "8888";
            // 
            // btnConn2
            // 
            this.btnConn2.Location = new System.Drawing.Point(296, 13);
            this.btnConn2.Name = "btnConn2";
            this.btnConn2.Size = new System.Drawing.Size(75, 35);
            this.btnConn2.TabIndex = 11;
            this.btnConn2.Text = "Connect";
            this.btnConn2.UseVisualStyleBackColor = true;
            this.btnConn2.Click += new System.EventHandler(this.btnConn2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(185, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Port";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "IP";
            // 
            // txtIP2
            // 
            this.txtIP2.Location = new System.Drawing.Point(30, 20);
            this.txtIP2.Name = "txtIP2";
            this.txtIP2.Size = new System.Drawing.Size(124, 21);
            this.txtIP2.TabIndex = 4;
            this.txtIP2.Text = "192.168.99.203";
            // 
            // lvEPC
            // 
            this.lvEPC.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lvEPC.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderID,
            this.columnHeaderEPC,
            this.columnHeaderRssi,
            this.columnHeaderCount,
            this.columnHeaderANT,
            this.columnHeaderIP});
            this.lvEPC.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvEPC.FullRowSelect = true;
            this.lvEPC.Location = new System.Drawing.Point(12, 72);
            this.lvEPC.Name = "lvEPC";
            this.lvEPC.Size = new System.Drawing.Size(835, 401);
            this.lvEPC.TabIndex = 11;
            this.lvEPC.UseCompatibleStateImageBehavior = false;
            this.lvEPC.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderID
            // 
            this.columnHeaderID.Text = "ID";
            this.columnHeaderID.Width = 0;
            // 
            // columnHeaderEPC
            // 
            this.columnHeaderEPC.Text = "EPC";
            this.columnHeaderEPC.Width = 420;
            // 
            // columnHeaderRssi
            // 
            this.columnHeaderRssi.Text = "Rssi";
            this.columnHeaderRssi.Width = 90;
            // 
            // columnHeaderCount
            // 
            this.columnHeaderCount.Text = "Count";
            this.columnHeaderCount.Width = 80;
            // 
            // columnHeaderANT
            // 
            this.columnHeaderANT.Text = "ANT";
            this.columnHeaderANT.Width = 50;
            // 
            // columnHeaderIP
            // 
            this.columnHeaderIP.Text = "IP";
            this.columnHeaderIP.Width = 180;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox6);
            this.groupBox3.Location = new System.Drawing.Point(857, 72);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(277, 401);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Settings";
            // 
            // groupBox6
            // 
            this.groupBox6.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox6.Controls.Add(this.cbPower);
            this.groupBox6.Controls.Add(this.cmbPower);
            this.groupBox6.Controls.Add(this.label24);
            this.groupBox6.Controls.Add(this.btnPowerGet);
            this.groupBox6.Controls.Add(this.label23);
            this.groupBox6.Controls.Add(this.btnPowerSet);
            this.groupBox6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox6.ForeColor = System.Drawing.Color.Black;
            this.groupBox6.Location = new System.Drawing.Point(16, 20);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(249, 92);
            this.groupBox6.TabIndex = 32;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Power";
            // 
            // cbPower
            // 
            this.cbPower.AutoSize = true;
            this.cbPower.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbPower.Location = new System.Drawing.Point(185, 49);
            this.cbPower.Name = "cbPower";
            this.cbPower.Size = new System.Drawing.Size(48, 16);
            this.cbPower.TabIndex = 26;
            this.cbPower.Text = "Save";
            this.cbPower.UseVisualStyleBackColor = true;
            // 
            // cmbPower
            // 
            this.cmbPower.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbPower.FormattingEnabled = true;
            this.cmbPower.Items.AddRange(new object[] {
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
            this.cmbPower.Location = new System.Drawing.Point(47, 14);
            this.cmbPower.Name = "cmbPower";
            this.cmbPower.Size = new System.Drawing.Size(104, 20);
            this.cmbPower.TabIndex = 6;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label24.Location = new System.Drawing.Point(6, 17);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(41, 12);
            this.label24.TabIndex = 14;
            this.label24.Text = "Power:";
            // 
            // btnPowerGet
            // 
            this.btnPowerGet.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPowerGet.ForeColor = System.Drawing.Color.Black;
            this.btnPowerGet.Location = new System.Drawing.Point(39, 49);
            this.btnPowerGet.Name = "btnPowerGet";
            this.btnPowerGet.Size = new System.Drawing.Size(70, 31);
            this.btnPowerGet.TabIndex = 13;
            this.btnPowerGet.Text = "Get";
            this.btnPowerGet.UseVisualStyleBackColor = true;
            this.btnPowerGet.Click += new System.EventHandler(this.btnPowerGet_Click);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label23.Location = new System.Drawing.Point(172, 17);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(23, 12);
            this.label23.TabIndex = 12;
            this.label23.Text = "dBm";
            // 
            // btnPowerSet
            // 
            this.btnPowerSet.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPowerSet.ForeColor = System.Drawing.Color.Black;
            this.btnPowerSet.Location = new System.Drawing.Point(115, 49);
            this.btnPowerSet.Name = "btnPowerSet";
            this.btnPowerSet.Size = new System.Drawing.Size(64, 31);
            this.btnPowerSet.TabIndex = 11;
            this.btnPowerSet.Text = "Set";
            this.btnPowerSet.UseVisualStyleBackColor = true;
            this.btnPowerSet.Click += new System.EventHandler(this.btnPowerSet_Click);
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(387, 490);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(11, 12);
            this.lblCount.TabIndex = 16;
            this.lblCount.Text = "0";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(447, 485);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "清空";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1145, 542);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.lvEPC);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtPort2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtIP2;
        private System.Windows.Forms.Button btnConn;
        private System.Windows.Forms.Button btnDisConn;
        private System.Windows.Forms.Button btnDisConn2;
        private System.Windows.Forms.Button btnConn2;
        private System.Windows.Forms.ListView lvEPC;
        private System.Windows.Forms.ColumnHeader columnHeaderID;
        private System.Windows.Forms.ColumnHeader columnHeaderEPC;
        private System.Windows.Forms.ColumnHeader columnHeaderRssi;
        private System.Windows.Forms.ColumnHeader columnHeaderCount;
        private System.Windows.Forms.ColumnHeader columnHeaderANT;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox cbPower;
        private System.Windows.Forms.ComboBox cmbPower;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Button btnPowerGet;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button btnPowerSet;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.ColumnHeader columnHeaderIP;
        private System.Windows.Forms.Button btnStart1;
        private System.Windows.Forms.Button btnStart2;
        private System.Windows.Forms.Button button1;

    }
}