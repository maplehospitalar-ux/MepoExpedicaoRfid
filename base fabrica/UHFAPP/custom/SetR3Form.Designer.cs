namespace UHFAPP.custom
{
    partial class SetR3Form
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
            this.lvParm = new System.Windows.Forms.ListView();
            this.cId = new System.Windows.Forms.ColumnHeader();
            this.cName = new System.Windows.Forms.ColumnHeader();
            this.cValue = new System.Windows.Forms.ColumnHeader();
            this.specification = new System.Windows.Forms.ColumnHeader();
            this.btnSet = new System.Windows.Forms.Button();
            this.SuspendLayout();
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
            this.lvParm.Location = new System.Drawing.Point(12, 12);
            this.lvParm.Name = "lvParm";
            this.lvParm.Size = new System.Drawing.Size(1245, 536);
            this.lvParm.TabIndex = 4;
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
            // btnSet
            // 
            this.btnSet.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSet.Location = new System.Drawing.Point(566, 566);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(135, 53);
            this.btnSet.TabIndex = 5;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // SetR3Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1269, 631);
            this.Controls.Add(this.btnSet);
            this.Controls.Add(this.lvParm);
            this.Name = "SetR3Form";
            this.Text = "R5Form";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvParm;
        private System.Windows.Forms.ColumnHeader cName;
        private System.Windows.Forms.ColumnHeader cValue;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.ColumnHeader cId;
        private System.Windows.Forms.ColumnHeader specification;
    }
}