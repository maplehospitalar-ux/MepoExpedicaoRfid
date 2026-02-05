namespace UHFAPP
{
    partial class UHFUpgradeFormEx
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnPath = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rbUHFModule = new System.Windows.Forms.RadioButton();
            this.rbReaderBootloader = new System.Windows.Forms.RadioButton();
            this.rbEx10SDKFirmware = new System.Windows.Forms.RadioButton();
            this.rbReaderApplication = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(37, 170);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(368, 254);
            this.textBox1.TabIndex = 12;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(38, 146);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(367, 18);
            this.progressBar1.TabIndex = 13;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(212, 102);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(105, 33);
            this.btnStart.TabIndex = 11;
            this.btnStart.Text = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnPath
            // 
            this.btnPath.Location = new System.Drawing.Point(323, 100);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(82, 35);
            this.btnPath.TabIndex = 10;
            this.btnPath.Text = "Select file";
            this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(41, 9);
            this.txtPath.Multiline = true;
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(364, 52);
            this.txtPath.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(-1, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.TabIndex = 14;
            this.label1.Text = "path:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(54, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(354, 20);
            this.label2.TabIndex = 15;
            this.label2.Text = "version:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // rbUHFModule
            // 
            this.rbUHFModule.AutoSize = true;
            this.rbUHFModule.Location = new System.Drawing.Point(41, 82);
            this.rbUHFModule.Name = "rbUHFModule";
            this.rbUHFModule.Size = new System.Drawing.Size(83, 16);
            this.rbUHFModule.TabIndex = 16;
            this.rbUHFModule.TabStop = true;
            this.rbUHFModule.Text = "UHF module";
            this.rbUHFModule.UseVisualStyleBackColor = true;
            // 
            // rbReaderBootloader
            // 
            this.rbReaderBootloader.AutoSize = true;
            this.rbReaderBootloader.Location = new System.Drawing.Point(41, 102);
            this.rbReaderBootloader.Name = "rbReaderBootloader";
            this.rbReaderBootloader.Size = new System.Drawing.Size(131, 16);
            this.rbReaderBootloader.TabIndex = 17;
            this.rbReaderBootloader.TabStop = true;
            this.rbReaderBootloader.Text = "Reader bootloader ";
            this.rbReaderBootloader.UseVisualStyleBackColor = true;
            // 
            // rbEx10SDKFirmware
            // 
            this.rbEx10SDKFirmware.AutoSize = true;
            this.rbEx10SDKFirmware.Location = new System.Drawing.Point(41, 124);
            this.rbEx10SDKFirmware.Name = "rbEx10SDKFirmware";
            this.rbEx10SDKFirmware.Size = new System.Drawing.Size(125, 16);
            this.rbEx10SDKFirmware.TabIndex = 18;
            this.rbEx10SDKFirmware.TabStop = true;
            this.rbEx10SDKFirmware.Text = "Ex10 SDK firmware";
            this.rbEx10SDKFirmware.UseVisualStyleBackColor = true;
            // 
            // rbReaderApplication
            // 
            this.rbReaderApplication.AutoSize = true;
            this.rbReaderApplication.Location = new System.Drawing.Point(41, 63);
            this.rbReaderApplication.Name = "rbReaderApplication";
            this.rbReaderApplication.Size = new System.Drawing.Size(131, 16);
            this.rbReaderApplication.TabIndex = 19;
            this.rbReaderApplication.TabStop = true;
            this.rbReaderApplication.Text = "Reader application";
            this.rbReaderApplication.UseVisualStyleBackColor = true;
            // 
            // UHFUpgradeFormEx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(434, 431);
            this.Controls.Add(this.rbReaderApplication);
            this.Controls.Add(this.rbEx10SDKFirmware);
            this.Controls.Add(this.rbReaderBootloader);
            this.Controls.Add(this.rbUHFModule);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnPath);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.MaximizeBox = false;
            this.Name = "UHFUpgradeFormEx";
            this.Text = "UHFUpgradeForm";
            this.Load += new System.EventHandler(this.UHFUpgradeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rbUHFModule;
        private System.Windows.Forms.RadioButton rbReaderBootloader;
        private System.Windows.Forms.RadioButton rbEx10SDKFirmware;
        private System.Windows.Forms.RadioButton rbReaderApplication;
    }
}