using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace UHFAPP.custom.authenticate
{
    public partial class AuthenticateForm : Form
    {

        AuthenticateAPI api = new AuthenticateAPI();

        public AuthenticateForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRead_Click(object sender, EventArgs e)
        {
            if (!DetectionFiltration())
            {
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
            byte[] pwd=new byte[8];

            string result = UHFAPI.getInstance().ReadData(pwd, filterBank, filterAddr, filterDataLen, filterData, 0x03, 192, 8);
            if (result == null || result.Length==0)
            {
                MessageBox.Show("fail");
                txtKey0Data.Text = "";
                return;
            }
            txtKey0Data.Text = result;
            MessageBox.Show("success");
        }
         /// <summary>
         /// 激活
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
        private void btnActivate_Click(object sender, EventArgs e)
        {
            if (!DetectionFiltration())
            {
                return;
            }

            if (MessageBox.Show("Are you sure you want to activate the secret key?", "", MessageBoxButtons.YesNo) == DialogResult.No)
            {
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
            byte[] data = DataConvert.HexStringToByteArray("E200");
            byte[] pwd = new byte[8];

            bool result = UHFAPI.getInstance().WriteData(pwd, filterBank, filterAddr, filterDataLen, filterData, 0x03, 200, 1, data);
            if (!result)
            {
                MessageBox.Show("fail");
                return;
            }

            MessageBox.Show("success!");
            return;
       
        }
        /// <summary>
        /// 写
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWrite_Click(object sender, EventArgs e)
        {
            if (!DetectionFiltration())
            {
                return;
            }
            string key = txtKey0Data.Text.Replace(" ", "");
            if (key.Length == 0)
            {
                MessageBox.Show("The data cannot be empty!");
                return;
            }

            if (!StringUtils.IsHexNumber(key))
            {
                MessageBox.Show("Please input the hex data!");
                return;
            }
            if (key.Length != 32)
            {
                MessageBox.Show("The length of the message must be 16 bytes!");
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
            byte[] pwd = new byte[8];

            bool result = UHFAPI.getInstance().WriteData(pwd, filterBank, filterAddr, filterDataLen, filterData, 0x03, 192, 8, DataConvert.HexStringToByteArray(key));
            if (!result)
            {
                MessageBox.Show("fail");
                return;
            }
            MessageBox.Show("success!");

        }

        /// <summary>
        /// 认证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAuthenticate_Click(object sender, EventArgs e)
        {
            if (!DetectionFiltration())
            {
                return;
            }

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
            byte keyID = byte.Parse(txtAuthenticateKeyID.Text);
            byte[] tData = DataConvert.HexStringToByteArray(txtAuthenticateData.Text);
            int password = 0;

            byte[] result = api.UHFAuthenticate(password, filterBank, filterAddr, filterDataLen, filterData, keyID, tData);
            if (result == null || result.Length == 0)
            {
                MessageBox.Show("fail");
                txtAuthenticateEncryptionData.Text = "";
                return;
            }
            txtAuthenticateEncryptionData.Text = DataConvert.ByteArrayToHexString(result);
            MessageBox.Show("success");
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDecode_Click(object sender, EventArgs e)
        {
            if (decodeKey.Text.Replace(" ", "").Length != 32)
            {
                MessageBox.Show("The length of the key must be 16 bytes!");
                return;
            }

            if (!StringUtils.IsHexNumber(decodeKey.Text.Replace(" ","")))
            {
                MessageBox.Show("Please input the hex key!");
                return;
            }

       
 
               

            byte[] key = DataConvert.HexStringToByteArray(decodeKey.Text);
            byte[] data = DataConvert.HexStringToByteArray(txtDecodeCiphertext.Text);
            byte[] result = api.AesDecrypto(key,data);
            if (result == null || result.Length == 0)
            {
                txtC.Text = "";
                txtRnd.Text = "";
                txtMsg.Text = "";
                MessageBox.Show("fail");
                return;
            }
            else
            {
                string hexData = DataConvert.ByteArrayToHexString(result).Replace(" ","") ;
                txtC.Text = HexFormat(hexData.Substring(0,4));
                txtRnd.Text = HexFormat(hexData.Substring(4, 8));
                txtMsg.Text = HexFormat(hexData.Substring(12, 20));
                MessageBox.Show("success");
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
        private void txtKey0Data_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txtKey0Data);
            string data = txtKey0Data.Text.Replace(" ", "");
            if (data.Length > 0)
            {
                txtKey0DataLen.Text = ((data.Length / 2) + ((data.Length % 2) == 0 ? 0 : 1)).ToString();  // txtRead_Length.Text = ((data.Length / 4) + ((data.Length % 4) == 0 ? 0 : 1)).ToString();
            }
            else
            {
                txtKey0DataLen.Text = "0";
            }
        }
        private void txtData_TextChanged(object sender, EventArgs e)
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

            //if (txtDataLen.Text.Replace(" ", "").Length > 20)
            //{
            //    txtDataLen.Text = txtDataLen.Text.Replace(" ", "").Substring(0, 20);
            //}
        }
        private void txtEncryptionData2_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txtAuthenticateEncryptionData);
            string data = txtAuthenticateEncryptionData.Text.Replace(" ", "");
            if (data.Length > 0)
            {
                txtEncryptionData2Len.Text = ((data.Length / 2) + ((data.Length % 2) == 0 ? 0 : 1)).ToString();  // txtRead_Length.Text = ((data.Length / 4) + ((data.Length % 4) == 0 ? 0 : 1)).ToString();
            }
            else
            {
                txtEncryptionData2Len.Text = "0";
            }
        }
        private void decodeKey_TextChanged(object sender, EventArgs e)
        {
            FormatHex(decodeKey);
            string data = decodeKey.Text.Replace(" ", "");
            if (data.Length > 0)
            {
                lblencryptoKeyLen.Text = ((data.Length / 2) + ((data.Length % 2) == 0 ? 0 : 1)).ToString();  // txtRead_Length.Text = ((data.Length / 4) + ((data.Length % 4) == 0 ? 0 : 1)).ToString();
            }
            else
            {
                lblencryptoKeyLen.Text = "0";
            }
        }
        private void txtData2_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txtDecodeCiphertext);
            string data = txtDecodeCiphertext.Text.Replace(" ", "");
            if (data.Length > 0)
            {
                txtData2Len.Text = ((data.Length / 2) + ((data.Length % 2) == 0 ? 0 : 1)).ToString();  // txtRead_Length.Text = ((data.Length / 4) + ((data.Length % 4) == 0 ? 0 : 1)).ToString();
            }
            else
            {
                txtData2Len.Text = "0";
            }
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
        private void AuthenticateForm_KeyDown(object sender, KeyEventArgs e)
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

      
      

       
         
    }
}
