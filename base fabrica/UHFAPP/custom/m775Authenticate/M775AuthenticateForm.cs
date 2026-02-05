using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BLEDeviceAPI;

namespace UHFAPP.custom.m775Authenticate
{
    public partial class M775AuthenticateForm : Form
    {
        public M775AuthenticateForm()
        {
            InitializeComponent();
        }
        M775AuthenticateAPI api = new M775AuthenticateAPI();

        private void btnAuthenticate_Click(object sender, EventArgs e)
        {
            if (!DetectionFiltration())
            {
                return;
            }
             txtAuthenticateData.Text = "00000000000000000000";

            if (txtAuthenticateData.Text.Replace(" ", "").Length != 20)
            {
                MessageBox.Show("The length of the message must be 10 bytes!");
                return;
            }

            byte filterBank = 1;
            if (rbTID.Checked)
            {
                filterBank = 2;
            }
            else if (rbUser.Checked)
            {
                filterBank = 3;
            }
            int filterAddr = int.Parse(txtPtr.Text);
            int filterDataLen = int.Parse(txtLen.Text);
            byte[] filterData = DataConvert.HexStringToByteArray(txtFilter_EPC.Text);
            byte keyID = 0;
            byte[] tData = DataConvert.HexStringToByteArray(txtAuthenticateData.Text);
            int password = 0;

            byte[] result = api.UHFAuthenticate(password, filterBank, filterAddr, filterDataLen, filterData, keyID, tData);
            if (result == null || result.Length == 0)
            {
                MessageBox.Show("fail");
                txtTid.Text = "";
                return;
            }
            else if (result.Length!=22)
            {
                MessageBox.Show("返回数据长度错误");
                txtTid.Text = "";
                return;
            }
            //Challenge：6个字节，Tags Shortened TID：8个字节，Tag Response：8个字节

            txtChallenge.Text = DataConvert.ByteArrayToHexString(Utils.CopyArray(result, 0, 6));
            txtTid.Text = DataConvert.ByteArrayToHexString(Utils.CopyArray(result, 6, 8));
            txtResponse.Text = DataConvert.ByteArrayToHexString(Utils.CopyArray(result, 14, 8));
           // MessageBox.Show("success");
        }

        private void txtFilter_EPC_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txtFilter_EPC);
            string data = txtFilter_EPC.Text.Replace(" ", "");
            if (data.Length > 0)
            {
                txtFilter_EPCLen.Text = ((data.Length / 2) + ((data.Length % 2) == 0 ? 0 : 1)).ToString();  // txtRead_Length.Text = ((data.Length / 4) + ((data.Length % 4) == 0 ? 0 : 1)).ToString();
            }
            else
            {
                txtFilter_EPCLen.Text = "0";
            } 
        }
        private void FormatHex(TextBox txt)
        {
            if (isDelete) return;
            string data = txt.Text.Trim().Replace(" ", "");
            if (data != string.Empty)
            {
                int SelectIndex = txt.SelectionStart - 1;
                char[] charData = data.ToCharArray(0, data.Length);
                char[] newChar = new char[charData.Length];
                int index = 0;
                for (int k = 0; k < charData.Length; k++)
                {
                    if (StringUtils.IsHexNumber2(charData[k]))
                    {
                        newChar[index] = charData[k];
                        index++;
                    }
                }
                string newData = new string(newChar, 0, index);
                StringBuilder sb = new StringBuilder();
                int count = (newData.Length / 2) + (newData.Length % 2);

                for (int k = 0; k < count; k++)
                {
                    if ((k * 2 + 2) <= newData.Length)
                    {
                        sb.Append(newData.Substring(k * 2, 2));
                    }
                    else
                    {
                        sb.Append(newData.Substring(k * 2, 1));
                    }
                    sb.Append(" ");
                }
                txt.Text = sb.ToString();

                if (SelectIndex >= 0)
                    txt.SelectionStart = SelectIndex + 2;
                //txt.Select(txt.Text.Length - 1, 0);

            }
        }
        bool isDelete = false;

        private void M775AuthenticateForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                isDelete = true;
            }
            else
            {
                isDelete = false;
            }
        }
        private string HexFormat(string hex)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                for (int k = 0; k < hex.Length / 2; k++)
                {
                    sb.Append(hex.Substring(k * 2, 2));
                    if (k < hex.Length / 2 - 1)
                    {
                        sb.Append(" ");
                    }
                }
                return sb.ToString().ToUpper();
            }
            catch (System.Exception ex)
            {
                return string.Empty;
            }

        }

        private bool DetectionFiltration()
        {

            if (txtLen.Text.Trim().Length != 0)
            {
                if (int.Parse(txtLen.Text) > 0)
                {
                    string filerData = txtFilter_EPC.Text.Replace(" ", "");
                    if (!StringUtils.IsHexNumber(filerData))
                    {
                        MessageBox.Show(Common.isEnglish ? "Please input the hex filter data!" : "请输入十六进制过滤数据!");
                        return false;
                    }

                    byte[] filerBuff = DataConvert.HexStringToByteArray(filerData);
                    int filerPtr = int.Parse(txtPtr.Text);
                    int filerLen = int.Parse(txtLen.Text);

                    if ((filerLen / 8 + (filerLen % 8 == 0 ? 0 : 1)) * 2 > filerData.Length)
                    {
                        MessageBox.Show(Common.isEnglish ? "filter data length error!" : "过滤数据和长度不匹配!");  //to do
                        return false;
                    }
                }

            }
            return true;
        }
        private void rbEPC_Click(object sender, EventArgs e)
        {
            txtPtr.Text = "32";
        }

        private void rbTID_Click(object sender, EventArgs e)
        {
            txtPtr.Text = "0";
        }

        private void rbUser_Click(object sender, EventArgs e)
        {
            txtPtr.Text = "0";
        }

        private void txtAuthenticateData_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txtAuthenticateData);
            string data = txtAuthenticateData.Text.Replace(" ", "");
            if (data.Length > 0)
            {
                txtDataLen.Text = ((data.Length / 2) + ((data.Length % 2) == 0 ? 0 : 1)).ToString();  // txtRead_Length.Text = ((data.Length / 4) + ((data.Length % 4) == 0 ? 0 : 1)).ToString();
            }
            else
            {
                txtDataLen.Text = "0";
            }
        }
    }
}
