namespace UHFAPP
{
    partial class TempertureTag2ReadEPCForm
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
            this.btnScanEPC = new System.Windows.Forms.Button();
            this.lvEPC = new System.Windows.Forms.ListView();
            this.columnHeaderID = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderEPC = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderTID = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderTemperature = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderRssi = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderCount = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderANT = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.qqqqqqqqqqqToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lto = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.txtData = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtfilerLen = new System.Windows.Forms.TextBox();
            this.txtPtr = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbUser = new System.Windows.Forms.RadioButton();
            this.rbEPC = new System.Windows.Forms.RadioButton();
            this.rbTID = new System.Windows.Forms.RadioButton();
            this.label29 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.button6 = new System.Windows.Forms.Button();
            this.gbInventoryMode = new System.Windows.Forms.GroupBox();
            this.cbInventoryMode = new System.Windows.Forms.ComboBox();
            this.label45 = new System.Windows.Forms.Label();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label39 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.txtinterval = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtdelay = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtMax = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtMin = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.gbInventoryMode.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnScanEPC
            // 
            this.btnScanEPC.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnScanEPC.Location = new System.Drawing.Point(240, 513);
            this.btnScanEPC.Name = "btnScanEPC";
            this.btnScanEPC.Size = new System.Drawing.Size(154, 48);
            this.btnScanEPC.TabIndex = 0;
            this.btnScanEPC.Text = "盘点温度";
            this.btnScanEPC.UseVisualStyleBackColor = true;
            this.btnScanEPC.Click += new System.EventHandler(this.btnScanEPC_Click);
            // 
            // lvEPC
            // 
            this.lvEPC.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lvEPC.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderID,
            this.columnHeaderEPC,
            this.columnHeaderTID,
            this.columnHeaderTemperature,
            this.columnHeader1,
            this.columnHeaderRssi,
            this.columnHeaderCount,
            this.columnHeaderANT});
            this.lvEPC.ContextMenuStrip = this.contextMenuStrip1;
            this.lvEPC.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvEPC.FullRowSelect = true;
            this.lvEPC.Location = new System.Drawing.Point(3, 10);
            this.lvEPC.Name = "lvEPC";
            this.lvEPC.Size = new System.Drawing.Size(708, 412);
            this.lvEPC.TabIndex = 2;
            this.lvEPC.UseCompatibleStateImageBehavior = false;
            this.lvEPC.View = System.Windows.Forms.View.Details;
            this.lvEPC.DoubleClick += new System.EventHandler(this.lvEPC_DoubleClick);
            // 
            // columnHeaderID
            // 
            this.columnHeaderID.Text = "ID";
            this.columnHeaderID.Width = 28;
            // 
            // columnHeaderEPC
            // 
            this.columnHeaderEPC.Text = "EPC";
            this.columnHeaderEPC.Width = 213;
            // 
            // columnHeaderTID
            // 
            this.columnHeaderTID.Text = "TID";
            this.columnHeaderTID.Width = 213;
            // 
            // columnHeaderTemperature
            // 
            this.columnHeaderTemperature.Text = "温度";
            this.columnHeaderTemperature.Width = 55;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "温度测量次数";
            this.columnHeader1.Width = 108;
            // 
            // columnHeaderRssi
            // 
            this.columnHeaderRssi.Text = "Rssi";
            // 
            // columnHeaderCount
            // 
            this.columnHeaderCount.Text = "Count";
            // 
            // columnHeaderANT
            // 
            this.columnHeaderANT.Text = "ANT";
            this.columnHeaderANT.Width = 40;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.qqqqqqqqqqqToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowCheckMargin = true;
            this.contextMenuStrip1.Size = new System.Drawing.Size(187, 26);
            this.contextMenuStrip1.Click += new System.EventHandler(this.contextMenuStrip1_Click);
            // 
            // qqqqqqqqqqqToolStripMenuItem
            // 
            this.qqqqqqqqqqqToolStripMenuItem.Name = "qqqqqqqqqqqToolStripMenuItem";
            this.qqqqqqqqqqqToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.qqqqqqqqqqqToolStripMenuItem.Text = "qqqqqqqqqqq";
            // 
            // lto
            // 
            this.lto.AutoSize = true;
            this.lto.BackColor = System.Drawing.Color.Transparent;
            this.lto.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lto.ForeColor = System.Drawing.Color.Black;
            this.lto.Location = new System.Drawing.Point(24, 467);
            this.lto.Name = "lto";
            this.lto.Size = new System.Drawing.Size(75, 19);
            this.lto.TabIndex = 3;
            this.lto.Text = "Total:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(24, 565);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "Time:";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(445, 513);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 45);
            this.button1.TabIndex = 24;
            this.button1.Text = "清空";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.BackColor = System.Drawing.Color.Transparent;
            this.groupBox8.Controls.Add(this.txtData);
            this.groupBox8.Controls.Add(this.label5);
            this.groupBox8.Controls.Add(this.txtfilerLen);
            this.groupBox8.Controls.Add(this.txtPtr);
            this.groupBox8.Controls.Add(this.label3);
            this.groupBox8.Controls.Add(this.label1);
            this.groupBox8.Controls.Add(this.groupBox1);
            this.groupBox8.Controls.Add(this.label29);
            this.groupBox8.Controls.Add(this.label4);
            this.groupBox8.Controls.Add(this.label30);
            this.groupBox8.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox8.ForeColor = System.Drawing.Color.Black;
            this.groupBox8.Location = new System.Drawing.Point(717, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(572, 117);
            this.groupBox8.TabIndex = 30;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Filter";
            // 
            // txtData
            // 
            this.txtData.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtData.Location = new System.Drawing.Point(46, 20);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtData.Size = new System.Drawing.Size(360, 41);
            this.txtData.TabIndex = 7;
            this.txtData.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(407, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 16);
            this.label5.TabIndex = 38;
            this.label5.Text = "0";
            // 
            // txtfilerLen
            // 
            this.txtfilerLen.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtfilerLen.Location = new System.Drawing.Point(481, 41);
            this.txtfilerLen.MaxLength = 4;
            this.txtfilerLen.Name = "txtfilerLen";
            this.txtfilerLen.Size = new System.Drawing.Size(36, 26);
            this.txtfilerLen.TabIndex = 34;
            this.txtfilerLen.Tag = "Number";
            this.txtfilerLen.Text = "0";
            this.txtfilerLen.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // txtPtr
            // 
            this.txtPtr.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPtr.Location = new System.Drawing.Point(482, 12);
            this.txtPtr.MaxLength = 4;
            this.txtPtr.Name = "txtPtr";
            this.txtPtr.Size = new System.Drawing.Size(36, 26);
            this.txtPtr.TabIndex = 6;
            this.txtPtr.Tag = "Number";
            this.txtPtr.Text = "32";
            this.txtPtr.TextChanged += new System.EventHandler(this.txtPtr_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(513, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 14);
            this.label3.TabIndex = 35;
            this.label3.Text = "(bit)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(449, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 16);
            this.label1.TabIndex = 33;
            this.label1.Text = "Len:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbUser);
            this.groupBox1.Controls.Add(this.rbEPC);
            this.groupBox1.Controls.Add(this.rbTID);
            this.groupBox1.Location = new System.Drawing.Point(9, 64);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(534, 47);
            this.groupBox1.TabIndex = 31;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "bank";
            // 
            // rbUser
            // 
            this.rbUser.AutoSize = true;
            this.rbUser.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbUser.Location = new System.Drawing.Point(118, 20);
            this.rbUser.Name = "rbUser";
            this.rbUser.Size = new System.Drawing.Size(58, 20);
            this.rbUser.TabIndex = 12;
            this.rbUser.Text = "User";
            this.rbUser.UseVisualStyleBackColor = true;
            this.rbUser.Click += new System.EventHandler(this.rbUser_Click);
            // 
            // rbEPC
            // 
            this.rbEPC.AutoSize = true;
            this.rbEPC.Checked = true;
            this.rbEPC.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbEPC.Location = new System.Drawing.Point(6, 19);
            this.rbEPC.Name = "rbEPC";
            this.rbEPC.Size = new System.Drawing.Size(50, 20);
            this.rbEPC.TabIndex = 8;
            this.rbEPC.TabStop = true;
            this.rbEPC.Text = "EPC";
            this.rbEPC.UseVisualStyleBackColor = true;
            this.rbEPC.Click += new System.EventHandler(this.rbEPC_Click);
            // 
            // rbTID
            // 
            this.rbTID.AutoSize = true;
            this.rbTID.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbTID.Location = new System.Drawing.Point(62, 20);
            this.rbTID.Name = "rbTID";
            this.rbTID.Size = new System.Drawing.Size(50, 20);
            this.rbTID.TabIndex = 9;
            this.rbTID.Text = "TID";
            this.rbTID.UseVisualStyleBackColor = true;
            this.rbTID.Click += new System.EventHandler(this.rbTID_Click);
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label29.Location = new System.Drawing.Point(2, 30);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(48, 16);
            this.label29.TabIndex = 5;
            this.label29.Text = "Data:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(513, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 14);
            this.label4.TabIndex = 36;
            this.label4.Text = "(bit)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label30.Location = new System.Drawing.Point(449, 15);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(40, 16);
            this.label30.TabIndex = 4;
            this.label30.Text = "Ptr:";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.groupBox5);
            this.panel1.Controls.Add(this.gbInventoryMode);
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.lblTotal);
            this.panel1.Controls.Add(this.lblTime);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.lvEPC);
            this.panel1.Controls.Add(this.groupBox8);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnScanEPC);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lto);
            this.panel1.Location = new System.Drawing.Point(0, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1296, 629);
            this.panel1.TabIndex = 31;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.button6);
            this.groupBox5.Location = new System.Drawing.Point(1033, 213);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(213, 55);
            this.groupBox5.TabIndex = 74;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "模式检查";
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button6.Location = new System.Drawing.Point(45, 17);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(93, 32);
            this.button6.TabIndex = 47;
            this.button6.Text = "Get";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // gbInventoryMode
            // 
            this.gbInventoryMode.Controls.Add(this.cbInventoryMode);
            this.gbInventoryMode.Controls.Add(this.label45);
            this.gbInventoryMode.Controls.Add(this.button10);
            this.gbInventoryMode.Controls.Add(this.button11);
            this.gbInventoryMode.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gbInventoryMode.Location = new System.Drawing.Point(717, 389);
            this.gbInventoryMode.Name = "gbInventoryMode";
            this.gbInventoryMode.Size = new System.Drawing.Size(543, 65);
            this.gbInventoryMode.TabIndex = 73;
            this.gbInventoryMode.TabStop = false;
            // 
            // cbInventoryMode
            // 
            this.cbInventoryMode.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbInventoryMode.FormattingEnabled = true;
            this.cbInventoryMode.Items.AddRange(new object[] {
            "EPC",
            "EPC+TID+温度标签"});
            this.cbInventoryMode.Location = new System.Drawing.Point(113, 25);
            this.cbInventoryMode.Name = "cbInventoryMode";
            this.cbInventoryMode.Size = new System.Drawing.Size(197, 24);
            this.cbInventoryMode.TabIndex = 66;
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label45.Location = new System.Drawing.Point(7, 29);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(48, 16);
            this.label45.TabIndex = 66;
            this.label45.Text = "Mode:";
            // 
            // button10
            // 
            this.button10.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button10.ForeColor = System.Drawing.Color.Black;
            this.button10.Location = new System.Drawing.Point(316, 22);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(90, 31);
            this.button10.TabIndex = 30;
            this.button10.Text = "Set";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            this.button11.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button11.ForeColor = System.Drawing.Color.Black;
            this.button11.Location = new System.Drawing.Point(417, 22);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(90, 31);
            this.button11.TabIndex = 29;
            this.button11.Text = "Get";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label39);
            this.groupBox4.Controls.Add(this.button4);
            this.groupBox4.Controls.Add(this.button5);
            this.groupBox4.Location = new System.Drawing.Point(717, 274);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(543, 109);
            this.groupBox4.TabIndex = 43;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "温度标签";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(370, 56);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(53, 12);
            this.label39.TabIndex = 37;
            this.label39.Text = "温度：--";
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button4.ForeColor = System.Drawing.Color.Black;
            this.button4.Location = new System.Drawing.Point(174, 35);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(121, 47);
            this.button4.TabIndex = 36;
            this.button4.Text = "即时单次测温";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button5.ForeColor = System.Drawing.Color.Black;
            this.button5.Location = new System.Drawing.Point(25, 35);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(123, 47);
            this.button5.TabIndex = 35;
            this.button5.Text = "初始化温度标签";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtPwd);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Location = new System.Drawing.Point(1033, 128);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(245, 79);
            this.groupBox3.TabIndex = 42;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "停止测温";
            // 
            // txtPwd
            // 
            this.txtPwd.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPwd.Location = new System.Drawing.Point(45, 29);
            this.txtPwd.MaxLength = 4;
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.Size = new System.Drawing.Size(83, 26);
            this.txtPwd.TabIndex = 39;
            this.txtPwd.Tag = "";
            this.txtPwd.Text = "00000000";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(6, 34);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(40, 16);
            this.label11.TabIndex = 39;
            this.label11.Text = "密码";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.Location = new System.Drawing.Point(129, 28);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 29);
            this.button2.TabIndex = 0;
            this.button2.Text = "StopLogging";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.txtinterval);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.txtdelay);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.txtMax);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.txtMin);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Location = new System.Drawing.Point(717, 126);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(310, 142);
            this.groupBox2.TabIndex = 40;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "开始测温";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.Location = new System.Drawing.Point(174, 50);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(24, 16);
            this.label16.TabIndex = 50;
            this.label16.Text = "℃";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.Location = new System.Drawing.Point(174, 22);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(24, 16);
            this.label15.TabIndex = 49;
            this.label15.Text = "℃";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(174, 74);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(24, 16);
            this.label14.TabIndex = 48;
            this.label14.Text = "分";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(175, 103);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(24, 16);
            this.label10.TabIndex = 47;
            this.label10.Text = "秒";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button3.Location = new System.Drawing.Point(204, 47);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(93, 32);
            this.button3.TabIndex = 46;
            this.button3.Text = "StartLogging";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtinterval
            // 
            this.txtinterval.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtinterval.Location = new System.Drawing.Point(123, 98);
            this.txtinterval.MaxLength = 4;
            this.txtinterval.Name = "txtinterval";
            this.txtinterval.Size = new System.Drawing.Size(49, 26);
            this.txtinterval.TabIndex = 45;
            this.txtinterval.Tag = "";
            this.txtinterval.Text = "60";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(13, 103);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(80, 16);
            this.label12.TabIndex = 44;
            this.label12.Text = "间隔时间:";
            // 
            // txtdelay
            // 
            this.txtdelay.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtdelay.Location = new System.Drawing.Point(123, 70);
            this.txtdelay.MaxLength = 4;
            this.txtdelay.Name = "txtdelay";
            this.txtdelay.Size = new System.Drawing.Size(49, 26);
            this.txtdelay.TabIndex = 42;
            this.txtdelay.Tag = "";
            this.txtdelay.Text = "0";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(12, 75);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(112, 16);
            this.label13.TabIndex = 43;
            this.label13.Text = "延时测温时间:";
            // 
            // txtMax
            // 
            this.txtMax.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtMax.Location = new System.Drawing.Point(123, 42);
            this.txtMax.MaxLength = 4;
            this.txtMax.Name = "txtMax";
            this.txtMax.Size = new System.Drawing.Size(49, 26);
            this.txtMax.TabIndex = 41;
            this.txtMax.Tag = "";
            this.txtMax.Text = "50";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(13, 47);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 16);
            this.label9.TabIndex = 40;
            this.label9.Text = "最高温度";
            // 
            // txtMin
            // 
            this.txtMin.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtMin.Location = new System.Drawing.Point(123, 14);
            this.txtMin.MaxLength = 4;
            this.txtMin.Name = "txtMin";
            this.txtMin.Size = new System.Drawing.Size(49, 26);
            this.txtMin.TabIndex = 39;
            this.txtMin.Tag = "";
            this.txtMin.Text = "-20";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(13, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 16);
            this.label8.TabIndex = 39;
            this.label8.Text = "最低温度";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(104, 462);
            this.lblTotal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(23, 24);
            this.lblTotal.TabIndex = 35;
            this.lblTotal.Text = "0";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.lblTime.Location = new System.Drawing.Point(104, 565);
            this.lblTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(23, 24);
            this.lblTime.TabIndex = 34;
            this.lblTime.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.label6.Location = new System.Drawing.Point(104, 513);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 24);
            this.label6.TabIndex = 33;
            this.label6.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(24, 513);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 19);
            this.label7.TabIndex = 32;
            this.label7.Text = "次数:";
            // 
            // TempertureTag2ReadEPCForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(1290, 631);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.Black;
            this.KeyPreview = true;
            this.Name = "TempertureTag2ReadEPCForm";
            this.Text = "ReadEPC";
            this.Load += new System.EventHandler(this.ScanEPCForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScanEPCForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ReadEPCForm_KeyDown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.gbInventoryMode.ResumeLayout(false);
            this.gbInventoryMode.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnScanEPC;
        private System.Windows.Forms.ListView lvEPC;
        private System.Windows.Forms.ColumnHeader columnHeaderID;
        private System.Windows.Forms.ColumnHeader columnHeaderEPC;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lto;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.RadioButton rbTID;
        private System.Windows.Forms.RadioButton rbEPC;
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.TextBox txtPtr;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.ColumnHeader columnHeaderTemperature;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ColumnHeader columnHeaderRssi;
        private System.Windows.Forms.ColumnHeader columnHeaderCount;
        private System.Windows.Forms.RadioButton rbUser;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtfilerLen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ColumnHeader columnHeaderANT;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem qqqqqqqqqqqToolStripMenuItem;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtMax;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtMin;
        private System.Windows.Forms.TextBox txtinterval;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtdelay;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.GroupBox gbInventoryMode;
        private System.Windows.Forms.ComboBox cbInventoryMode;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeaderTID;
    }
}