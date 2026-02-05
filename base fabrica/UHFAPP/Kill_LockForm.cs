using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinForm_Test;

namespace UHFAPP
{
    public partial class Kill_LockForm : BaseForm
    {
        private string comboAction1 = "可读可写";
        private string comboAction2 = "可读不可写";
        private string comboAction3 = "不可读可写";
        private string comboAction4 = "不可读不可写";

        private string comboAction5 = "保留";
        private string comboAction6 = "不需要鉴别";
        private string comboAction7 = "需要鉴别,不需要安全通信";
        private string comboAction8 = "需要鉴别,需要安全通信";

        private string txtUserNumber = "用户区编号:";

        int userNumber = 0;

        public Kill_LockForm()
        {
            InitializeComponent();
        }
        public Kill_LockForm(bool isOpen)
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
        #region 过滤
        private void txtFilter_EPC_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txtFilter_EPC);
            string data = txtFilter_EPC.Text.Replace(" ", "");
            if (data.Length > 0)
            {
                label29.Text = ((data.Length / 2) + ((data.Length % 2) == 0 ? 0 : 1)).ToString();  // txtRead_Length.Text = ((data.Length / 4) + ((data.Length % 4) == 0 ? 0 : 1)).ToString();
            }
            else
            {
                label29.Text = "0";
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
        private void txtPtr_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            if (!StringUtils.IsNumber(txt.Text))
            {
                if (rbEPC.Checked)
                {
                    txt.Text = "32";
                }
                else {
                    txt.Text = "0";
                }
            }
        }

        private void txtLen_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            if (!StringUtils.IsNumber(txt.Text))
            {
                txt.Text = "0";
            }
        }
        #endregion


        #region BlockPermalock

        private void button3_Click(object sender, EventArgs e)
        {

            if (!DetectionFiltration())
                return;

            string filerData = txtFilter_EPC.Text.Replace(" ", "");
            string accessPwd = txtBlockPermalockPwd.Text.Replace(" ", "");
 
            if (accessPwd.Length != 8)
            {
                MessageBoxEx.Show(this,Common.isEnglish?"The length of the password must be 8!":"密码长度必须为8位!");
                return;
            }
            int bank = cmbBlockPermalockBank.SelectedIndex;
            byte[] pwd = DataConvert.HexStringToByteArray(accessPwd);
            byte[] filerBuff = DataConvert.HexStringToByteArray(filerData);
            int Ptr = int.Parse(txtBlockPermalockPtr.Text);

            int FilterStartaddr = int.Parse(txtPtr.Text);
            int FilterLength = int.Parse(txtLen.Text);
            int FilterBank = 1;
            if (rbTID.Checked)
            {
                FilterBank = 2;
            }
            else if (rbUser.Checked)
            {
                FilterBank = 3;
            }


            int ReadLock = cmbBlockPermalockReadLock.SelectedIndex;
            int uRange = Ptr + 1;

            int[] data = new int[16];
            if (cbBlock1.Checked) data[0] = 1;
            if (cbBlock2.Checked) data[1] = 1;
            if (cbBlock3.Checked) data[2] = 1;
            if (cbBlock4.Checked) data[3] = 1;
            if (cbBlock5.Checked) data[4] = 1;
            if (cbBlock6.Checked) data[5] = 1;
            if (cbBlock7.Checked) data[6] = 1;
            if (cbBlock8.Checked) data[7] = 1;
            if (cbBlock9.Checked) data[8] = 1;
            if (cbBlock10.Checked) data[9] = 1;
            if (cbBlock11.Checked) data[10] = 1;
            if (cbBlock12.Checked) data[11] = 1;
            if (cbBlock13.Checked) data[12] = 1;
            if (cbBlock14.Checked) data[13] = 1;
            if (cbBlock15.Checked) data[14] = 1;
            if (cbBlock16.Checked) data[15] = 1;


            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < data.Length; k++) {
                sb.Append(data[k]);
            }
            byte[] uMaskbuf = new byte[2];
            uMaskbuf[0] = Convert.ToByte(sb.ToString().Substring(0, 8), 2);
            uMaskbuf[1] = Convert.ToByte(sb.ToString().Substring(8, 8), 2);


            label8.Text = "Maskbuf:" + DataConvert.ByteArrayToHexString(uMaskbuf);

            string msg = "";
            if (uhf.BlockPermalock(pwd, (byte)FilterBank, FilterStartaddr,
                FilterLength, filerBuff, (byte)ReadLock, (byte)bank, Ptr, (byte)uRange, uMaskbuf))
            {
                msg = "success";
                if (ReadLock == 0)
                {
                    label8.Text = "Maskbuf:" + DataConvert.ByteArrayToHexString(uMaskbuf);
                    string str1 = System.Convert.ToString(uMaskbuf[0], 2);
                    str1 = "0000000".Substring(0, 8 - str1.Length) + str1;
                    string str2 = System.Convert.ToString(uMaskbuf[1], 2);
                    str2 = "0000000".Substring(0, 8 - str2.Length) + str2;
                    cbBlock1.Checked = (str1.Substring(0, 1) == "1" ? true : false);
                    cbBlock2.Checked = (str1.Substring(1, 1) == "1" ? true : false);
                    cbBlock3.Checked = (str1.Substring(2, 1) == "1" ? true : false);
                    cbBlock4.Checked = (str1.Substring(3, 1) == "1" ? true : false);
                    cbBlock5.Checked = (str1.Substring(4, 1) == "1" ? true : false);
                    cbBlock6.Checked = (str1.Substring(5, 1) == "1" ? true : false);
                    cbBlock7.Checked = (str1.Substring(6, 1) == "1" ? true : false);
                    cbBlock8.Checked = (str1.Substring(7, 1) == "1" ? true : false);

                    cbBlock9.Checked = (str2.Substring(0, 1) == "1" ? true : false);
                    cbBlock10.Checked = (str2.Substring(1, 1) == "1" ? true : false);
                    cbBlock11.Checked = (str2.Substring(2, 1) == "1" ? true : false);
                    cbBlock12.Checked = (str2.Substring(3, 1) == "1" ? true : false);
                    cbBlock13.Checked = (str2.Substring(4, 1) == "1" ? true : false);
                    cbBlock14.Checked = (str2.Substring(5, 1) == "1" ? true : false);
                    cbBlock15.Checked = (str2.Substring(6, 1) == "1" ? true : false);
                    cbBlock16.Checked = (str2.Substring(7, 1) == "1" ? true : false);
                }
            }
            else
            {
                msg = "failure"; 
            }
            frmWaitingBox f = new frmWaitingBox((obj, args) =>
            {
                System.Threading.Thread.Sleep(500);
            }, msg);
            f.ShowDialog(this);
        }

 

        private void txtBlockPermalockPtr_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            if (!StringUtils.IsNumber(txt.Text))
            {
                txt.Text = "0";
            }
            int index = cmbBlockPermalockBank.SelectedIndex;
            if (index == 0)
            {
                if (int.Parse(txt.Text) < 2)
                {
                    txt.Text = "2";
                }
            }
        }



        private void txtBlockPermalockPwd_TextChanged(object sender, EventArgs e)
        {
            FormatHex_PWD(txtBlockPermalockPwd);
        }

        private void cmbBlockPermalockBank_SelectedIndexChanged(object sender, EventArgs e)
        {

            int index = cmbBlockPermalockBank.SelectedIndex;
            if (index == 1)
            {
                if (int.Parse(txtBlockPermalockPtr.Text) < 2)
                {
                    txtBlockPermalockPtr.Text = "2";
                }
            }

            if (index == 3)
            {
                enabledBlock(false);
            }
            else {
                enabledBlock(true);
            }
        }
    
        #endregion
        #region lock

    

        private void txtLockPwd_TextChanged(object sender, EventArgs e)
        {
            FormatHex_PWD(txtLockPwd);
        }
       

        private void button5_Click(object sender, EventArgs e)
        {
            if (!DetectionFiltration())
                return;

            string filerData = txtFilter_EPC.Text.Replace(" ", "");
            string accessPwd = txtLockPwd.Text.Replace(" ", "");
            int FilterStartaddr = int.Parse(txtPtr.Text);
            int FilterLength = int.Parse(txtLen.Text);

         
            if (accessPwd.Length != 8)
            {
                MessageBoxEx.Show(this,Common.isEnglish?"The length of the password must be 8!":"密码长度必须为8位!");
                return;
            }

            if ((FilterLength / 8 + (FilterLength % 8 == 0 ? 0 : 1)) * 2 > filerData.Length)
            {
                MessageBox.Show(Common.isEnglish ? "filter data length error!" : "过滤数据和长度不匹配!");  //to do
                return;
            }

            byte[] pwd = DataConvert.HexStringToByteArray(accessPwd);
            byte[] filerBuff = DataConvert.HexStringToByteArray(filerData);



            int FilterBank = 1;
            if (rbTID.Checked) 
            {
                FilterBank = 2;
            }
            else if (rbUser.Checked)
            {
                FilterBank = 3;
            }
         
            byte[] lockbuf = new byte[3];

            int[] ilockCode=new int[20];
            if (cbKill.Checked)
            {
                if (rbTemporaryOpen.Checked || rbTemporaryLock.Checked)
                {
                    ilockCode[19] = 1;
                    ilockCode[9] = rbTemporaryOpen.Checked ? 0 : 1 ;
                }
                else if (rbPermanentOpen.Checked || rbPermanentLock.Checked)
                {
                    ilockCode[18] = 1;
                    ilockCode[8] = 1;
                    ilockCode[19] = 1;
                    ilockCode[9] = rbPermanentOpen.Checked ? 0 : 1;
                }      
            }
            if (cbAccess.Checked)
            {
                if (rbTemporaryOpen.Checked || rbTemporaryLock.Checked)
                {
                    ilockCode[17] = 1;
                    ilockCode[7] = rbTemporaryOpen.Checked ? 0 : 1;
                }
                else if (rbPermanentOpen.Checked || rbPermanentLock.Checked)
                {
                    ilockCode[16] = 1;
                    ilockCode[6] = 1;
                    ilockCode[17] = 1;
                    ilockCode[7] = rbPermanentOpen.Checked ? 0 : 1;
                }  
            }
            if (cbEPC.Checked) {
                if (rbTemporaryOpen.Checked || rbTemporaryLock.Checked)
                {
                    ilockCode[15] = 1;
                    ilockCode[5] = rbTemporaryOpen.Checked ? 0 : 1;
                }
                else if (rbPermanentOpen.Checked || rbPermanentLock.Checked)
                {
                    ilockCode[14] = 1;
                    ilockCode[4] = 1;
                    ilockCode[15] = 1;
                    ilockCode[5] = rbPermanentOpen.Checked ? 0 : 1;
                }  
            }
            if (cbTID.Checked) {
                if (rbTemporaryOpen.Checked || rbTemporaryLock.Checked)
                {
                    ilockCode[13] = 1;
                    ilockCode[3] = rbTemporaryOpen.Checked ? 0 : 1;
                }
                else if (rbPermanentOpen.Checked || rbPermanentLock.Checked)
                {
                    ilockCode[12] = 1;
                    ilockCode[12] = 1;
                    ilockCode[13] = 1;
                    ilockCode[3] = rbPermanentOpen.Checked ? 0 : 1;
                }  
            }
            if (cbUser.Checked) {
                if (rbTemporaryOpen.Checked || rbTemporaryLock.Checked)
                {
                    ilockCode[11] = 1;
                    ilockCode[1] = rbTemporaryOpen.Checked ? 0 : 1;
                }
                else if (rbPermanentOpen.Checked || rbPermanentLock.Checked)
                {
                    ilockCode[0] = 1;
                    ilockCode[10] = 1;
                    ilockCode[11] = 1;
                    ilockCode[1] = rbPermanentOpen.Checked ? 0 : 1;
                } 
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("0000");
            for (int k = ilockCode.Length-1; k >=0; k--)
            {
                sb.Append(ilockCode[k]);
            }
            lockbuf[0]=Convert.ToByte(sb.ToString().Substring(0,8),2);
            lockbuf[1] = Convert.ToByte(sb.ToString().Substring(8, 8), 2);
            lockbuf[2] = Convert.ToByte(sb.ToString().Substring(16, 8), 2);

            label7.Text ="LockData:"+ DataConvert.ByteArrayToHexString(lockbuf);
            string msg = "";
            if (uhf.LockTag(pwd, (byte)FilterBank, FilterStartaddr, FilterLength, filerBuff, lockbuf))
            {
                msg = Common.isEnglish?"success":"成功!";
            }
            else
            {
                msg = Common.isEnglish?"failure":"失败!";  
            }
            frmWaitingBox f = new frmWaitingBox((obj, args) =>
            {
                System.Threading.Thread.Sleep(500);
            }, msg);
            f.ShowDialog(this);

        }

        #endregion
        #region kill
         
        private void txtRead_Epc_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txtFilter_EPC);
        }

        private void txtRead_AccessPwd_TextChanged(object sender, EventArgs e)
        {
            FormatHex_PWD(txtKill_AccessPwd);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (!DetectionFiltration())
                return;

            string filerData = txtFilter_EPC.Text.Replace(" ", "");
            string accessPwd = txtKill_AccessPwd.Text.Replace(" ", "");
            byte[] pwd = DataConvert.HexStringToByteArray(accessPwd);
            byte[] filerBuff = DataConvert.HexStringToByteArray(filerData);
            int FilterStartaddr = int.Parse(txtPtr.Text);
            int FilterLength = int.Parse(txtLen.Text);
            int FilterBank = 1;
            if (rbTID.Checked)
            {
                FilterBank = 2;
            }
            else if (rbUser.Checked)
            {
                FilterBank = 3;
            }
          
            if (accessPwd.Length != 8)
            {
                MessageBoxEx.Show(this,Common.isEnglish?"The length of the password must be 8!":"密码长度必须为8位!");
                return;
            }
            if (uhf.KillTag(pwd, (byte)FilterBank, FilterStartaddr, FilterLength, filerBuff))
            {
                MessageBoxEx.Show(this,Common.isEnglish?"Kill success!":"Kill成功!");
            }
            else
            {
                MessageBoxEx.Show(this,Common.isEnglish?"Kill failure!":"Kill失败!");
            }
        }
        #endregion

        private void Kill_LockForm_Load(object sender, EventArgs e)
        {
            lock_CheckedChanged(null,null);
            MainForm.eventOpen += MainForm_eventOpen;
            cmbBlockPermalockReadLock.SelectedIndex = 0;
            cmbBlockPermalockBank.SelectedIndex = 3;
            enabledBlock(false);

            MainForm.eventSwitchUI += MainForm_eventSwitchUI;
            MainForm_eventSwitchUI();
            txtFilter_EPC.Text = Common.tag;
            textBox2.Text = "E003";

            if (txtFilter_EPC.Text.Replace(" ", "").Length > 0)
            {
                txtLen.Text = (txtFilter_EPC.Text.Replace(" ","").Length/2 *8)+"";
            }
            label17.Text = "";
          
        }

        private void Kill_LockForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.eventOpen -= MainForm_eventOpen;
            MainForm.eventSwitchUI -= MainForm_eventSwitchUI;
        }

        private void lock_CheckedChanged(object sender, EventArgs e)
        {
            byte[] lockbuf = new byte[3];

            int[] ilockCode = new int[20];
            if (cbKill.Checked)
            {
                if (rbTemporaryOpen.Checked || rbTemporaryLock.Checked)
                {
                    ilockCode[19] = 1;
                    ilockCode[9] = rbTemporaryOpen.Checked ? 0 : 1;
                }
                else if (rbPermanentOpen.Checked || rbPermanentLock.Checked)
                {
                    ilockCode[18] = 1;
                    ilockCode[8] = 1;
                    ilockCode[19] = 1;
                    ilockCode[9] = rbPermanentOpen.Checked ? 0 : 1;
                }
            }
            if (cbAccess.Checked)
            {
                if (rbTemporaryOpen.Checked || rbTemporaryLock.Checked)
                {
                    ilockCode[17] = 1;
                    ilockCode[7] = rbTemporaryOpen.Checked ? 0 : 1;
                }
                else if (rbPermanentOpen.Checked || rbPermanentLock.Checked)
                {
                    ilockCode[16] = 1;
                    ilockCode[6] = 1;
                    ilockCode[17] = 1;
                    ilockCode[7] = rbPermanentOpen.Checked ? 0 : 1;
                }
            }
            if (cbEPC.Checked)
            {
                if (rbTemporaryOpen.Checked || rbTemporaryLock.Checked)
                {
                    ilockCode[15] = 1;
                    ilockCode[5] = rbTemporaryOpen.Checked ? 0 : 1;
                }
                else if (rbPermanentOpen.Checked || rbPermanentLock.Checked)
                {
                    ilockCode[14] = 1;
                    ilockCode[4] = 1;
                    ilockCode[15] = 1;
                    ilockCode[5] = rbPermanentOpen.Checked ? 0 : 1;
                }
            }
            if (cbTID.Checked)
            {
                if (rbTemporaryOpen.Checked || rbTemporaryLock.Checked)
                {
                    ilockCode[13] = 1;
                    ilockCode[3] = rbTemporaryOpen.Checked ? 0 : 1;
                }
                else if (rbPermanentOpen.Checked || rbPermanentLock.Checked)
                {
                    ilockCode[12] = 1;
                    ilockCode[12] = 1;
                    ilockCode[13] = 1;
                    ilockCode[3] = rbPermanentOpen.Checked ? 0 : 1;
                }
            }
            if (cbUser.Checked)
            {
                if (rbTemporaryOpen.Checked || rbTemporaryLock.Checked)
                {
                    ilockCode[11] = 1;
                    ilockCode[1] = rbTemporaryOpen.Checked ? 0 : 1;
                }
                else if (rbPermanentOpen.Checked || rbPermanentLock.Checked)
                {
                    ilockCode[0] = 1;
                    ilockCode[10] = 1;
                    ilockCode[11] = 1;
                    ilockCode[1] = rbPermanentOpen.Checked ? 0 : 1;
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("0000");
            for (int k = ilockCode.Length - 1; k >= 0; k--)
            {
                sb.Append(ilockCode[k]);
            }
            lockbuf[0] = Convert.ToByte(sb.ToString().Substring(0, 8), 2);
            lockbuf[1] = Convert.ToByte(sb.ToString().Substring(8, 8), 2);
            lockbuf[2] = Convert.ToByte(sb.ToString().Substring(16, 8), 2);

            label7.Text = "LockData:" + DataConvert.ByteArrayToHexString(lockbuf);
        }



        private void FormatHex_PWD(TextBox txt)
        {
            if (isDelete) return;
            string data = txt.Text.Trim().Replace(" ", "");
            if (data != string.Empty)
            {
                if (data.Length > 8)
                {
                    data = data.Substring(0, 8);
                }
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

        private void cbBlock1_Click(object sender, EventArgs e)
        {

            int[] data = new int[16];
            if (cbBlock1.Checked) data[0] = 1;
            if (cbBlock2.Checked) data[1] = 1;
            if (cbBlock3.Checked) data[2] = 1;
            if (cbBlock4.Checked) data[3] = 1;
            if (cbBlock5.Checked) data[4] = 1;
            if (cbBlock6.Checked) data[5] = 1;
            if (cbBlock7.Checked) data[6] = 1;
            if (cbBlock8.Checked) data[7] = 1;
            if (cbBlock9.Checked) data[8] = 1;
            if (cbBlock10.Checked) data[9] = 1;
            if (cbBlock11.Checked) data[10] = 1;
            if (cbBlock12.Checked) data[11] = 1;
            if (cbBlock13.Checked) data[12] = 1;
            if (cbBlock14.Checked) data[13] = 1;
            if (cbBlock15.Checked) data[14] = 1;
            if (cbBlock16.Checked) data[15] = 1;


            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < data.Length; k++)
            {
                sb.Append(data[k]);
            }
            byte[] uMaskbuf = new byte[2];
            uMaskbuf[0] = Convert.ToByte(sb.ToString().Substring(0, 8), 2);
            uMaskbuf[1] = Convert.ToByte(sb.ToString().Substring(8, 8), 2);


            label8.Text = "Maskbuf:" + DataConvert.ByteArrayToHexString(uMaskbuf);
        }


        private void enabledBlock(bool enable)
        {

            cbBlock16.Enabled = enable;
            cbBlock10.Enabled = enable;
            cbBlock11.Enabled = enable;
            cbBlock12.Enabled = enable;
            cbBlock13.Enabled = enable;
            cbBlock14.Enabled = enable;
            cbBlock15.Enabled = enable;
            cbBlock16.Enabled = enable;
            cbBlock9.Enabled = enable;
        }

        private void MainForm_eventSwitchUI() {

            int comboBank_index = comboBank.SelectedIndex;
            int comboConfig_index = comboConfig.SelectedIndex;
            int comboAction_index = comboAction.SelectedIndex;

            if (Common.isEnglish)
            {
                groupBox2.Text = "lock";
         
                label2.Text = "Access Pwd:";
                label4.Text = "Can't use the default password";
                rbTemporaryOpen.Text = "Open";
                rbTemporaryLock.Text = "Lock";
                rbPermanentOpen.Text = "Permanent Open";
                rbPermanentLock.Text = "Permanent Lock";
                cbKill.Text = "Kill-pwd";
                cbAccess.Text = "Access-pwd";
                button5.Text = "Confirm";

                groupBox1.Text = "Kill";

                label19.Text = "Access Pwd:";
                label5.Text = "Access Pwd:";
                label18.Text=label1.Text = label6.Text = "Can't use the default password";
                button2.Text = "kill";

                label14.Text = "Bank:";
                label15.Text = "Config";
                label16.Text = "Action:";

                label26.Text = "Bank:";
                label25.Text = "Ptr:";
                label23.Text = "Access-pwd:";
                label20.Text = "ReadLock:";
                button3.Text = "Confirm";
                label9.Text = "Access-pwd:";
                label10.Text = "cmd:";

                button1.Text = "Confirm";
                groupBox6.Text = "filter";
                button4.Text = "Confirm";

                groupBox9.Text = "GB/GJB Lock";
                 
                 comboBank.Items.Clear();
                 comboBank.Items.Add("TagInfo");
                 comboBank.Items.Add("Encode");
                 comboBank.Items.Add("Secure");
                 comboBank.Items.Add("User");

                 comboConfig.Items.Clear();
                 comboConfig.Items.Add("Storage area property");
                 comboConfig.Items.Add("Secure");

                comboAction1 = "Read-write";
                comboAction2 = "Read only";
                comboAction3 = "Write only";
                comboAction4 = "Unreadable and Writable";

                comboAction5 = "Reserved";
                comboAction6 = "No identification is required.";
                comboAction7 = "Need authentication, no need for secure communication";
                comboAction8 = "Need identification, need secure communication";

                txtUserNumber = "User Area Number:";
            }
            else {

                groupBox2.Text = "锁";
        
                label2.Text = "访问密码:";
                label4.Text = "不能使用默认密码";
                rbTemporaryOpen.Text = "开放";
                rbTemporaryLock.Text = "锁";
                rbPermanentOpen.Text = "永久开放";
                rbPermanentLock.Text = "永久锁定";
                button5.Text = "确定";

                groupBox1.Text = "Kill";

                label19.Text = "访问密码:";
                label5.Text = "访问密码:";
                label18.Text=label1.Text = label6.Text = "不能使用默认密码";
                button2.Text = "kill标签";

                label14.Text = "存储区域:";
                label15.Text = "配置:";
                label16.Text = "动作:";
              
                label26.Text = "存储区域:";
                label25.Text = "起始地址:";
                label23.Text = "访问密码:";
                label20.Text = "操作方式:";
                button3.Text = "确定";
                groupBox6.Text = "过滤";

                label9.Text = "密码:";
                label10.Text = "命令:";
                button1.Text="确定";
                button4.Text = "确定";

 
                comboAction1 = "可读可写";
                comboAction2 = "可读不可写";
                comboAction3 = "不可读可写";
                comboAction4 = "不可读不可写";

                comboAction5 = "保留";
                comboAction6 = "不需要鉴别";
                comboAction7 = "需要鉴别,不需要安全通信";
                comboAction8 = "需要鉴别,需要安全通信";

                txtUserNumber = "用户区编号:";
                groupBox9.Text = "国标/国军标 Lock";

                comboBank.Items.Clear();
                comboBank.Items.Add("标签信息区");
                comboBank.Items.Add("编码区");
                comboBank.Items.Add("安全区");
                comboBank.Items.Add("用户区");

                comboConfig.Items.Clear();
                comboConfig.Items.Add("存储区属性");
                comboConfig.Items.Add("安全模式");
            }

            if (comboBank_index >= 0) {
                comboBank.SelectedIndex = comboBank_index;
            }

            if (comboConfig_index >= 0)
            {
                comboConfig.SelectedIndex = comboConfig_index;
            }

            if (comboAction_index >= 0)
            {
                comboAction.SelectedIndex = comboAction_index;
            }
        }
        public bool DetectionFiltration()
        {
            if (int.Parse(txtLen.Text) > 0)
            {
                string filerData = txtFilter_EPC.Text.Replace(" ", "");
                if (!StringUtils.IsHexNumber(filerData))
                {
                    MessageBox.Show(Common.isEnglish ? "Please input the hex filter data!" : "请输入十六进制过滤数据!");
                    return false;
                }
            }
            return true;
        }

        #region Deactivate/re-activation
            private void textBox2_TextChanged(object sender, EventArgs e)
            {
                //FormatHex(textBox2);
            }

            private void button1_Click(object sender, EventArgs e)
            {
                if (!DetectionFiltration())
                    return;

                string filerData = txtFilter_EPC.Text.Replace(" ", "");
                string accessPwd = textBox1.Text.Replace(" ", "");
                string cmd = textBox2.Text.Replace(" ","");
                if (accessPwd.Length != 8)
                {
                    MessageBoxEx.Show(this, Common.isEnglish ? "The length of the password must be 8!" : "密码长度必须为8位!");
                    return;
                }
                if (cmd.Length != 4)
                {
                    MessageBoxEx.Show(this, Common.isEnglish ? "The length of the cmd must be 4!" : "命令必须是两个字节!");
                    return;
                }

                if (!StringUtils.IsHexNumber(cmd))
                {
                    MessageBox.Show(Common.isEnglish ? "Please input the hex cmd!" : "请输入十六进制命令!");
                    return;
                }

            
                byte[] pwd = DataConvert.HexStringToByteArray(accessPwd);
                int FilterStartaddr = int.Parse(txtPtr.Text);
                int FilterLength = int.Parse(txtLen.Text);
                byte[] filerBuff = DataConvert.HexStringToByteArray(filerData);
                int FilterBank = 1;
                if (rbTID.Checked)
                {
                    FilterBank = 2;
                }
                else if (rbUser.Checked)
                {
                    FilterBank = 3;
                }

                int icmd = Int32.Parse(cmd, System.Globalization.NumberStyles.HexNumber);

                if (uhf.Deactivate(icmd, pwd, (byte)FilterBank, FilterStartaddr, FilterLength, filerBuff))
                {
                    MessageBoxEx.Show(this, Common.isEnglish ? "success!" : "成功!");
                }
                else
                {
                    MessageBoxEx.Show(this, Common.isEnglish ? "fail!" : "失败!");
                }
            }
            private void textBox1_TextChanged(object sender, EventArgs e)
            {
                FormatHex_PWD(textBox1);
            }
        #endregion


            bool isDelete = false;
            private void Kill_LockForm_KeyDown(object sender, KeyEventArgs e)
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
            #region 国际标签锁

                private void txtGBPWD_TextChanged(object sender, EventArgs e)
                {
                    FormatHex_PWD(txtGBPWD);
                }


                //配置
                private void comboConfig_SelectedIndexChanged(object sender, EventArgs e)
                {
                    comboAction.Items.Clear();
                    comboAction.Enabled = true;
                    if (comboConfig.SelectedIndex == 0)
                    {
                        comboAction.Items.Add(comboAction1);
                        comboAction.Items.Add(comboAction2);
                        comboAction.Items.Add(comboAction3);
                        comboAction.Items.Add(comboAction4);
                        comboAction.SelectedIndex = 0;
                    }
                    else
                    {
                        comboAction.Items.Add(comboAction5);
                        comboAction.Items.Add(comboAction6);
                        comboAction.Items.Add(comboAction7);
                        comboAction.Items.Add(comboAction8);
                        comboAction.SelectedIndex =1;
                    }
                }

                 //存储区
                private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
                {
                    if (comboBank.SelectedIndex == 3)
                    {
                        ChoiceNumberForm choiceForm = new ChoiceNumberForm();
                        choiceForm.StartPosition = FormStartPosition.CenterParent;
                        choiceForm.ShowDialog();
                        userNumber = choiceForm.number;
                        label17.Text = txtUserNumber + userNumber;

                    }
                    else
                    {
                        label17.Text = "";
                    }
                }

                private void button4_Click(object sender, EventArgs e)
                {
                    if (!DetectionFiltration())
                        return;

                    string filerData = txtFilter_EPC.Text.Replace(" ", "");
                    string accessPwd = txtGBPWD.Text.Replace(" ", "");
                    byte[] pwd = DataConvert.HexStringToByteArray(accessPwd);
                    byte[] filerBuff = DataConvert.HexStringToByteArray(filerData);
                    int FilterStartaddr = int.Parse(txtPtr.Text);
                    int FilterLength = int.Parse(txtLen.Text);
                    int FilterBank = 1;
                    if (rbTID.Checked)
                    {
                        FilterBank = 2;
                    }
                    else if (rbUser.Checked)
                    {
                        FilterBank = 3;
                    }

                    if (accessPwd.Length != 8)
                    {
                        MessageBoxEx.Show(this, Common.isEnglish ? "The length of the password must be 8!" : "密码长度必须为8位!");
                        return;
                    }

                    if (comboBank.SelectedIndex < 0 || comboConfig.SelectedIndex < 0 || comboAction.SelectedIndex < 0) {

                        MessageBoxEx.Show(this, Common.isEnglish ? "failure!" : "失败!");
                        return;
                    }

                    byte memory = 0;
                    if (comboBank.SelectedIndex == 0) {
                        memory = 0;
                    }
                    else if (comboBank.SelectedIndex == 1) {
                        memory = 0x10;
                    }
                    else if (comboBank.SelectedIndex == 2)
                    {
                        memory = 0x20;
                    }
                    else if (comboBank.SelectedIndex == 3)
                    {
                        memory = (byte) (0x30 + userNumber);
                    }
                    byte config = (byte)comboConfig.SelectedIndex;
                    byte action = (byte)comboAction.SelectedIndex;

                    string msg = "";
                    if (uhf.GBTagLock(pwd, (byte)FilterBank, FilterStartaddr, FilterLength, filerBuff, memory,config,action ))
                    {
                        msg = Common.isEnglish ? "success" : "成功!";
                    }
                    else
                    {
                        msg = Common.isEnglish ? "failure" : "失败!";
                    }
                    frmWaitingBox f = new frmWaitingBox((obj, args) =>
                    {
                        System.Threading.Thread.Sleep(500);
                    }, msg);
                    f.ShowDialog(this);
                  
                }


            #endregion

                private void label15_Click(object sender, EventArgs e)
                {

                }

                private void label17_Click(object sender, EventArgs e)
                {

                }

                private void label17_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
                {
                    ChoiceNumberForm choiceForm = new ChoiceNumberForm();
                    choiceForm.StartPosition = FormStartPosition.CenterParent;
                    choiceForm.ShowDialog();
                    userNumber = choiceForm.number;
                    label17.Text = txtUserNumber + userNumber;
                }

            
             
      



    }
      


}
