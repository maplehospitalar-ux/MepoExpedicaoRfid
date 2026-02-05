using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;

namespace UHFAPP.custom
{
    public partial class SetR3Form : Form
    {
        List<ParameterInfo> list = new List<ParameterInfo>();
        List<ParameterInfo> listSpecification = new List<ParameterInfo>();
        public SetR3Form()
        {
            InitializeComponent();

          // string strData = "aa=1";
          //  byte[] data = System.Text.Encoding.ASCII.GetBytes(strData);
           // int result = UHFAPI.UHFUploadUserParam(data, (short)(data.Length));

            ReadListSpecificationData();
            ReadFlashData();
            LoadData();
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
                ParameterInfo info=list[k];

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
             byte[] param=new byte[512];
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
                int index= this.lvParm.SelectedItems[0].Index;
                ParameterInfo info = list[index];

                this.Hide();
                UHFAPP.custom.R5ModifyParametersForm f = new UHFAPP.custom.R5ModifyParametersForm(info.Name, info.Value, info.Specification);
                f.StartPosition = FormStartPosition.CenterScreen;
                DialogResult rsult=f.ShowDialog();
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
            if (listSpecification != null && listSpecification.Count > 0) {
                for (int k = 0; k < listSpecification.Count; k++) {
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



   


  //private void WriteData(string key,string value)
  //      {
  //          StringBuilder sb = new StringBuilder();
  //          string path = System.Environment.CurrentDirectory + "\\DEVICE.CFG";
  //          StreamReader sr = new StreamReader(path, Encoding.Default);
  //          string temp = null;
  //          while ((temp = sr.ReadLine()) != null)
  //          {
  //              if (temp.Trim().StartsWith(key))
  //              {
  //                  sb.Append(key + "=" + value);

  //              }
  //              else
  //              {
  //                  sb.Append(temp);
  //              }
  //              sb.Append("\r\n");
  //          }
  //          sr.Close();

  //          StreamWriter sw = new StreamWriter(path, false);
  //          sw.Write(sb.ToString());
  //          sw.Close();
  //      }