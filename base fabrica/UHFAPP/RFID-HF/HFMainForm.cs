using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinForm_Test;
using UHFAPP.RFID_HF;

namespace UHFAPP.RFID
{
    /**
     * 
     * 使用之前先调用 OpenUsb();打开设备。
     * 退出之前先调用 CloseUsb()关闭设备。
     * 
     * 以上两个函数和UHFAPI 里面的同名函数通用.
     * 
     */
    public partial class RFIDMainForm : Form
    {
        HF14443API hfAPI = new HF14443API();
        HF15693API hf15693API = new HF15693API();
        PSAMAPI psam = new PSAMAPI();
        public RFIDMainForm()
        {
            InitializeComponent();
            //hfAPI.OpenUsb();
            cmbM1TagType.SelectedIndex = 0;
            cmbM1KeyType.SelectedIndex = 0;
            cmbM1Sector.SelectedIndex = 1;
            cmbM1Block.SelectedIndex = 0;

            cmb15693Block.SelectedIndex = 0;

        }
        private void RFIDMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
           //hfAPI.CloseUsb();
        }



        #region 14443A

        /// <summary>
        /// 读
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRead_Click(object sender, EventArgs e)
        {
            byte[] buffType = hfAPI.RequestTypeA(0x52);
            if (buffType == null)
            {
                ShowMessage(Common.isEnglish ? "Card not found!" : "寻卡失败!");
                return;
            }

            byte[] buffCardID = hfAPI.AnticollTypeA();
            if (buffType == null)
            {
                ShowMessage(Common.isEnglish ? "Read failure" : "获取卡片号失败!");
                return;
            }

            int type = hfAPI.SelectTypeA();
            if (type == -1)
            {
                ShowMessage(Common.isEnglish ? "Read failure" : "获取卡片类型失败!");
                return;
            }

            byte cMode = (byte)(0x60 + cmbM1KeyType.SelectedIndex);//0x60:A  ;  0x61:B
            int cBlock = (cmbM1Sector.SelectedIndex * 4) + cmbM1Block.SelectedIndex;
            byte[] pcKey = DataConvert.HexStringToByteArray(txtM1Key.Text);
            bool result = hfAPI.Authentication(cMode, (byte)cBlock, pcKey);
            if (!result)
            {
                ShowMessage(Common.isEnglish ? "The key validation fail" : "卡片认证失败!");
                return;
            }

            byte[] data = hfAPI.ReadBlock((byte)cBlock);
            if (data == null)
            {
                ShowMessage(Common.isEnglish ? "Read failure" : "获取block数据失败!");
                return;
            }

            txtM1Data.Text = DataConvert.ByteArrayToHexString(data);
            ShowMessage(Common.isEnglish ? "Read Success" : "获取成功!");
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWrite_Click(object sender, EventArgs e)
        {
            int sector = cmbM1Sector.SelectedIndex;
            int block = cmbM1Block.SelectedIndex;

            if (sector == 0 && block == 0)
            {
                ShowMessage(Common.isEnglish ? "failure" : "0扇区的0数据块不能写入");
                return;
            }
            else if (sector < 32 && block == 3)
            {
                ShowMessage(Common.isEnglish?"This program does not support the data block write operation, the data is password control block are not familiar with the tag structure please do not write to":"此程序不支持该数据块的写入操作,该数据块是密码控制块");
                return;
            }
            else if (sector > 31 && block == 15)
            {
                ShowMessage(Common.isEnglish ? "This program does not support the data block write operation, the data is password control block are not familiar with the tag structure please do not write to" : "此程序不支持该数据块的写入操作,该数据块是密码控制块");
                return;
            }

            string strData = txtM1Data.Text.Replace(" ","") ;
            if (strData.Length == 0)
            {
                ShowMessage(Common.isEnglish ? "Content to be written can not be empty!" : "写入内容不能为空");
                return;
            }
            else if (!StringUtils.IsHexNumber(strData))
            {
                ShowMessage(Common.isEnglish ? "Please enter the hexadecimal number content" : "请输入十六进制数");
                return;
            }

            byte[] buffType = hfAPI.RequestTypeA(0x52);
            if (buffType == null)
            {
                ShowMessage(Common.isEnglish ? "Card not found!" : "寻卡失败!");
                return;
            }

            byte[] buffCardID = hfAPI.AnticollTypeA();
            if (buffType == null)
            {
                ShowMessage(Common.isEnglish ? "failure" : "获取卡片号失败!");
                return;
            }

            int type = hfAPI.SelectTypeA();
            if (type == -1)
            {
                ShowMessage(Common.isEnglish ? "failure" : "获取卡片类型失败!");
                return;
            }
            byte cMode = (byte)(0x60 + cmbM1KeyType.SelectedIndex);//0x60:A  ;  0x61:B
            int cBlock = (cmbM1Sector.SelectedIndex * 4) + cmbM1Block.SelectedIndex;
            byte[] pcKey = DataConvert.HexStringToByteArray(txtM1Key.Text);
            bool result = hfAPI.Authentication(cMode, (byte)cBlock, pcKey);
            if (!result)
            {
                ShowMessage(Common.isEnglish ? "The key validation fail" : "卡片认证失败!");
                return;
            }

             result = hfAPI.WriteBlock((byte)cBlock, DataConvert.HexStringToByteArray(strData));
            if (!result)
            {
                ShowMessage(Common.isEnglish ? "failure" : "写卡失败!");
                return;
            }

            ShowMessage(Common.isEnglish ? "Success" : "写卡成功!");

        }
        private void txtM1Data_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txtM1Data);
        }

        private void cmbM1TagType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbM1TagType.SelectedIndex == 0)
            {
                //S50
                cmbM1Sector.Items.Clear();
                object[] items = new object[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
                cmbM1Sector.Items.AddRange(items);

                cmbM1Sector.SelectedIndex = 1;
            }
            else if (cmbM1TagType.SelectedIndex == 1)
            {
                //S70
                cmbM1Sector.Items.Clear();
                object[] items = new object[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 };
                cmbM1Sector.Items.AddRange(items);

                cmbM1Sector.SelectedIndex = 1;
            }
        }

        private void cmbM1Sector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbM1Sector.SelectedIndex >= 32)
            {
                int oldSelect = cmbM1Block.SelectedIndex;

                cmbM1Block.Items.Clear();
                object[] items = new object[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
                cmbM1Block.Items.AddRange(items);

                if (oldSelect <= 15)
                {
                    cmbM1Block.SelectedIndex = oldSelect;
                }
                else
                {
                    cmbM1Block.SelectedIndex = 0;
                }
            }
            else
            {
                int oldSelect = cmbM1Block.SelectedIndex;

                cmbM1Block.Items.Clear();
                object[] items = new object[] { 0, 1, 2, 3 };
                cmbM1Block.Items.AddRange(items);

                if (oldSelect <= 3)
                {
                    cmbM1Block.SelectedIndex = oldSelect;
                }
                else
                {
                    cmbM1Block.SelectedIndex = 0;
                }

            }

        }
   
       #endregion

        #region 14443A CPU
         private void btn14443ACPUInit_Click(object sender, EventArgs e)
         {
             txtCPUReceive.Text = "";
             string hexData = txt14443ACPUData.Text.Replace(" ", "");
             if (hexData.Length == 0)
             {
                 ShowMessage(Common.isEnglish ? "Content to be send can not be empty!" : "内容不能为空");
                 return;
             }
             else if (!StringUtils.IsHexNumber(hexData))
             {
                 ShowMessage(Common.isEnglish ? "Please enter the hexadecimal number content" : "请输入十六进制数");
                 return;
             }




             byte[] buffType = hfAPI.RequestTypeA(0x52);
             if (buffType == null)
             {
                 ShowMessage(Common.isEnglish ? "Card not found!" : "寻卡失败!");
                 return;
             }

             byte[] buffCardID = hfAPI.AnticollTypeA();
             if (buffType == null)
             {
                 ShowMessage(Common.isEnglish ? "failure" : "获取卡片号失败!");
                 return;
             }

             int type = hfAPI.SelectTypeA();
             if (type == -1)
             {
                 ShowMessage(Common.isEnglish ? "failure" : "获取卡片类型失败!");
                 return;
             }

             byte[] data=hfAPI.RatsTypeA();
             if (data == null)
             {
                 ShowMessage(Common.isEnglish?">RATS and PPS error":"RATS和PPS出错!");
                 return;
             }
             //RATS返回码-->data

             byte[] result = hfAPI.CpuCommand(DataConvert.HexStringToByteArray(hexData));
             if (result == null)
             {
                 ShowMessage(Common.isEnglish ? "Send failure" : "命令发送出错!");
                 return;
             }

             txtCPUReceive.Text = DataConvert.ByteArrayToHexString(result);

             ShowMessage(Common.isEnglish ? "Success" : "命令发送成功!");
         }
       #endregion

        #region 14443B
         /// <summary>
         /// 14443B
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void btnGetUID_Click(object sender, EventArgs e)
         {
             txtUID.Text = "";
             byte[] data= hfAPI.GetUidTypeB();
             if (data == null)
             {
                 ShowMessage(Common.isEnglish ? "failure" : "获取失败!");
                 return;
             }
             txtUID.Text = DataConvert.ByteArrayToHexString(data);
             ShowMessage(Common.isEnglish ? "Success" : "获取成功!");
         }

         #endregion 

        #region 15693
        private void btn15693Read_Click(object sender, EventArgs e)
        {
            ISO15693Entity entity = hf15693API.Inventory();
            if (entity == null)
            {
                ShowMessage(Common.isEnglish ? "Card not found!" : "寻卡失败!");
                return;
            }

            int block = cmb15693Block.SelectedIndex;
            byte[] data = hf15693API.Read(entity, block);
            if (data == null)
            {
                ShowMessage(Common.isEnglish ? "failure" : "读取卡片失败!");
                return;
            }

            txt15693Data.Text = DataConvert.ByteArrayToHexString(data);
            ShowMessage(Common.isEnglish ? "Success" : "读取成功!");
        }

        private void btn15693Write_Click(object sender, EventArgs e)
        {
            string strData = txt15693Data.Text.Replace(" ","");
            if (strData.Length == 0)
            {
                ShowMessage(Common.isEnglish ? "Content to be written can not be empty!" : "写入内容不能为空");
                return;
            }
            else if (!StringUtils.IsHexNumber(strData))
            {
                ShowMessage(Common.isEnglish ? "Please enter the hexadecimal number content" : "请输入十六进制数");
                return;
            } 

            ISO15693Entity entity = hf15693API.Inventory();
            if (entity == null)
            {
                ShowMessage(Common.isEnglish ? "Card not found!" : "寻卡失败!");
                return;
            }
            int block = cmb15693Block.SelectedIndex;
            byte[] data = DataConvert.HexStringToByteArray(txt15693Data.Text);
            bool result = hf15693API.Write(entity, block, data);
            if (!result)
            {
                ShowMessage(Common.isEnglish ? "failure" : "写卡片失败!");
                return;
            }
            ShowMessage(Common.isEnglish ? "Success" : "写入成功!");

        }

        private void btnAFIWrite_Click(object sender, EventArgs e)
        {
            string afi = txtAFI.Text;

            if (afi.Length != 2)
            {
                ShowMessage(Common.isEnglish ? "Data should be a byte" : "写入的数据必须是1个字节!");
                return;
            }
            else if (!StringUtils.IsHexNumber(afi))
            {
                ShowMessage(Common.isEnglish ? "Please enter the hexadecimal number content" : "请输入十六进制数");
                return;
            }

            ISO15693Entity entity = hf15693API.Inventory();
            if (entity == null)
            {
                ShowMessage(Common.isEnglish ? "Card not found!" : "寻卡失败!");
                return;
            }
            bool result = hf15693API.WriteAFI(entity, Convert.ToByte(afi, 16));
            if (!result)
            {
                ShowMessage(Common.isEnglish ? "failure" : "写失败!");
                return;
            }
            ShowMessage(Common.isEnglish ? "Success" : "写成功!");

        }

        private void btnAFILock_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(Common.isEnglish ? "Are you sure you want to lock it?" : "确定要锁吗？", Common.isEnglish ? "Lock" : "锁", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
            {
                return;
            }

            ISO15693Entity entity = hf15693API.Inventory();
            if (entity == null)
            {
                ShowMessage(Common.isEnglish ? "Card not found!" : "寻卡失败!");
                return;
            }

            bool result = hf15693API.LockAFI();
            if (!result)
            {
                ShowMessage(Common.isEnglish ? "failure" : "锁失败!");
                return;
            }
            ShowMessage(Common.isEnglish ? "Success" : "锁成功!");
        }

        private void btnDsfidWrite_Click(object sender, EventArgs e)
        {
            string dsfid = txtDsfid.Text;

            if (dsfid.Length != 2)
            {
                ShowMessage(Common.isEnglish ? "Data should be a byte" : "写入的数据必须是1个字节!");
                return;
            }
            else if (!StringUtils.IsHexNumber(dsfid))
            {
                ShowMessage(Common.isEnglish ? "Please enter the hexadecimal number content" : "请输入十六进制数");
                return;
            }

            ISO15693Entity entity = hf15693API.Inventory();
            if (entity == null)
            {
                ShowMessage(Common.isEnglish ? "Card not found!" : "寻卡失败!");
                return;
            }
            bool result = hf15693API.WriteDsfid(entity, Convert.ToByte(dsfid, 16));
            if (!result)
            {
                ShowMessage(Common.isEnglish ? "failure" : "写失败!");
                return;
            }
            ShowMessage(Common.isEnglish ? "Success" : "写成功!");
        }

        private void btnDsfidLock_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(Common.isEnglish ? "Are you sure you want to lock it?" : "确定要锁吗？", Common.isEnglish ? "Lock" : "锁", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
            {
                return;
            }

            ISO15693Entity entity = hf15693API.Inventory();
            if (entity == null)
            {
                ShowMessage(Common.isEnglish ? "Card not found!" : "寻卡失败!");
                return;
            }

            bool result = hf15693API.LockDsfid();
            if (!result)
            {
                ShowMessage(Common.isEnglish ? "failure" : "锁失败!");
                return;
            }
            ShowMessage(Common.isEnglish ? "Success" : "锁成功!");
        }

     

        private void txt15693Data_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txt15693Data);
        }
       #endregion

        #region PSAM
            private void btnInit_Click(object sender, EventArgs e)
            {
                txtPsamReceive.Text = "";
                int card = rbCard1.Checked ? 0 : 1;
                if (!psam.Init((byte)card))
                {
                    ShowMessage(Common.isEnglish ? "Initialization failed" : "初始化失败");
                    return;
                }
                byte[] resetData=psam.Reset((byte)card);
                if (resetData == null)
                {
                    ShowMessage(Common.isEnglish ? "failure" : "复位失败");
                    return;
                }
                txtPsamReceive.Text = DataConvert.ByteArrayToHexString(resetData);
                btnPsamSend.Enabled = true;
                btnInit.Enabled = false;
                btnFree.Enabled = true;
                rbCard1.Enabled = false;
                rbCard2.Enabled = false;
                ShowMessage(Common.isEnglish ? "Success" : "成功");
              
            }

            private void btnFree_Click(object sender, EventArgs e)
            {
                int card = rbCard1.Checked ? 0 : 1;
                if (psam.Free((byte)card))
                {
                    btnPsamSend.Enabled = false;
                    btnInit.Enabled = true;
                    btnFree.Enabled = false;
                    rbCard1.Enabled = true;
                    rbCard2.Enabled = true;
                    ShowMessage(Common.isEnglish ? "Success" : "成功");
                    return;
                }
                ShowMessage(Common.isEnglish ? "failure" : "失败");
            }

            private void button3_Click(object sender, EventArgs e)
            {
                txtPsamReceive.Text = "";
                string psamData = txtPsamData.Text.Replace(" ", "");

                if (psamData.Length == 0)
                {
                    ShowMessage(Common.isEnglish ? "The content cannot be empty" : "内容不能为空");
                    return;
                }
                else if (!StringUtils.IsHexNumber(psamData))
                {
                    ShowMessage(Common.isEnglish ? "Please enter the hexadecimal number content" : "请输入十六进制数");
                    return;
                }

                int card = rbCard1.Checked ? 0 : 1;
                byte[] data = psam.TransferCmd((byte)card, DataConvert.HexStringToByteArray(psamData));
                if (data == null)
                {
                    ShowMessage(Common.isEnglish ? "failure" : "失败");
                    return;
                }
                txtPsamReceive.Text = DataConvert.ByteArrayToHexString(data);
                ShowMessage(Common.isEnglish ? "Success" : "成功");
            }

        #endregion

        private void ShowMessage(string msg)
        {
             
                {
                    frmWaitingBox f = new frmWaitingBox((obj, args) =>
                    {
                        System.Threading.Thread.Sleep(500);
                    }, msg);
                    f.ShowDialog(this);
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
        private void RFIDMainForm_KeyDown(object sender, KeyEventArgs e)
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
  
        private void txt14443ACPUData_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txt14443ACPUData);
        }

        private void txtPsamData_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txt14443ACPUData);
        }

        private void btn14443B_Click(object sender, EventArgs e)
        {

            byte[] data=hfAPI.ResetTypeB();
            if (data==null)
            {
                ShowMessage(Common.isEnglish ? "reset fail!" : "重置卡片失败!");
                return;
            }

            txt14443BReceive.Text = "";
            string hexData = txt14443BSendData.Text.Replace(" ", "");
            if (hexData.Length == 0)
            {
                ShowMessage(Common.isEnglish ? "Content to be send can not be empty!" : "内容不能为空");
                return;
            }
            else if (!StringUtils.IsHexNumber(hexData))
            {
                ShowMessage(Common.isEnglish ? "Please enter the hexadecimal number content" : "请输入十六进制数");
                return;
            }

            byte[] result = hfAPI.CpuCommand(DataConvert.HexStringToByteArray(hexData));
            if (result == null)
            {
                ShowMessage(Common.isEnglish ? "Send failure" : "命令发送出错!");
                return;
            }

            txt14443BReceive.Text = DataConvert.ByteArrayToHexString(result);
            ShowMessage(Common.isEnglish ? "Success" : "命令发送成功!");

        }

     
    






    }
}
