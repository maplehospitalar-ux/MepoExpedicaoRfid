using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UHFAPP.multidevice
{
    public partial class DeviceListForm : Form
    {
        public DeviceListForm(List<DeviceInfo> list)
        {
            InitializeComponent();
            for (int k = 0; k < list.Count; k++)
            {
                comboBox1.Items.Add(list[k].Ip);
            }
            
        }


        public string ip;

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ip = comboBox1.Items[comboBox1.SelectedIndex].ToString();
            this.Close();
        }
    }
}
