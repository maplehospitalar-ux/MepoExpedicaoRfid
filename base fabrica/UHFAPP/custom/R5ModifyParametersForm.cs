using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UHFAPP.custom
{
    public partial class R5ModifyParametersForm : Form
    {
        public R5ModifyParametersForm()
        {
            InitializeComponent();
        }
        public R5ModifyParametersForm(string name, string value, string specification)
        {
            InitializeComponent();

            txtName.Text = name;
            txtValue.Text = value;
            textBox3.Text = specification;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            name = txtName.Text;
            value = txtValue.Text;
            specification= textBox3.Text;
            this.Close();

        }

        public string name;
        public string value;
        public string specification;
    }
}
