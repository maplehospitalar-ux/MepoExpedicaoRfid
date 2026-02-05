using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UHFAPP
{
    public partial class ChoiceNumberForm : Form
    {
        public int number = 0;
        public ChoiceNumberForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;

            if (Common.isEnglish) {

                button1.Text = "OK";
                label1.Text = "Number:";
            
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            number = int.Parse(comboBox1.Text); //comboBox1.SelectedIndex;
            this.Close();
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            ComboBox txt = (ComboBox)sender;
            if (!StringUtils.IsNumber(txt.Text))
            {
                txt.Text = "0";
            }
            else {
                if (int.Parse(txt.Text) > 15) {

                    txt.Text = "15";
                }
            
            }
            
        }
    }
}
