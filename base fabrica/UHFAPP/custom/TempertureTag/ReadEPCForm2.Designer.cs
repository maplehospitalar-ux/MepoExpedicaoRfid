namespace UHFAPP
{
    partial class ReadEPCForm2
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
            this.columnHeaderRssi = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderCount = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderANT = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.qqqqqqqqqqqToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lto = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.filerLen = new System.Windows.Forms.TextBox();
            this.txtPtr = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbUser = new System.Windows.Forms.RadioButton();
            this.rbEPC = new System.Windows.Forms.RadioButton();
            this.rbTID = new System.Windows.Forms.RadioButton();
            this.cbSave = new System.Windows.Forms.CheckBox();
            this.btnSet = new System.Windows.Forms.Button();
            this.txtData = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnScanEPC
            // 
            this.btnScanEPC.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnScanEPC.Location = new System.Drawing.Point(552, 553);
            this.btnScanEPC.Name = "btnScanEPC";
            this.btnScanEPC.Size = new System.Drawing.Size(93, 48);
            this.btnScanEPC.TabIndex = 0;
            this.btnScanEPC.Text = "Start";
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
            this.columnHeaderRssi,
            this.columnHeaderCount,
            this.columnHeaderANT});
            this.lvEPC.ContextMenuStrip = this.contextMenuStrip1;
            this.lvEPC.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvEPC.FullRowSelect = true;
            this.lvEPC.Location = new System.Drawing.Point(0, 94);
            this.lvEPC.Name = "lvEPC";
            this.lvEPC.Size = new System.Drawing.Size(1287, 438);
            this.lvEPC.TabIndex = 2;
            this.lvEPC.UseCompatibleStateImageBehavior = false;
            this.lvEPC.View = System.Windows.Forms.View.Details;
            this.lvEPC.DoubleClick += new System.EventHandler(this.lvEPC_DoubleClick);
            // 
            // columnHeaderID
            // 
            this.columnHeaderID.Text = "ID";
            this.columnHeaderID.Width = 77;
            // 
            // columnHeaderEPC
            // 
            this.columnHeaderEPC.Text = "EPC";
            this.columnHeaderEPC.Width = 700;
            // 
            // columnHeaderTID
            // 
            this.columnHeaderTID.Text = "TID";
            this.columnHeaderTID.Width = 280;
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
            this.lto.Location = new System.Drawing.Point(56, 553);
            this.lto.Name = "lto";
            this.lto.Size = new System.Drawing.Size(75, 19);
            this.lto.TabIndex = 3;
            this.lto.Text = "Total:";
            this.lto.Click += new System.EventHandler(this.lto_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(230, 586);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "Time:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(688, 553);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 48);
            this.button1.TabIndex = 24;
            this.button1.Text = "Clear";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.BackColor = System.Drawing.Color.Transparent;
            this.groupBox8.Controls.Add(this.label5);
            this.groupBox8.Controls.Add(this.filerLen);
            this.groupBox8.Controls.Add(this.txtPtr);
            this.groupBox8.Controls.Add(this.label3);
            this.groupBox8.Controls.Add(this.label1);
            this.groupBox8.Controls.Add(this.button2);
            this.groupBox8.Controls.Add(this.groupBox1);
            this.groupBox8.Controls.Add(this.cbSave);
            this.groupBox8.Controls.Add(this.btnSet);
            this.groupBox8.Controls.Add(this.txtData);
            this.groupBox8.Controls.Add(this.label29);
            this.groupBox8.Controls.Add(this.label30);
            this.groupBox8.Controls.Add(this.label4);
            this.groupBox8.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox8.ForeColor = System.Drawing.Color.Black;
            this.groupBox8.Location = new System.Drawing.Point(0, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(1287, 74);
            this.groupBox8.TabIndex = 30;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Filter";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(612, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 16);
            this.label5.TabIndex = 38;
            this.label5.Text = "0";
            // 
            // filerLen
            // 
            this.filerLen.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.filerLen.Location = new System.Drawing.Point(846, 28);
            this.filerLen.MaxLength = 4;
            this.filerLen.Name = "filerLen";
            this.filerLen.Size = new System.Drawing.Size(49, 26);
            this.filerLen.TabIndex = 34;
            this.filerLen.Tag = "Number";
            this.filerLen.Text = "0";
            this.filerLen.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // txtPtr
            // 
            this.txtPtr.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPtr.Location = new System.Drawing.Point(716, 29);
            this.txtPtr.MaxLength = 4;
            this.txtPtr.Name = "txtPtr";
            this.txtPtr.Size = new System.Drawing.Size(49, 26);
            this.txtPtr.TabIndex = 6;
            this.txtPtr.Tag = "Number";
            this.txtPtr.Text = "32";
            this.txtPtr.TextChanged += new System.EventHandler(this.txtPtr_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(761, 37);
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
            this.label1.Location = new System.Drawing.Point(801, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 33;
            this.label1.Text = "长度:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.Location = new System.Drawing.Point(1207, 42);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(69, 29);
            this.button2.TabIndex = 32;
            this.button2.Text = "reset";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbUser);
            this.groupBox1.Controls.Add(this.rbEPC);
            this.groupBox1.Controls.Add(this.rbTID);
            this.groupBox1.Location = new System.Drawing.Point(932, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(181, 47);
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
            // cbSave
            // 
            this.cbSave.AutoSize = true;
            this.cbSave.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbSave.Location = new System.Drawing.Point(1119, 35);
            this.cbSave.Name = "cbSave";
            this.cbSave.Size = new System.Drawing.Size(59, 20);
            this.cbSave.TabIndex = 11;
            this.cbSave.Text = "Save";
            this.cbSave.UseVisualStyleBackColor = true;
            // 
            // btnSet
            // 
            this.btnSet.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSet.ForeColor = System.Drawing.Color.Black;
            this.btnSet.Location = new System.Drawing.Point(1207, 14);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(69, 29);
            this.btnSet.TabIndex = 10;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // txtData
            // 
            this.txtData.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtData.Location = new System.Drawing.Point(50, 20);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtData.Size = new System.Drawing.Size(558, 41);
            this.txtData.TabIndex = 7;
            this.txtData.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label29.Location = new System.Drawing.Point(6, 30);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(48, 16);
            this.label29.TabIndex = 5;
            this.label29.Text = "Data:";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label30.Location = new System.Drawing.Point(670, 34);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(40, 16);
            this.label30.TabIndex = 4;
            this.label30.Text = "Ptr:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(893, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 14);
            this.label4.TabIndex = 36;
            this.label4.Text = "(bit)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.lblTotal);
            this.panel1.Controls.Add(this.lblTime);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.lvEPC);
            this.panel1.Controls.Add(this.groupBox8);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnScanEPC);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lto);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1296, 629);
            this.panel1.TabIndex = 31;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(433, 553);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(23, 24);
            this.label9.TabIndex = 37;
            this.label9.Text = "0";
            this.label9.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(375, 556);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 19);
            this.label8.TabIndex = 36;
            this.label8.Text = "速率:";
            this.label8.Visible = false;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(163, 551);
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
            this.lblTime.Location = new System.Drawing.Point(305, 586);
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
            this.label6.Location = new System.Drawing.Point(304, 551);
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
            this.label7.Location = new System.Drawing.Point(229, 553);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 19);
            this.label7.TabIndex = 32;
            this.label7.Text = "次数:";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button3.Location = new System.Drawing.Point(810, 549);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(156, 48);
            this.button3.TabIndex = 31;
            this.button3.Text = "自动";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // ReadEPCForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(1290, 631);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.Black;
            this.KeyPreview = true;
            this.Name = "ReadEPCForm2";
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
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.RadioButton rbTID;
        private System.Windows.Forms.RadioButton rbEPC;
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.TextBox txtPtr;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.ColumnHeader columnHeaderTID;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ColumnHeader columnHeaderRssi;
        private System.Windows.Forms.ColumnHeader columnHeaderCount;
        private System.Windows.Forms.CheckBox cbSave;
        private System.Windows.Forms.RadioButton rbUser;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox filerLen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ColumnHeader columnHeaderANT;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem qqqqqqqqqqqToolStripMenuItem;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
    }
}