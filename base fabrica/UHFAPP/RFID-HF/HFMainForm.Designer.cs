namespace UHFAPP.RFID
{
    partial class RFIDMainForm
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
            this.cmbM1KeyType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbM1TagType = new System.Windows.Forms.ComboBox();
            this.lblM1TagType = new System.Windows.Forms.Label();
            this.txtM1Data = new System.Windows.Forms.TextBox();
            this.cmbM1Block = new System.Windows.Forms.ComboBox();
            this.lblM1Block = new System.Windows.Forms.Label();
            this.cmbM1Sector = new System.Windows.Forms.ComboBox();
            this.lblM1Sector = new System.Windows.Forms.Label();
            this.lblM1Key = new System.Windows.Forms.Label();
            this.txtM1Key = new System.Windows.Forms.TextBox();
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnRead = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnDsfidLock = new System.Windows.Forms.Button();
            this.btnAFILock = new System.Windows.Forms.Button();
            this.txtDsfid = new System.Windows.Forms.TextBox();
            this.btn15693Write = new System.Windows.Forms.Button();
            this.btn15693Read = new System.Windows.Forms.Button();
            this.btnDsfidWrite = new System.Windows.Forms.Button();
            this.btnAFIWrite = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAFI = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt15693Data = new System.Windows.Forms.TextBox();
            this.cmb15693Block = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbl15693Block = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtPsamReceive = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnPsamSend = new System.Windows.Forms.Button();
            this.btnFree = new System.Windows.Forms.Button();
            this.btnInit = new System.Windows.Forms.Button();
            this.txtPsamData = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.rbCard2 = new System.Windows.Forms.RadioButton();
            this.rbCard1 = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txt14443BSendData = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.btn14443B = new System.Windows.Forms.Button();
            this.btnGetUID = new System.Windows.Forms.Button();
            this.txt14443BReceive = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtCPUReceive = new System.Windows.Forms.TextBox();
            this.lblMSG = new System.Windows.Forms.Label();
            this.btn14443ACPUInit = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txt14443ACPUData = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.txtUID = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbM1KeyType);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbM1TagType);
            this.groupBox1.Controls.Add(this.lblM1TagType);
            this.groupBox1.Controls.Add(this.txtM1Data);
            this.groupBox1.Controls.Add(this.cmbM1Block);
            this.groupBox1.Controls.Add(this.lblM1Block);
            this.groupBox1.Controls.Add(this.cmbM1Sector);
            this.groupBox1.Controls.Add(this.lblM1Sector);
            this.groupBox1.Controls.Add(this.lblM1Key);
            this.groupBox1.Controls.Add(this.txtM1Key);
            this.groupBox1.Controls.Add(this.btnWrite);
            this.groupBox1.Controls.Add(this.btnRead);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(6, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(350, 285);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "14443A";
            // 
            // cmbM1KeyType
            // 
            this.cmbM1KeyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbM1KeyType.FormattingEnabled = true;
            this.cmbM1KeyType.Items.AddRange(new object[] {
            "A",
            "B"});
            this.cmbM1KeyType.Location = new System.Drawing.Point(245, 31);
            this.cmbM1KeyType.Name = "cmbM1KeyType";
            this.cmbM1KeyType.Size = new System.Drawing.Size(64, 24);
            this.cmbM1KeyType.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(160, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 11;
            this.label1.Text = "Key Type:";
            // 
            // cmbM1TagType
            // 
            this.cmbM1TagType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbM1TagType.FormattingEnabled = true;
            this.cmbM1TagType.Items.AddRange(new object[] {
            "S50",
            "S70"});
            this.cmbM1TagType.Location = new System.Drawing.Point(88, 31);
            this.cmbM1TagType.Name = "cmbM1TagType";
            this.cmbM1TagType.Size = new System.Drawing.Size(64, 24);
            this.cmbM1TagType.TabIndex = 10;
            this.cmbM1TagType.SelectedIndexChanged += new System.EventHandler(this.cmbM1TagType_SelectedIndexChanged);
            // 
            // lblM1TagType
            // 
            this.lblM1TagType.AutoSize = true;
            this.lblM1TagType.Location = new System.Drawing.Point(6, 34);
            this.lblM1TagType.Name = "lblM1TagType";
            this.lblM1TagType.Size = new System.Drawing.Size(80, 16);
            this.lblM1TagType.TabIndex = 9;
            this.lblM1TagType.Text = "Tag Type:";
            // 
            // txtM1Data
            // 
            this.txtM1Data.Location = new System.Drawing.Point(9, 129);
            this.txtM1Data.Multiline = true;
            this.txtM1Data.Name = "txtM1Data";
            this.txtM1Data.Size = new System.Drawing.Size(317, 92);
            this.txtM1Data.TabIndex = 8;
            this.txtM1Data.TextChanged += new System.EventHandler(this.txtM1Data_TextChanged);
            // 
            // cmbM1Block
            // 
            this.cmbM1Block.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbM1Block.FormattingEnabled = true;
            this.cmbM1Block.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3"});
            this.cmbM1Block.Location = new System.Drawing.Point(245, 60);
            this.cmbM1Block.Name = "cmbM1Block";
            this.cmbM1Block.Size = new System.Drawing.Size(64, 24);
            this.cmbM1Block.TabIndex = 7;
            // 
            // lblM1Block
            // 
            this.lblM1Block.AutoSize = true;
            this.lblM1Block.Location = new System.Drawing.Point(178, 63);
            this.lblM1Block.Name = "lblM1Block";
            this.lblM1Block.Size = new System.Drawing.Size(56, 16);
            this.lblM1Block.TabIndex = 6;
            this.lblM1Block.Text = "Block:";
            // 
            // cmbM1Sector
            // 
            this.cmbM1Sector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbM1Sector.FormattingEnabled = true;
            this.cmbM1Sector.Items.AddRange(new object[] {
            "0",
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
            "15"});
            this.cmbM1Sector.Location = new System.Drawing.Point(88, 60);
            this.cmbM1Sector.Name = "cmbM1Sector";
            this.cmbM1Sector.Size = new System.Drawing.Size(64, 24);
            this.cmbM1Sector.TabIndex = 5;
            this.cmbM1Sector.SelectedIndexChanged += new System.EventHandler(this.cmbM1Sector_SelectedIndexChanged);
            // 
            // lblM1Sector
            // 
            this.lblM1Sector.AutoSize = true;
            this.lblM1Sector.Location = new System.Drawing.Point(6, 63);
            this.lblM1Sector.Name = "lblM1Sector";
            this.lblM1Sector.Size = new System.Drawing.Size(64, 16);
            this.lblM1Sector.TabIndex = 4;
            this.lblM1Sector.Text = "Sector:";
            // 
            // lblM1Key
            // 
            this.lblM1Key.AutoSize = true;
            this.lblM1Key.Location = new System.Drawing.Point(6, 99);
            this.lblM1Key.Name = "lblM1Key";
            this.lblM1Key.Size = new System.Drawing.Size(40, 16);
            this.lblM1Key.TabIndex = 3;
            this.lblM1Key.Text = "Key:";
            // 
            // txtM1Key
            // 
            this.txtM1Key.Location = new System.Drawing.Point(88, 96);
            this.txtM1Key.Name = "txtM1Key";
            this.txtM1Key.Size = new System.Drawing.Size(221, 26);
            this.txtM1Key.TabIndex = 2;
            this.txtM1Key.Text = "FFFFFFFFFFFF";
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new System.Drawing.Point(191, 227);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(83, 32);
            this.btnWrite.TabIndex = 1;
            this.btnWrite.Text = "Write";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(31, 227);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(83, 32);
            this.btnRead.TabIndex = 0;
            this.btnRead.Text = "Read";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnDsfidLock);
            this.groupBox2.Controls.Add(this.btnAFILock);
            this.groupBox2.Controls.Add(this.txtDsfid);
            this.groupBox2.Controls.Add(this.btn15693Write);
            this.groupBox2.Controls.Add(this.btn15693Read);
            this.groupBox2.Controls.Add(this.btnDsfidWrite);
            this.groupBox2.Controls.Add(this.btnAFIWrite);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtAFI);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txt15693Data);
            this.groupBox2.Controls.Add(this.cmb15693Block);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.lbl15693Block);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(393, 14);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(403, 283);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "15693";
            // 
            // btnDsfidLock
            // 
            this.btnDsfidLock.Location = new System.Drawing.Point(268, 197);
            this.btnDsfidLock.Name = "btnDsfidLock";
            this.btnDsfidLock.Size = new System.Drawing.Size(67, 28);
            this.btnDsfidLock.TabIndex = 19;
            this.btnDsfidLock.Text = "Lock";
            this.btnDsfidLock.UseVisualStyleBackColor = true;
            this.btnDsfidLock.Click += new System.EventHandler(this.btnDsfidLock_Click);
            // 
            // btnAFILock
            // 
            this.btnAFILock.Location = new System.Drawing.Point(268, 156);
            this.btnAFILock.Name = "btnAFILock";
            this.btnAFILock.Size = new System.Drawing.Size(67, 28);
            this.btnAFILock.TabIndex = 18;
            this.btnAFILock.Text = "Lock";
            this.btnAFILock.UseVisualStyleBackColor = true;
            this.btnAFILock.Click += new System.EventHandler(this.btnAFILock_Click);
            // 
            // txtDsfid
            // 
            this.txtDsfid.Location = new System.Drawing.Point(66, 196);
            this.txtDsfid.Name = "txtDsfid";
            this.txtDsfid.Size = new System.Drawing.Size(53, 26);
            this.txtDsfid.TabIndex = 17;
            // 
            // btn15693Write
            // 
            this.btn15693Write.Location = new System.Drawing.Point(165, 104);
            this.btn15693Write.Name = "btn15693Write";
            this.btn15693Write.Size = new System.Drawing.Size(67, 32);
            this.btn15693Write.TabIndex = 11;
            this.btn15693Write.Text = "Write";
            this.btn15693Write.UseVisualStyleBackColor = true;
            this.btn15693Write.Click += new System.EventHandler(this.btn15693Write_Click);
            // 
            // btn15693Read
            // 
            this.btn15693Read.Location = new System.Drawing.Point(81, 104);
            this.btn15693Read.Name = "btn15693Read";
            this.btn15693Read.Size = new System.Drawing.Size(67, 33);
            this.btn15693Read.TabIndex = 10;
            this.btn15693Read.Text = "Read";
            this.btn15693Read.UseVisualStyleBackColor = true;
            this.btn15693Read.Click += new System.EventHandler(this.btn15693Read_Click);
            // 
            // btnDsfidWrite
            // 
            this.btnDsfidWrite.Location = new System.Drawing.Point(195, 196);
            this.btnDsfidWrite.Name = "btnDsfidWrite";
            this.btnDsfidWrite.Size = new System.Drawing.Size(67, 29);
            this.btnDsfidWrite.TabIndex = 16;
            this.btnDsfidWrite.Text = "Write";
            this.btnDsfidWrite.UseVisualStyleBackColor = true;
            this.btnDsfidWrite.Click += new System.EventHandler(this.btnDsfidWrite_Click);
            // 
            // btnAFIWrite
            // 
            this.btnAFIWrite.Location = new System.Drawing.Point(195, 156);
            this.btnAFIWrite.Name = "btnAFIWrite";
            this.btnAFIWrite.Size = new System.Drawing.Size(67, 28);
            this.btnAFIWrite.TabIndex = 15;
            this.btnAFIWrite.Text = "Write";
            this.btnAFIWrite.UseVisualStyleBackColor = true;
            this.btnAFIWrite.Click += new System.EventHandler(this.btnAFIWrite_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 202);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 16);
            this.label3.TabIndex = 14;
            this.label3.Text = "Dsfid:";
            // 
            // txtAFI
            // 
            this.txtAFI.Location = new System.Drawing.Point(66, 160);
            this.txtAFI.Name = "txtAFI";
            this.txtAFI.Size = new System.Drawing.Size(53, 26);
            this.txtAFI.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 163);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 16);
            this.label2.TabIndex = 13;
            this.label2.Text = "AFI:";
            // 
            // txt15693Data
            // 
            this.txt15693Data.Location = new System.Drawing.Point(66, 53);
            this.txt15693Data.Multiline = true;
            this.txt15693Data.Name = "txt15693Data";
            this.txt15693Data.Size = new System.Drawing.Size(269, 45);
            this.txt15693Data.TabIndex = 12;
            this.txt15693Data.TextChanged += new System.EventHandler(this.txt15693Data_TextChanged);
            // 
            // cmb15693Block
            // 
            this.cmb15693Block.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb15693Block.FormattingEnabled = true;
            this.cmb15693Block.Items.AddRange(new object[] {
            "0",
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
            "27"});
            this.cmb15693Block.Location = new System.Drawing.Point(66, 26);
            this.cmb15693Block.Name = "cmb15693Block";
            this.cmb15693Block.Size = new System.Drawing.Size(64, 24);
            this.cmb15693Block.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(115, 166);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 12);
            this.label5.TabIndex = 21;
            this.label5.Text = "(1byte hex)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(114, 202);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 14);
            this.label6.TabIndex = 22;
            this.label6.Text = "(1byte hex)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 16);
            this.label4.TabIndex = 20;
            this.label4.Text = "Data:";
            // 
            // lbl15693Block
            // 
            this.lbl15693Block.AutoSize = true;
            this.lbl15693Block.Location = new System.Drawing.Point(13, 29);
            this.lbl15693Block.Name = "lbl15693Block";
            this.lbl15693Block.Size = new System.Drawing.Size(56, 16);
            this.lbl15693Block.TabIndex = 8;
            this.lbl15693Block.Text = "Block:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtPsamReceive);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.btnPsamSend);
            this.groupBox3.Controls.Add(this.btnFree);
            this.groupBox3.Controls.Add(this.btnInit);
            this.groupBox3.Controls.Add(this.txtPsamData);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.rbCard2);
            this.groupBox3.Controls.Add(this.rbCard1);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(828, 14);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(388, 296);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "PSAM";
            // 
            // txtPsamReceive
            // 
            this.txtPsamReceive.Location = new System.Drawing.Point(60, 217);
            this.txtPsamReceive.Multiline = true;
            this.txtPsamReceive.Name = "txtPsamReceive";
            this.txtPsamReceive.ReadOnly = true;
            this.txtPsamReceive.Size = new System.Drawing.Size(312, 66);
            this.txtPsamReceive.TabIndex = 32;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(21, 191);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 16);
            this.label10.TabIndex = 31;
            this.label10.Text = "Receive:";
            // 
            // btnPsamSend
            // 
            this.btnPsamSend.Enabled = false;
            this.btnPsamSend.Location = new System.Drawing.Point(318, 73);
            this.btnPsamSend.Name = "btnPsamSend";
            this.btnPsamSend.Size = new System.Drawing.Size(54, 42);
            this.btnPsamSend.TabIndex = 30;
            this.btnPsamSend.Text = "Send";
            this.btnPsamSend.UseVisualStyleBackColor = true;
            this.btnPsamSend.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnFree
            // 
            this.btnFree.Enabled = false;
            this.btnFree.Location = new System.Drawing.Point(183, 139);
            this.btnFree.Name = "btnFree";
            this.btnFree.Size = new System.Drawing.Size(97, 35);
            this.btnFree.TabIndex = 29;
            this.btnFree.Text = "free";
            this.btnFree.UseVisualStyleBackColor = true;
            this.btnFree.Click += new System.EventHandler(this.btnFree_Click);
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(60, 142);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(96, 34);
            this.btnInit.TabIndex = 28;
            this.btnInit.Text = "init";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // txtPsamData
            // 
            this.txtPsamData.Location = new System.Drawing.Point(60, 70);
            this.txtPsamData.Multiline = true;
            this.txtPsamData.Name = "txtPsamData";
            this.txtPsamData.Size = new System.Drawing.Size(252, 45);
            this.txtPsamData.TabIndex = 26;
            this.txtPsamData.Text = "0084000004";
            this.txtPsamData.TextChanged += new System.EventHandler(this.txtPsamData_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 82);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 16);
            this.label9.TabIndex = 27;
            this.label9.Text = "Data:";
            // 
            // rbCard2
            // 
            this.rbCard2.AutoSize = true;
            this.rbCard2.Location = new System.Drawing.Point(120, 34);
            this.rbCard2.Name = "rbCard2";
            this.rbCard2.Size = new System.Drawing.Size(66, 20);
            this.rbCard2.TabIndex = 25;
            this.rbCard2.Text = "Card2";
            this.rbCard2.UseVisualStyleBackColor = true;
            // 
            // rbCard1
            // 
            this.rbCard1.AutoSize = true;
            this.rbCard1.Checked = true;
            this.rbCard1.Location = new System.Drawing.Point(24, 32);
            this.rbCard1.Name = "rbCard1";
            this.rbCard1.Size = new System.Drawing.Size(66, 20);
            this.rbCard1.TabIndex = 24;
            this.rbCard1.TabStop = true;
            this.rbCard1.Text = "Card1";
            this.rbCard1.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 57);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(0, 16);
            this.label8.TabIndex = 9;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txt14443BReceive);
            this.groupBox4.Controls.Add(this.groupBox6);
            this.groupBox4.Controls.Add(this.txt14443BSendData);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.btn14443B);
            this.groupBox4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox4.Location = new System.Drawing.Point(393, 307);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(403, 289);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "14443B";
            // 
            // txt14443BSendData
            // 
            this.txt14443BSendData.Location = new System.Drawing.Point(74, 34);
            this.txt14443BSendData.Multiline = true;
            this.txt14443BSendData.Name = "txt14443BSendData";
            this.txt14443BSendData.Size = new System.Drawing.Size(243, 45);
            this.txt14443BSendData.TabIndex = 28;
            this.txt14443BSendData.Text = "0084000004";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(20, 46);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(48, 16);
            this.label13.TabIndex = 26;
            this.label13.Text = "Data:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 98);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(72, 16);
            this.label11.TabIndex = 26;
            this.label11.Text = "Receive:";
            // 
            // btn14443B
            // 
            this.btn14443B.Location = new System.Drawing.Point(321, 38);
            this.btn14443B.Name = "btn14443B";
            this.btn14443B.Size = new System.Drawing.Size(74, 32);
            this.btn14443B.TabIndex = 29;
            this.btn14443B.Text = "Send";
            this.btn14443B.UseVisualStyleBackColor = true;
            this.btn14443B.Click += new System.EventHandler(this.btn14443B_Click);
            // 
            // btnGetUID
            // 
            this.btnGetUID.Location = new System.Drawing.Point(287, 42);
            this.btnGetUID.Name = "btnGetUID";
            this.btnGetUID.Size = new System.Drawing.Size(78, 32);
            this.btnGetUID.TabIndex = 27;
            this.btnGetUID.Text = "GetUID";
            this.btnGetUID.UseVisualStyleBackColor = true;
            this.btnGetUID.Click += new System.EventHandler(this.btnGetUID_Click);
            // 
            // txt14443BReceive
            // 
            this.txt14443BReceive.Location = new System.Drawing.Point(77, 85);
            this.txt14443BReceive.Multiline = true;
            this.txt14443BReceive.Name = "txt14443BReceive";
            this.txt14443BReceive.ReadOnly = true;
            this.txt14443BReceive.Size = new System.Drawing.Size(240, 45);
            this.txt14443BReceive.TabIndex = 26;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtCPUReceive);
            this.groupBox5.Controls.Add(this.lblMSG);
            this.groupBox5.Controls.Add(this.btn14443ACPUInit);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.txt14443ACPUData);
            this.groupBox5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox5.Location = new System.Drawing.Point(6, 303);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(350, 293);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "14443A-CPU";
            // 
            // txtCPUReceive
            // 
            this.txtCPUReceive.Location = new System.Drawing.Point(57, 185);
            this.txtCPUReceive.Multiline = true;
            this.txtCPUReceive.Name = "txtCPUReceive";
            this.txtCPUReceive.ReadOnly = true;
            this.txtCPUReceive.Size = new System.Drawing.Size(269, 45);
            this.txtCPUReceive.TabIndex = 25;
            // 
            // lblMSG
            // 
            this.lblMSG.AutoSize = true;
            this.lblMSG.Location = new System.Drawing.Point(18, 159);
            this.lblMSG.Name = "lblMSG";
            this.lblMSG.Size = new System.Drawing.Size(72, 16);
            this.lblMSG.TabIndex = 24;
            this.lblMSG.Text = "Receive:";
            // 
            // btn14443ACPUInit
            // 
            this.btn14443ACPUInit.Location = new System.Drawing.Point(102, 104);
            this.btn14443ACPUInit.Name = "btn14443ACPUInit";
            this.btn14443ACPUInit.Size = new System.Drawing.Size(83, 32);
            this.btn14443ACPUInit.TabIndex = 23;
            this.btn14443ACPUInit.Text = "Send";
            this.btn14443ACPUInit.UseVisualStyleBackColor = true;
            this.btn14443ACPUInit.Click += new System.EventHandler(this.btn14443ACPUInit_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 50);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 16);
            this.label7.TabIndex = 22;
            this.label7.Text = "Data:";
            // 
            // txt14443ACPUData
            // 
            this.txt14443ACPUData.Location = new System.Drawing.Point(60, 38);
            this.txt14443ACPUData.Multiline = true;
            this.txt14443ACPUData.Name = "txt14443ACPUData";
            this.txt14443ACPUData.Size = new System.Drawing.Size(269, 45);
            this.txt14443ACPUData.TabIndex = 21;
            this.txt14443ACPUData.Text = "0084000004";
            this.txt14443ACPUData.TextChanged += new System.EventHandler(this.txt14443ACPUData_TextChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.txtUID);
            this.groupBox6.Controls.Add(this.btnGetUID);
            this.groupBox6.Location = new System.Drawing.Point(10, 152);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(385, 118);
            this.groupBox6.TabIndex = 30;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Identity Card";
            // 
            // txtUID
            // 
            this.txtUID.Location = new System.Drawing.Point(41, 38);
            this.txtUID.Multiline = true;
            this.txtUID.Name = "txtUID";
            this.txtUID.ReadOnly = true;
            this.txtUID.Size = new System.Drawing.Size(240, 45);
            this.txtUID.TabIndex = 31;
            // 
            // RFIDMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(1290, 625);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "RFIDMainForm";
            this.Text = "RFIDMainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RFIDMainForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RFIDMainForm_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.TextBox txtM1Key;
        private System.Windows.Forms.Label lblM1Key;
        private System.Windows.Forms.ComboBox cmbM1Sector;
        private System.Windows.Forms.Label lblM1Sector;
        private System.Windows.Forms.ComboBox cmbM1Block;
        private System.Windows.Forms.Label lblM1Block;
        private System.Windows.Forms.TextBox txtM1Data;
        private System.Windows.Forms.Label lblM1TagType;
        private System.Windows.Forms.ComboBox cmbM1KeyType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbM1TagType;
        private System.Windows.Forms.Button btn15693Write;
        private System.Windows.Forms.Button btn15693Read;
        private System.Windows.Forms.ComboBox cmb15693Block;
        private System.Windows.Forms.Label lbl15693Block;
        private System.Windows.Forms.TextBox txt15693Data;
        private System.Windows.Forms.TextBox txtAFI;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAFILock;
        private System.Windows.Forms.TextBox txtDsfid;
        private System.Windows.Forms.Button btnDsfidWrite;
        private System.Windows.Forms.Button btnAFIWrite;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnDsfidLock;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt14443ACPUData;
        private System.Windows.Forms.Button btn14443ACPUInit;
        private System.Windows.Forms.Label lblMSG;
        private System.Windows.Forms.TextBox txtCPUReceive;
        private System.Windows.Forms.Button btnGetUID;
        private System.Windows.Forms.TextBox txt14443BReceive;
        private System.Windows.Forms.RadioButton rbCard2;
        private System.Windows.Forms.RadioButton rbCard1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtPsamData;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnPsamSend;
        private System.Windows.Forms.Button btnFree;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.TextBox txtPsamReceive;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btn14443B;
        private System.Windows.Forms.TextBox txt14443BSendData;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox txtUID;
    }
}