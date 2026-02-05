using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using BLEDeviceAPI;

namespace UHFAPP.custom
{
    public partial class CryptoTransmitForm : Form
    {
        //crypto module transmit
        //pin--point of data which send to crypto mudle 
        //inLen--the length of send data 
        //pout--point of receive crypto returned data
        //outLen--the length of received data
        //wait_recv_ms--the maxinum millsecond while waiting for crypto module return 

       [DllImport("UHFAPI.dll",CallingConvention = CallingConvention.Cdecl)]
       private extern static int CryptoTransmit(byte[] pin, short inLen,byte[] pout,byte[] outLen,  short wait_recv_ms);


        public CryptoTransmitForm()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] pin = DataConvert.HexStringToByteArray(txtSendData.Text);
            if (pin != null && pin.Length > 0)
            {
                byte[] pout = new byte[512];
                byte[] outLen = new byte[1];
                short wait_recv_ms = 1000;
                int result = CryptoTransmit(pin, (short)pin.Length, pout, outLen, wait_recv_ms);
                if (result == 0)
                {
                    byte[] outData = Utils.CopyArray(pout, outLen[0]);
                    textBox2.Text = DataConvert.ByteArrayToHexString(outData);
                }
                else
                {
                    MessageBox.Show("失败!");
                }
            }
            else
            {
                MessageBox.Show("输入数据不能为空!");
            }


        }


        bool isDelete = false;
        private void ReadWriteTagForm_KeyDown(object sender, KeyEventArgs e)
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

        private void txtSendData_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txtSendData);
            string data = txtSendData.Text.Replace(" ", "");
            if (data.Length > 0)
            {
                label3.Text = ((data.Length / 2) + ((data.Length % 2) == 0 ? 0 : 1)).ToString();  // txtRead_Length.Text = ((data.Length / 4) + ((data.Length % 4) == 0 ? 0 : 1)).ToString();
            }
            else
            {
                label3.Text = "0";
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

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }
    }
}
