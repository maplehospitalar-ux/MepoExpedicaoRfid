namespace UHFAPP.custom.m775Authenticate
{
    partial class M775AuthenticateForm
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtResponse = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDataLen = new System.Windows.Forms.Label();
            this.txtTid = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAuthenticateData = new System.Windows.Forms.TextBox();
            this.btnAuthenticate = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtChallenge = new System.Windows.Forms.TextBox();
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
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.groupBox2.Controls.Add(this.txtResponse);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtDataLen);
            this.groupBox2.Controls.Add(this.txtTid);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtAuthenticateData);
            this.groupBox2.Controls.Add(this.btnAuthenticate);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtChallenge);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(12, 175);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(531, 239);
            this.groupBox2.TabIndex = 54;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "M775 Authenticate";
            // 
            // txtResponse
            // 
            this.txtResponse.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtResponse.Location = new System.Drawing.Point(137, 129);
            this.txtResponse.Name = "txtResponse";
            this.txtResponse.ReadOnly = true;
            this.txtResponse.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResponse.Size = new System.Drawing.Size(367, 21);
            this.txtResponse.TabIndex = 54;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(12, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 53;
            this.label2.Text = "Tag Response:";
            // 
            // txtDataLen
            // 
            this.txtDataLen.AutoSize = true;
            this.txtDataLen.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtDataLen.Location = new System.Drawing.Point(510, 45);
            this.txtDataLen.Name = "txtDataLen";
            this.txtDataLen.Size = new System.Drawing.Size(11, 12);
            this.txtDataLen.TabIndex = 52;
            this.txtDataLen.Text = "0";
            this.txtDataLen.Visible = false;
            // 
            // txtTid
            // 
            this.txtTid.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtTid.Location = new System.Drawing.Point(137, 98);
            this.txtTid.Name = "txtTid";
            this.txtTid.ReadOnly = true;
            this.txtTid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTid.Size = new System.Drawing.Size(367, 21);
            this.txtTid.TabIndex = 46;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(12, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 48;
            this.label1.Text = "Challenge:";
            // 
            // txtAuthenticateData
            // 
            this.txtAuthenticateData.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtAuthenticateData.Location = new System.Drawing.Point(139, 39);
            this.txtAuthenticateData.MaxLength = 30;
            this.txtAuthenticateData.Name = "txtAuthenticateData";
            this.txtAuthenticateData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAuthenticateData.Size = new System.Drawing.Size(367, 21);
            this.txtAuthenticateData.TabIndex = 51;
            this.txtAuthenticateData.Visible = false;
            this.txtAuthenticateData.TextChanged += new System.EventHandler(this.txtAuthenticateData_TextChanged);
            // 
            // btnAuthenticate
            // 
            this.btnAuthenticate.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAuthenticate.Location = new System.Drawing.Point(157, 168);
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
            this.label3.Location = new System.Drawing.Point(12, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 12);
            this.label3.TabIndex = 45;
            this.label3.Text = "Tags Shortened TID:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 9F);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(12, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 50;
            this.label5.Text = "Message:";
            this.label5.Visible = false;
            // 
            // txtChallenge
            // 
            this.txtChallenge.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtChallenge.Location = new System.Drawing.Point(137, 67);
            this.txtChallenge.Name = "txtChallenge";
            this.txtChallenge.ReadOnly = true;
            this.txtChallenge.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtChallenge.Size = new System.Drawing.Size(367, 21);
            this.txtChallenge.TabIndex = 49;
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
            this.groupBox4.Location = new System.Drawing.Point(12, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(531, 147);
            this.groupBox4.TabIndex = 53;
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
            // M775AuthenticateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(559, 414);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.KeyPreview = true;
            this.Name = "M775AuthenticateForm";
            this.Text = "M775AuthenticateForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.M775AuthenticateForm_KeyDown);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label txtDataLen;
        private System.Windows.Forms.TextBox txtTid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAuthenticateData;
        private System.Windows.Forms.Button btnAuthenticate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtChallenge;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label txtFilter_EPCLen;
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
        private System.Windows.Forms.TextBox txtResponse;
        private System.Windows.Forms.Label label2;
    }
}