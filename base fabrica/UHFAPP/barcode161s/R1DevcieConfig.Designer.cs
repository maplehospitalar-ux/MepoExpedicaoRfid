namespace UHFAPP.r1
{
    partial class R1DevcieConfig
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSet = new System.Windows.Forms.Button();
            this.lvParm = new System.Windows.Forms.ListView();
            this.cId = new System.Windows.Forms.ColumnHeader();
            this.cName = new System.Windows.Forms.ColumnHeader();
            this.cValue = new System.Windows.Forms.ColumnHeader();
            this.specification = new System.Windows.Forms.ColumnHeader();
            this.label8 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel1.Controls.Add(this.btnSet);
            this.panel1.Controls.Add(this.lvParm);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1252, 641);
            this.panel1.TabIndex = 33;
            // 
            // btnSet
            // 
            this.btnSet.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSet.Location = new System.Drawing.Point(498, 565);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(135, 53);
            this.btnSet.TabIndex = 62;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // lvParm
            // 
            this.lvParm.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lvParm.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cId,
            this.cName,
            this.cValue,
            this.specification});
            this.lvParm.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvParm.FullRowSelect = true;
            this.lvParm.Location = new System.Drawing.Point(3, 12);
            this.lvParm.Name = "lvParm";
            this.lvParm.Size = new System.Drawing.Size(1245, 536);
            this.lvParm.TabIndex = 61;
            this.lvParm.UseCompatibleStateImageBehavior = false;
            this.lvParm.View = System.Windows.Forms.View.Details;
            this.lvParm.DoubleClick += new System.EventHandler(this.lvParm_DoubleClick);
            // 
            // cId
            // 
            this.cId.Text = "Id";
            this.cId.Width = 50;
            // 
            // cName
            // 
            this.cName.Text = "Name";
            this.cName.Width = 150;
            // 
            // cValue
            // 
            this.cValue.Text = "Value";
            this.cValue.Width = 100;
            // 
            // specification
            // 
            this.specification.Text = "Specification";
            this.specification.Width = 1300;
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
            // R1DevcieConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1252, 641);
            this.Controls.Add(this.panel1);
            this.Name = "R1DevcieConfig";
            this.Text = "R1DevcieConfig";
            this.Load += new System.EventHandler(this.R1DevcieConfig_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ListView lvParm;
        private System.Windows.Forms.ColumnHeader cId;
        private System.Windows.Forms.ColumnHeader cName;
        private System.Windows.Forms.ColumnHeader cValue;
        private System.Windows.Forms.ColumnHeader specification;
        private System.Windows.Forms.Button btnSet;
    }
}