namespace UHFAPP.custom.authenticate
{
    partial class AuthenticateForm
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtFilter_EPCLen = new System.Windows.Forms.Label();
            this.txtLen = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.txtPtr = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbUser = new System.Windows.Forms.RadioButton();
            this.rbEPC = new System.Windows.Forms.RadioButton();
            this.rbTID = new System.Windows.Forms.RadioButton();
            this.label22 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.txtFilter_EPC = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtEncryptionData2Len = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnActivate = new System.Windows.Forms.Button();
            this.txtKey0DataLen = new System.Windows.Forms.Label();
            this.btnRead = new System.Windows.Forms.Button();
            this.txtKey0Data = new System.Windows.Forms.TextBox();
            this.lblKey0 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtDataLen = new System.Windows.Forms.Label();
            this.txtAuthenticateEncryptionData = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAuthenticateData = new System.Windows.Forms.TextBox();
            this.btnAuthenticate = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtAuthenticateKeyID = new System.Windows.Forms.TextBox();
            this.txtDecodeCiphertext = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblencryptoKeyLen = new System.Windows.Forms.Label();
            this.txtC = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtRnd = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnDecode = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.decodeKey = new System.Windows.Forms.TextBox();
            this.txtData2Len = new System.Windows.Forms.Label();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.groupBox4.Controls.Add(this.txtFilter_EPCLen);
            this.groupBox4.Controls.Add(this.txtLen);
            this.groupBox4.Controls.Add(this.label24);
            this.groupBox4.Controls.Add(this.label23);
            this.groupBox4.Controls.Add(this.txtPtr);
            this.groupBox4.Controls.Add(this.groupBox3);
            this.groupBox4.Controls.Add(this.label22);
            this.groupBox4.Controls.Add(this.label30);
            this.groupBox4.Controls.Add(this.txtFilter_EPC);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox4.ForeColor = System.Drawing.Color.Black;
            this.groupBox4.Location = new System.Drawing.Point(13, 20);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(531, 147);
            this.groupBox4.TabIndex = 29;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Filter";
            // 
            // txtFilter_EPCLen
            // 
            this.txtFilter_EPCLen.AutoSize = true;
            this.txtFilter_EPCLen.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtFilter_EPCLen.Location = new System.Drawing.Point(487, 22);
            this.txtFilter_EPCLen.Name = "txtFilter_EPCLen";
            this.txtFilter_EPCLen.Size = new System.Drawing.Size(0, 12);
            this.txtFilter_EPCLen.TabIndex = 39;
            // 
            // txtLen
            // 
            this.txtLen.Font = new System.Drawing.Font("宋体", 9F);
            this.txtLen.Location = new System.Drawing.Point(264, 113);
            this.txtLen.MaxLength = 3;
            this.txtLen.Name = "txtLen";
            this.txtLen.Size = new System.Drawing.Size(82, 21);
            this.txtLen.TabIndex = 37;
            this.txtLen.Tag = "Number";
            this.txtLen.Text = "0";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("宋体", 9F);
            this.label24.Location = new System.Drawing.Point(350, 118);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(35, 12);
            this.label24.TabIndex = 38;
            this.label24.Text = "(bit)";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("宋体", 9F);
            this.label23.Location = new System.Drawing.Point(218, 118);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(47, 12);
            this.label23.TabIndex = 36;
            this.label23.Text = "Length:";
            // 
            // txtPtr
            // 
            this.txtPtr.Font = new System.Drawing.Font("宋体", 9F);
            this.txtPtr.Location = new System.Drawing.Point(73, 113);
            this.txtPtr.MaxLength = 3;
            this.txtPtr.Name = "txtPtr";
            this.txtPtr.Size = new System.Drawing.Size(82, 21);
            this.txtPtr.TabIndex = 33;
            this.txtPtr.Tag = "Number";
            this.txtPtr.Text = "32";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbUser);
            this.groupBox3.Controls.Add(this.rbEPC);
            this.groupBox3.Controls.Add(this.rbTID);
            this.groupBox3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(72, 58);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(296, 47);
            this.groupBox3.TabIndex = 34;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "bank";
            // 
            // rbUser
            // 
            this.rbUser.AutoSize = true;
            this.rbUser.Font = new System.Drawing.Font("宋体", 9F);
            this.rbUser.Location = new System.Drawing.Point(114, 20);
            this.rbUser.Name = "rbUser";
            this.rbUser.Size = new System.Drawing.Size(47, 16);
            this.rbUser.TabIndex = 12;
            this.rbUser.Text = "User";
            this.rbUser.UseVisualStyleBackColor = true;
            this.rbUser.Click += new System.EventHandler(this.rbUser_Click);
            // 
            // rbEPC
            // 
            this.rbEPC.AutoSize = true;
            this.rbEPC.Checked = true;
            this.rbEPC.Font = new System.Drawing.Font("宋体", 9F);
            this.rbEPC.Location = new System.Drawing.Point(11, 19);
            this.rbEPC.Name = "rbEPC";
            this.rbEPC.Size = new System.Drawing.Size(41, 16);
            this.rbEPC.TabIndex = 8;
            this.rbEPC.TabStop = true;
            this.rbEPC.Text = "EPC";
            this.rbEPC.UseVisualStyleBackColor = true;
            this.rbEPC.Click += new System.EventHandler(this.rbEPC_Click);
            // 
            // rbTID
            // 
            this.rbTID.AutoSize = true;
            this.rbTID.Font = new System.Drawing.Font("宋体", 9F);
            this.rbTID.Location = new System.Drawing.Point(67, 20);
            this.rbTID.Name = "rbTID";
            this.rbTID.Size = new System.Drawing.Size(41, 16);
            this.rbTID.TabIndex = 9;
            this.rbTID.Text = "TID";
            this.rbTID.UseVisualStyleBackColor = true;
            this.rbTID.Click += new System.EventHandler(this.rbTID_Click);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("宋体", 9F);
            this.label22.Location = new System.Drawing.Point(155, 119);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(35, 12);
            this.label22.TabIndex = 35;
            this.label22.Text = "(bit)";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("宋体", 9F);
            this.label30.Location = new System.Drawing.Point(37, 117);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(29, 12);
            this.label30.TabIndex = 32;
            this.label30.Text = "Ptr:";
            // 
            // txtFilter_EPC
            // 
            this.txtFilter_EPC.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtFilter_EPC.Location = new System.Drawing.Point(73, 14);
            this.txtFilter_EPC.Multiline = true;
            this.txtFilter_EPC.Name = "txtFilter_EPC";
            this.txtFilter_EPC.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFilter_EPC.Size = new System.Drawing.Size(415, 38);
            this.txtFilter_EPC.TabIndex = 12;
            this.txtFilter_EPC.TextChanged += new System.EventHandler(this.txtFilter_EPC_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 9F);
            this.label12.ForeColor = System.Drawing.Color.Black;
            this.label12.Location = new System.Drawing.Point(37, 26);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 12);
            this.label12.TabIndex = 11;
            this.label12.Text = "Data:";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.groupBox1.Controls.Add(this.txtEncryptionData2Len);
            this.groupBox1.Controls.Add(this.groupBox6);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(559, 501);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            // 
            // txtEncryptionData2Len
            // 
            this.txtEncryptionData2Len.AutoSize = true;
            this.txtEncryptionData2Len.Location = new System.Drawing.Point(513, 298);
            this.txtEncryptionData2Len.Name = "txtEncryptionData2Len";
            this.txtEncryptionData2Len.Size = new System.Drawing.Size(0, 12);
            this.txtEncryptionData2Len.TabIndex = 53;
            // 
            // groupBox6
            // 
            this.groupBox6.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.groupBox6.Controls.Add(this.btnWrite);
            this.groupBox6.Controls.Add(this.btnActivate);
            this.groupBox6.Controls.Add(this.txtKey0DataLen);
            this.groupBox6.Controls.Add(this.btnRead);
            this.groupBox6.Controls.Add(this.txtKey0Data);
            this.groupBox6.Controls.Add(this.lblKey0);
            this.groupBox6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox6.Location = new System.Drawing.Point(13, 390);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(531, 95);
            this.groupBox6.TabIndex = 53;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Read/Write/Activate";
            // 
            // btnWrite
            // 
            this.btnWrite.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnWrite.Location = new System.Drawing.Point(179, 47);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(86, 29);
            this.btnWrite.TabIndex = 54;
            this.btnWrite.Text = "Write";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // btnActivate
            // 
            this.btnActivate.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnActivate.Location = new System.Drawing.Point(310, 47);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(86, 29);
            this.btnActivate.TabIndex = 48;
            this.btnActivate.Text = "Activate";
            this.btnActivate.UseVisualStyleBackColor = true;
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            // 
            // txtKey0DataLen
            // 
            this.txtKey0DataLen.AutoSize = true;
            this.txtKey0DataLen.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtKey0DataLen.Location = new System.Drawing.Point(498, 29);
            this.txtKey0DataLen.Name = "txtKey0DataLen";
            this.txtKey0DataLen.Size = new System.Drawing.Size(0, 12);
            this.txtKey0DataLen.TabIndex = 53;
            // 
            // btnRead
            // 
            this.btnRead.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRead.Location = new System.Drawing.Point(60, 47);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(86, 29);
            this.btnRead.TabIndex = 53;
            this.btnRead.Text = "Read";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // txtKey0Data
            // 
            this.txtKey0Data.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtKey0Data.Location = new System.Drawing.Point(46, 20);
            this.txtKey0Data.MaxLength = 48;
            this.txtKey0Data.Name = "txtKey0Data";
            this.txtKey0Data.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtKey0Data.Size = new System.Drawing.Size(448, 21);
            this.txtKey0Data.TabIndex = 52;
            this.txtKey0Data.TextChanged += new System.EventHandler(this.txtKey0Data_TextChanged);
            // 
            // lblKey0
            // 
            this.lblKey0.AutoSize = true;
            this.lblKey0.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblKey0.Location = new System.Drawing.Point(11, 27);
            this.lblKey0.Name = "lblKey0";
            this.lblKey0.Size = new System.Drawing.Size(29, 12);
            this.lblKey0.TabIndex = 0;
            this.lblKey0.Text = "Key0";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.groupBox2.Controls.Add(this.txtDataLen);
            this.groupBox2.Controls.Add(this.txtAuthenticateEncryptionData);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtAuthenticateData);
            this.groupBox2.Controls.Add(this.btnAuthenticate);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtAuthenticateKeyID);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(13, 183);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(531, 201);
            this.groupBox2.TabIndex = 52;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "AES Authenticate";
            // 
            // txtDataLen
            // 
            this.txtDataLen.AutoSize = true;
            this.txtDataLen.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtDataLen.Location = new System.Drawing.Point(498, 78);
            this.txtDataLen.Name = "txtDataLen";
            this.txtDataLen.Size = new System.Drawing.Size(0, 12);
            this.txtDataLen.TabIndex = 52;
            // 
            // txtAuthenticateEncryptionData
            // 
            this.txtAuthenticateEncryptionData.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtAuthenticateEncryptionData.Location = new System.Drawing.Point(102, 110);
            this.txtAuthenticateEncryptionData.Name = "txtAuthenticateEncryptionData";
            this.txtAuthenticateEncryptionData.ReadOnly = true;
            this.txtAuthenticateEncryptionData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAuthenticateEncryptionData.Size = new System.Drawing.Size(392, 21);
            this.txtAuthenticateEncryptionData.TabIndex = 46;
            this.txtAuthenticateEncryptionData.TextChanged += new System.EventHandler(this.txtEncryptionData2_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(2, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 48;
            this.label1.Text = "Key ID:";
            // 
            // txtAuthenticateData
            // 
            this.txtAuthenticateData.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtAuthenticateData.Location = new System.Drawing.Point(102, 74);
            this.txtAuthenticateData.MaxLength = 30;
            this.txtAuthenticateData.Name = "txtAuthenticateData";
            this.txtAuthenticateData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAuthenticateData.Size = new System.Drawing.Size(392, 21);
            this.txtAuthenticateData.TabIndex = 51;
            this.txtAuthenticateData.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            // 
            // btnAuthenticate
            // 
            this.btnAuthenticate.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAuthenticate.Location = new System.Drawing.Point(199, 152);
            this.btnAuthenticate.Name = "btnAuthenticate";
            this.btnAuthenticate.Size = new System.Drawing.Size(147, 36);
            this.btnAuthenticate.TabIndex = 47;
            this.btnAuthenticate.Text = "Authenticate";
            this.btnAuthenticate.UseVisualStyleBackColor = true;
            this.btnAuthenticate.Click += new System.EventHandler(this.btnAuthenticate_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(2, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 45;
            this.label3.Text = "Enciphered data:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 9F);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(3, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 50;
            this.label5.Text = "Message:";
            // 
            // txtAuthenticateKeyID
            // 
            this.txtAuthenticateKeyID.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtAuthenticateKeyID.Location = new System.Drawing.Point(102, 41);
            this.txtAuthenticateKeyID.Name = "txtAuthenticateKeyID";
            this.txtAuthenticateKeyID.ReadOnly = true;
            this.txtAuthenticateKeyID.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAuthenticateKeyID.Size = new System.Drawing.Size(392, 21);
            this.txtAuthenticateKeyID.TabIndex = 49;
            this.txtAuthenticateKeyID.Text = "0";
            // 
            // txtDecodeCiphertext
            // 
            this.txtDecodeCiphertext.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtDecodeCiphertext.Location = new System.Drawing.Point(68, 61);
            this.txtDecodeCiphertext.Name = "txtDecodeCiphertext";
            this.txtDecodeCiphertext.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDecodeCiphertext.Size = new System.Drawing.Size(302, 21);
            this.txtDecodeCiphertext.TabIndex = 44;
            this.txtDecodeCiphertext.TextChanged += new System.EventHandler(this.txtData2_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(6, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 43;
            this.label2.Text = "Data：";
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.groupBox5.Controls.Add(this.lblencryptoKeyLen);
            this.groupBox5.Controls.Add(this.txtC);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.txtRnd);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.txtMsg);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.btnDecode);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.decodeKey);
            this.groupBox5.Controls.Add(this.txtDecodeCiphertext);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.txtData2Len);
            this.groupBox5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox5.Location = new System.Drawing.Point(577, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(405, 278);
            this.groupBox5.TabIndex = 31;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "AES Decode";
            // 
            // lblencryptoKeyLen
            // 
            this.lblencryptoKeyLen.AutoSize = true;
            this.lblencryptoKeyLen.Font = new System.Drawing.Font("宋体", 9F);
            this.lblencryptoKeyLen.ForeColor = System.Drawing.Color.Black;
            this.lblencryptoKeyLen.Location = new System.Drawing.Point(376, 34);
            this.lblencryptoKeyLen.Name = "lblencryptoKeyLen";
            this.lblencryptoKeyLen.Size = new System.Drawing.Size(0, 12);
            this.lblencryptoKeyLen.TabIndex = 60;
            // 
            // txtC
            // 
            this.txtC.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtC.Location = new System.Drawing.Point(68, 93);
            this.txtC.Name = "txtC";
            this.txtC.ReadOnly = true;
            this.txtC.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtC.Size = new System.Drawing.Size(302, 21);
            this.txtC.TabIndex = 59;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 9F);
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(8, 98);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 12);
            this.label9.TabIndex = 58;
            this.label9.Text = "C_TAM1:";
            // 
            // txtRnd
            // 
            this.txtRnd.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtRnd.Location = new System.Drawing.Point(68, 128);
            this.txtRnd.Name = "txtRnd";
            this.txtRnd.ReadOnly = true;
            this.txtRnd.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRnd.Size = new System.Drawing.Size(302, 21);
            this.txtRnd.TabIndex = 57;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 9F);
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(6, 133);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 56;
            this.label8.Text = "TRnd_TAM1:";
            // 
            // txtMsg
            // 
            this.txtMsg.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtMsg.Location = new System.Drawing.Point(68, 160);
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.ReadOnly = true;
            this.txtMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMsg.Size = new System.Drawing.Size(302, 21);
            this.txtMsg.TabIndex = 55;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 9F);
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(8, 165);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 54;
            this.label7.Text = "Message:";
            // 
            // btnDecode
            // 
            this.btnDecode.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDecode.Location = new System.Drawing.Point(134, 209);
            this.btnDecode.Name = "btnDecode";
            this.btnDecode.Size = new System.Drawing.Size(133, 36);
            this.btnDecode.TabIndex = 52;
            this.btnDecode.Text = "decode";
            this.btnDecode.UseVisualStyleBackColor = true;
            this.btnDecode.Click += new System.EventHandler(this.btnDecode_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 9F);
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(18, 133);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 12);
            this.label6.TabIndex = 52;
            this.label6.Text = "TRnd:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 9F);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(5, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 50;
            this.label4.Text = "Key：";
            // 
            // decodeKey
            // 
            this.decodeKey.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.decodeKey.Location = new System.Drawing.Point(68, 31);
            this.decodeKey.MaxLength = 48;
            this.decodeKey.Name = "decodeKey";
            this.decodeKey.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.decodeKey.Size = new System.Drawing.Size(302, 21);
            this.decodeKey.TabIndex = 51;
            this.decodeKey.TextChanged += new System.EventHandler(this.decodeKey_TextChanged);
            // 
            // txtData2Len
            // 
            this.txtData2Len.AutoSize = true;
            this.txtData2Len.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtData2Len.Location = new System.Drawing.Point(369, 64);
            this.txtData2Len.Name = "txtData2Len";
            this.txtData2Len.Size = new System.Drawing.Size(0, 12);
            this.txtData2Len.TabIndex = 61;
            // 
            // AuthenticateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(996, 520);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.Name = "AuthenticateForm";
            this.Text = "AuthenticateForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AuthenticateForm_KeyDown);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtLen;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtPtr;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbUser;
        private System.Windows.Forms.RadioButton rbEPC;
        private System.Windows.Forms.RadioButton rbTID;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox txtFilter_EPC;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnAuthenticate;
        private System.Windows.Forms.TextBox txtAuthenticateEncryptionData;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDecodeCiphertext;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtAuthenticateData;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAuthenticateKeyID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox decodeKey;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnDecode;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnActivate;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.TextBox txtKey0Data;
        private System.Windows.Forms.Label lblKey0;
        private System.Windows.Forms.TextBox txtMsg;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtC;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtRnd;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblencryptoKeyLen;
        private System.Windows.Forms.Label txtData2Len;
        private System.Windows.Forms.Label txtFilter_EPCLen;
        private System.Windows.Forms.Label txtEncryptionData2Len;
        private System.Windows.Forms.Label txtDataLen;
        private System.Windows.Forms.Label txtKey0DataLen;
        private System.Windows.Forms.Button btnWrite;

    }
}