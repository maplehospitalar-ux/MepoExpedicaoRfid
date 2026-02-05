using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using UHFAPP.custom;

namespace UHFAPP.r1
{
    public partial class R1DevcieConfig : BaseForm
    {

        List<ParameterInfo> list = new List<ParameterInfo>();
        List<ParameterInfo> listSpecification = new List<ParameterInfo>();
        public R1DevcieConfig(bool isOpen)
        {
            InitializeComponent();
            if (isOpen)
            {
                panel1.Enabled = true;
            }
            else
            {
                panel1.Enabled = false;
            }
        }

        void MainForm_eventOpen(bool open)
        {
            if (open)
            {
                panel1.Enabled = true;
            }
            else
            {
                panel1.Enabled = false;
            }
        }

        private void R1DevcieConfig_Load(object sender, EventArgs e)
        {
            R1MainForm.eventOpen += MainForm_eventOpen;
            R1MainForm.eventSwitchUI += MainForm_eventSwitchUI;
            MainForm_eventSwitchUI();

            ReadListSpecificationData();
            ReadFlashData();
            LoadData();
        }
        private void MainForm_eventSwitchUI()
        {
            if (Common.isEnglish)
            {
          
            }
            else
            {
             
            }
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            if (list != null && list.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int k = 0; k < list.Count; k++)
                {
                    ParameterInfo info = list[k];
                    sb.Append(info.Name);
                    sb.Append("=");
                    sb.Append(info.Value);
                    sb.Append(";");
                }
                byte[] data = System.Text.Encoding.ASCII.GetBytes(sb.ToString());
                int result = UHFAPI.UHFUploadUserParam(data, (short)(data.Length));
                if (result == 0)
                {
                    MessageBox.Show("success");
                    return;
                }
            }

            MessageBox.Show("fail");
        }

        private void LoadData()
        {
            int index = 0;
            lvParm.Items.Clear();
            for (int k = 0; k < list.Count; k++)
            {
                ParameterInfo info = list[k];

                ListViewItem lv = new ListViewItem();
                lv.Text = (index + 1).ToString();

                ListViewItem.ListViewSubItem itemName = new ListViewItem.ListViewSubItem();
                itemName.Name = "cName";
                itemName.Text = info.Name;
                lv.SubItems.Add(itemName);

                ListViewItem.ListViewSubItem itemValue = new ListViewItem.ListViewSubItem();
                itemValue.Name = "cValue";
                itemValue.Text = info.Value;
                lv.SubItems.Add(itemValue);

                ListViewItem.ListViewSubItem itemSpecification = new ListViewItem.ListViewSubItem();
                itemSpecification.Name = "cSpecification";
                itemSpecification.Text = info.Specification;
                lv.SubItems.Add(itemSpecification);

                lvParm.Items.Insert(index, lv);
                index = index + 1;
            }


        }



        private void ReadFlashData()
        {
            list.Clear();
            byte[] param = new byte[512];
            short[] paramLen = new short[10];
            UHFAPI.UHFDownloadUserParam(param, paramLen);

            if (param != null && param.Length > 0 && paramLen[0] > 0)
            {
                string strParam = System.Text.Encoding.ASCII.GetString(param, 0, paramLen[0]);
                string[] data = strParam.Split(';');
                for (int k = 0; k < data.Length; k++)
                {
                    if (data[k] != null && data[k].Contains('='))
                    {
                        string[] temp = data[k].Split('=');
                        ParameterInfo info = new ParameterInfo();
                        info.Name = temp[0].Trim();
                        info.Value = temp[1].Trim();
                        info.Specification = GetSpecification(info.Name);
                        list.Add(info);
                    }
                }

            }

        }
        private void ModificationData(string key, string value)
        {
            //if (value != null && value.Length > 0)
            {
                for (int k = 0; k < list.Count; k++)
                {
                    ParameterInfo info = list[k];
                    if (info.Name == key)
                    {
                        info.Value = value.Trim();
                    }
                }
            }
        }


        //编辑参数
        private void lvParm_DoubleClick(object sender, EventArgs e)
        {
            if (lvParm.SelectedItems.Count >= 0)
            {
                int index = this.lvParm.SelectedItems[0].Index;
                ParameterInfo info = list[index];

                this.Hide();
                ParametersForm f = new ParametersForm(info.Name, info.Value, info.Specification);
                f.StartPosition = FormStartPosition.CenterScreen;
                
               
                DialogResult rsult = f.ShowDialog();
                this.Show();
                if (rsult == DialogResult.OK)
                {
                    ModificationData(f.name, f.value);
                    LoadData();
                }
              
                return;
            }
        }

        class ParameterInfo
        {
            string name;

            public string Name
            {
                get { return name; }
                set { name = value; }
            }
            string value;

            public string Value
            {
                get { return this.value; }
                set { this.value = value; }
            }
            string specification;

            public string Specification
            {
                get { return specification; }
                set { specification = value; }
            }


        }

        //读取注释配置文件
        private void ReadListSpecificationData()
        {
            listSpecification.Clear();
            string path = System.Environment.CurrentDirectory + "\\DEVICE.CFG";
            StreamReader sr = new StreamReader(path, Encoding.Default);
            string temp = null;

            while ((temp = sr.ReadLine()) != null)
            {
                if (temp.Contains("#"))
                {
                    ParameterInfo info = new ParameterInfo();
                    string[] str = temp.Split('#');
                    info.Name = str[0].Trim();
                    info.Specification = str[1].Trim();
                    listSpecification.Add(info);
                }
            }
            sr.Close();
        }
        //根据名称获取注释
        private string GetSpecification(string name)
        {
            if (listSpecification != null && listSpecification.Count > 0)
            {
                for (int k = 0; k < listSpecification.Count; k++)
                {
                    if (name == listSpecification[k].Name)
                    {
                        return listSpecification[k].Specification;
                    }
                }
            }
            return "";
        }
 
    }
}
