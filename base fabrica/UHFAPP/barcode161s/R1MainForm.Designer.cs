namespace UHFAPP
{
    partial class R1MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(R1MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uHFVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uHFUpgradeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetR3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.combCommunicationMode = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel7 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configToolStripMenuItem,
            this.uHFVersionToolStripMenuItem,
            this.toolStripMenuItem1,
            this.uHFUpgradeToolStripMenuItem,
            this.SetR3ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1092, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "ScanEPC";
            this.menuStrip1.ItemAdded += new System.Windows.Forms.ToolStripItemEventHandler(this.menuStrip1_ItemAdded);
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            this.configToolStripMenuItem.Size = new System.Drawing.Size(99, 21);
            this.configToolStripMenuItem.Text = "Configuration";
            this.configToolStripMenuItem.Click += new System.EventHandler(this.configToolStripMenuItem_Click);
            // 
            // uHFVersionToolStripMenuItem
            // 
            this.uHFVersionToolStripMenuItem.Name = "uHFVersionToolStripMenuItem";
            this.uHFVersionToolStripMenuItem.Size = new System.Drawing.Size(71, 21);
            this.uHFVersionToolStripMenuItem.Text = "UHF Info";
            this.uHFVersionToolStripMenuItem.Click += new System.EventHandler(this.uHFVersionToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(95, 21);
            this.toolStripMenuItem1.Text = "Temperature";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // uHFUpgradeToolStripMenuItem
            // 
            this.uHFUpgradeToolStripMenuItem.Enabled = false;
            this.uHFUpgradeToolStripMenuItem.Name = "uHFUpgradeToolStripMenuItem";
            this.uHFUpgradeToolStripMenuItem.Size = new System.Drawing.Size(100, 21);
            this.uHFUpgradeToolStripMenuItem.Text = "UHF Upgrade";
            this.uHFUpgradeToolStripMenuItem.Click += new System.EventHandler(this.uHFUpgradeToolStripMenuItem_Click);
            // 
            // SetR3ToolStripMenuItem
            // 
            this.SetR3ToolStripMenuItem.Name = "SetR3ToolStripMenuItem";
            this.SetR3ToolStripMenuItem.Size = new System.Drawing.Size(91, 21);
            this.SetR3ToolStripMenuItem.Text = "User Setting";
            this.SetR3ToolStripMenuItem.Click += new System.EventHandler(this.SetR3ToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel4,
            this.combCommunicationMode,
            this.toolStripOpen,
            this.toolStripLabel2,
            this.toolStripLabel6,
            this.toolStripLabel7,
            this.toolStripLabel3,
            this.toolStripComboBox1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(1092, 34);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "Open";
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(56, 31);
            this.toolStripLabel4.Text = "通信方式";
            // 
            // combCommunicationMode
            // 
            this.combCommunicationMode.Items.AddRange(new object[] {
            "USB"});
            this.combCommunicationMode.Name = "combCommunicationMode";
            this.combCommunicationMode.Size = new System.Drawing.Size(121, 34);
            // 
            // toolStripOpen
            // 
            this.toolStripOpen.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.toolStripOpen.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolStripOpen.BackgroundImage")));
            this.toolStripOpen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolStripOpen.Checked = true;
            this.toolStripOpen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripOpen.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripOpen.Image = ((System.Drawing.Image)(resources.GetObject("toolStripOpen.Image")));
            this.toolStripOpen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripOpen.Name = "toolStripOpen";
            this.toolStripOpen.Size = new System.Drawing.Size(60, 31);
            this.toolStripOpen.Text = "  Open  ";
            this.toolStripOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.toolStripOpen.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(80, 31);
            this.toolStripLabel2.Text = "                  ";
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(0, 31);
            // 
            // toolStripLabel7
            // 
            this.toolStripLabel7.Name = "toolStripLabel7";
            this.toolStripLabel7.Size = new System.Drawing.Size(428, 31);
            this.toolStripLabel7.Text = "                                                                                 " +
                "                        ";
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(65, 31);
            this.toolStripLabel3.Text = "Language";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 34);
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
            // 
            // R1MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1092, 702);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "R1MainForm";
            this.Text = "UHF(1.3.4)";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripOpen;
        private System.Windows.Forms.ToolStripMenuItem uHFVersionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripComboBox combCommunicationMode;
        private System.Windows.Forms.ToolStripMenuItem uHFUpgradeToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripLabel toolStripLabel7;
        private System.Windows.Forms.ToolStripMenuItem SetR3ToolStripMenuItem;

    }
}

