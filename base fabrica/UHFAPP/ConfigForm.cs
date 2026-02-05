using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinForm_Test;
using System.Threading;

namespace UHFAPP
{
    public partial class ConfigForm : BaseForm
    {

        bool isFlag = false;
        public ConfigForm()
        {
            InitializeComponent();
        }
        public ConfigForm(bool isOpen)
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
            cmbAntWorkTime.SelectedIndex = 0;
        }

        void MainForm_eventOpen(bool open)
        {
            if (open)
            {
                panel1.Enabled = true;
                if (MainForm.MODE == 2)
                {
                    gbAnt.Enabled = false;
                    gbIP.Enabled = false;
                    gbIp2.Enabled = false;
                    bgGPIO.Enabled = false;
                    gbWorkMode.Enabled = false;
                    groupBox25.Enabled = false;
                    btnReset.Enabled = false;

                    gbIP.Visible = false;
                    gbIp2.Visible = false;
                    groupBox7.Visible = false;
                    gbWorkMode.Visible = false;
                    gbAnt.Visible = false;
                    bgGPIO.Visible = false;
                    btnReset.Visible = false;
                }
                else
                {
                    gbAnt.Enabled = true;
                    gbIP.Enabled = true;
                    gbIp2.Enabled = true;
                    bgGPIO.Enabled = true;
                    gbWorkMode.Enabled = true;
                    groupBox25.Enabled = true;
                    btnReset.Enabled = true;

                    gbIP.Visible = true;
                    gbIp2.Visible = true;
                    groupBox7.Visible = true;
                    gbWorkMode.Visible = true;
                    gbAnt.Visible = true;
                    bgGPIO.Visible = true;
                    btnReset.Visible = true;
                }
            }
            else
            {
                panel1.Enabled = false;
            }
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            MainForm.eventOpen += MainForm_eventOpen;
            cmbLinkFrequency.SelectedIndex = 3;

            MainForm.eventSwitchUI += MainForm_eventSwitchUI;
            MainForm_eventSwitchUI();
            comboBox1.SelectedIndex = 0;
            cmbOutStatus.SelectedIndex = 0;
            cmbInput.SelectedIndex = 0;
            comRM.SelectedIndex = 0;

            if (MainForm.MODE == 2)
            {
                gbAnt.Enabled = false;
                gbIP.Enabled = false;
                gbIp2.Enabled = false;
                bgGPIO.Enabled = false;
                gbWorkMode.Enabled = false;
                groupBox25.Enabled = false;
                btnReset.Enabled = false;

                gbIP.Visible = false;
                gbIp2.Visible = false;
                groupBox7.Visible = false;
                gbWorkMode.Visible = false;
                gbAnt.Visible = false;
                bgGPIO.Visible = false;
                btnReset.Visible = false;
            }
            else
            {
                gbAnt.Enabled = true;
                gbIP.Enabled = true;
                gbIp2.Enabled = true;
                bgGPIO.Enabled = true;
                gbWorkMode.Enabled = true;
                groupBox25.Enabled = true;
                btnReset.Enabled = true;

                gbIP.Visible = true;
                gbIp2.Visible = true;
                groupBox7.Visible = true;
                gbWorkMode.Visible = true;
                gbAnt.Visible = true;
                bgGPIO.Visible = true;
                btnReset.Visible = true;
            }
             
        }

        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.eventOpen -= MainForm_eventOpen;
            MainForm.eventSwitchUI -= MainForm_eventSwitchUI;
        }
        #region 功率
        private void btnPowerGet_Click(object sender, EventArgs e)
        {
            string msg = Common.isEnglish?"Get the power failure!":"获取功率失败!";
            byte power =0;
            if (uhf.GetPower(ref power))
            {
                cmbPower_ANT1.SelectedIndex = power - 1;
                msg = Common.isEnglish?"Get the power success":"获取功率成功!";
            }
            showMessage(msg);
        }

        private void btnPowerSet_Click(object sender, EventArgs e)
        {
            string msg = Common.isEnglish ? "Set the power failure!" : "设置功率失败!";
            if (cmbPower_ANT1.SelectedIndex >= 0)
            {
                byte power1 = (byte)(cmbPower_ANT1.SelectedIndex + 1);

                byte save = (byte)(cbPower.Checked?1:0);
                if (uhf.SetPower(save, power1))
                {
                    msg = Common.isEnglish ? "Set the power success!" : "设置功率成功!";
                }

            }
            showMessage(msg);
        } 

        #endregion

        #region 工作频率
//917.1
//917.3
//917.5
//917.7
//917.9
//918.1
//918.3
//918.5
//918.7
//918.9
//919.1
//919.3
//919.5
//919.7
//919.9
//920.1
//920.3
//920.5
//920.7
//920.9
//921.1
//921.3
//921.5
//921.7
//921.9
//922.1
//922.3
//922.5
//922.7
//922.9
//923.1
//923.3

        private void btnWorkModeSet_Click(object sender, EventArgs e)
        {
             string msg =Common.isEnglish? "failure!":"频点失败!";
             try
             {
                 if (comboBox1.Text != "")
                 {
                     string frequency = comboBox1.Text.Replace(".", "");
                     if (frequency.Length == 3) {
                         frequency = frequency + "000";
                     }
                     else if (frequency.Length == 4) {
                         frequency = frequency + "00";
                     }
                     else if (frequency.Length == 5)
                     {
                         frequency = frequency + "0";
                     }
                     if (StringUtils.IsNumber(frequency))
                     {
                         int[] ifrequency = new int[] { int.Parse(frequency) };
                         if (uhf.SetJumpFrequency(1, ifrequency))
                         {
                             msg = Common.isEnglish ? "success!" : "设置频点成功!";
                         }
                     }
                 }
             }
             catch (Exception ex) { 
             
             }

             showMessage(msg);
        }
        private void btnWorkModeGet_Click(object sender, EventArgs e)
        {
            string msg =Common.isEnglish? "failure!":"获取频点失败!";
            int[] ifrequency = new int[1];
            if (uhf.GetJumpFrequency(ref ifrequency))
            {
                comboBox1.Text = ifrequency[0].ToString().Insert(3, ".");
                msg = Common.isEnglish?"success!":"获取频点成功!";
            }

            showMessage(msg);
        }
        #endregion

        #region Gen2
        private void btnGen2Get_Click(object sender, EventArgs e)
        {
            byte Target = 0;
            byte Action = 0;
            byte T = 0;
            byte Q = 0;
            byte StartQ = 0;
            byte MinQ = 0;
            byte MaxQ = 0;
            byte D = 0;
            byte Coding = 0;
            byte P = 0;
            byte Sel = 0;
            byte Session = 0;
            byte G = 0;
            byte LF = 0;
            string msg = Common.isEnglish?"failure":"获取失败!";
            int start = Environment.TickCount;

            bool result=uhf.GetGen2(ref  Target, ref   Action, ref   T, ref   Q,
                     ref   StartQ, ref   MinQ,
                     ref   MaxQ, ref   D, ref   Coding, ref   P,
                     ref   Sel, ref   Session, ref   G, ref   LF);
          //  MessageBox.Show("耗时：" + (Environment.TickCount-start));
            if (result)
            {
                cmbTarget.SelectedIndex = Target;
                cmbAction.SelectedIndex = Action;
                cmbT.SelectedIndex = T;
                cmbQ.SelectedIndex = Q;
                cmbCoding.SelectedIndex = Coding;
                cmbP.SelectedIndex = P;
                cmbSel.SelectedIndex = Sel;
                cmbStartQ.SelectedIndex = StartQ;
                cmbMinQ.SelectedIndex = MinQ;
                cmbMaxQ.SelectedIndex = MaxQ;
                cmbDr.SelectedIndex = D;
                cmbSession.SelectedIndex = Session;
                cmbG.SelectedIndex = G;
                cmbLinkFrequency.SelectedIndex = LF;
                msg = Common.isEnglish?"success":"获取成功!";
                btnGen2Set.Enabled = true;
            }


            showMessage(msg);
        }
        private void btnGen2Set_Click(object sender, EventArgs e)
        {
            string msg =Common.isEnglish? "Set the Gen2 failure!":"设置失败!";
            try
            {
                byte Target =(byte) cmbTarget.SelectedIndex;
                byte Action = (byte)cmbAction.SelectedIndex;
                byte T = (byte)cmbT.SelectedIndex;
                byte Q = (byte)cmbQ.SelectedIndex;
                byte StartQ = (byte)cmbStartQ.SelectedIndex;
                byte MinQ = (byte)cmbMinQ.SelectedIndex;
                byte MaxQ = (byte)cmbMaxQ.SelectedIndex;
                byte D = (byte)cmbDr.SelectedIndex;
                byte Coding = (byte)cmbCoding.SelectedIndex;
                byte P = (byte)cmbP.SelectedIndex;
                byte Sel = (byte)cmbSel.SelectedIndex;
                byte Session = (byte)cmbSession.SelectedIndex;
                byte G = (byte)cmbG.SelectedIndex;
                byte LF = (byte)cmbLinkFrequency.SelectedIndex;
                if (uhf.SetGen2(Target, Action, T, Q, StartQ, MinQ, MaxQ, D, Coding, P, Sel, Session, G, LF))
                {
                    msg = Common.isEnglish?"Set the Gen2 success!":"设置成功!"; 
                }
                
            }
            catch (Exception ex)
            {
               
            }
            showMessage(msg);
        }
        #endregion

        #region CW
        private void btnGetCW_Click(object sender, EventArgs e)
        {
            string msg = "failure!";
            if (uhf.SetCW(1))
            {
                msg = "success!";
            }
            showMessage(msg);
        }
        private void btnSetCW_Click(object sender, EventArgs e)
        {
            string msg = "failure!";
            if (uhf.SetCW(0))
            {
                msg = "success!";
            }
            showMessage(msg);
        }
        #endregion

        #region 天线
        private void btnGetANT_Click(object sender, EventArgs e)
        {
             cmbAnt8.Checked = false;
             cmbAnt7.Checked = false;
             cmbAnt6.Checked = false;
             cmbAnt5.Checked = false;
             cmbAnt4.Checked = false;
             cmbAnt3.Checked = false;
             cmbAnt2.Checked = false;
             cmbAnt1.Checked = false;
             cmbAnt16.Checked = false;
             cmbAnt15.Checked = false;
             cmbAnt14.Checked = false;
             cmbAnt13.Checked = false;
             cmbAnt12.Checked = false;
             cmbAnt11.Checked = false;
             cmbAnt10.Checked = false;
             cmbAnt9.Checked = false;

            string msg = Common.isEnglish?"failure!":"获取天线失败!";
            byte[] ant = new byte[2];
            if (uhf.GetANT(ant))
            {
                foreach (Control control in this.gbAnt.Controls)
                {
                    if (control is CheckBox) {
                        CheckBox checkBox = (CheckBox)control;
                        checkBox.Checked = false;
                    }
                }
               // ant[0] = 00;
               // ant[1] = 03;

             //   MessageBox.Show("ant[0]=" + DataConvert.ByteArrayToHexString(new byte[] { ant[0] }) + "ant[1]="+DataConvert.ByteArrayToHexString(new byte[] { ant[1] }));

                string t1 = System.Convert.ToString(ant[0], 2);
                string t2  = System.Convert.ToString(ant[1], 2);

                string temp1 = "00000000".Substring(0, 8 - t1.Length) + t1;
                string temp2 = "00000000".Substring(0, 8 - t2.Length) + t2;

                if (temp2.Substring(0, 1) == "1") cmbAnt8.Checked = true;
                if (temp2.Substring(1, 1) == "1") cmbAnt7.Checked = true;
                if (temp2.Substring(2, 1) == "1") cmbAnt6.Checked = true;
                if (temp2.Substring(3, 1) == "1") cmbAnt5.Checked = true;
                if (temp2.Substring(4, 1) == "1") cmbAnt4.Checked = true;
                if (temp2.Substring(5, 1) == "1") cmbAnt3.Checked = true;
                if (temp2.Substring(6, 1) == "1") cmbAnt2.Checked = true;
                if (temp2.Substring(7, 1) == "1") cmbAnt1.Checked = true;

                if (temp1.Substring(0, 1) == "1") cmbAnt16.Checked = true;
                if (temp1.Substring(1, 1) == "1") cmbAnt15.Checked = true;
                if (temp1.Substring(2, 1) == "1") cmbAnt14.Checked = true;
                if (temp1.Substring(3, 1) == "1") cmbAnt13.Checked = true;
                if (temp1.Substring(4, 1) == "1") cmbAnt12.Checked = true;
                if (temp1.Substring(5, 1) == "1") cmbAnt11.Checked = true;
                if (temp1.Substring(6, 1) == "1") cmbAnt10.Checked = true;
                if (temp1.Substring(7, 1) == "1") cmbAnt9.Checked = true;

                msg = Common.isEnglish ? "success" : "获取天线成功!";
              //  msg = Common.isEnglish?"success":"获取天线成功!("+ DataConvert.ByteArrayToHexString(ant)+")";
            }

            showMessage(msg);
        }
        private void btnSetAnt_Click(object sender, EventArgs e)
        {
            string msg = Common.isEnglish?"failure":"设置天线失败!";
            StringBuilder sb1 = new StringBuilder();
            if (cmbAnt8.Checked) sb1.Append("1"); else sb1.Append("0");
            if (cmbAnt7.Checked) sb1.Append("1"); else sb1.Append("0");
            if (cmbAnt6.Checked) sb1.Append("1"); else sb1.Append("0");
            if (cmbAnt5.Checked) sb1.Append("1"); else sb1.Append("0");
            if (cmbAnt4.Checked) sb1.Append("1"); else sb1.Append("0");
            if (cmbAnt3.Checked) sb1.Append("1"); else sb1.Append("0");
            if (cmbAnt2.Checked) sb1.Append("1"); else sb1.Append("0");
            if (cmbAnt1.Checked) sb1.Append("1"); else sb1.Append("0");

            StringBuilder sb2 = new StringBuilder();
            if (cmbAnt16.Checked) sb2.Append("1"); else sb2.Append("0");
            if (cmbAnt15.Checked) sb2.Append("1"); else sb2.Append("0");
            if (cmbAnt14.Checked) sb2.Append("1"); else sb2.Append("0");
            if (cmbAnt13.Checked) sb2.Append("1"); else sb2.Append("0");
            if (cmbAnt12.Checked) sb2.Append("1"); else sb2.Append("0");
            if (cmbAnt11.Checked) sb2.Append("1"); else sb2.Append("0");
            if (cmbAnt10.Checked) sb2.Append("1"); else sb2.Append("0");
            if (cmbAnt9.Checked) sb2.Append("1"); else sb2.Append("0");

            byte[] ant = new[]{
                   Convert.ToByte(sb2.ToString(),2),
                   Convert.ToByte(sb1.ToString(),2)
            };
            
            byte flag = cbAntSet.Checked ? (byte)1 : (byte)0;
            if (uhf.SetANT(flag, ant))
            {
                msg = Common.isEnglish ? "success" : "设置天线成功!"; ;
               // msg = Common.isEnglish ? "success" : "设置天线成功!(" + DataConvert.ByteArrayToHexString(ant) + ")"; ;
            }
            showMessage(msg);

        }


        private void btnGetANTWorkTime_Click(object sender, EventArgs e)
        {
            string msg = "failure";
            int ant = cmbAntWorkTime.SelectedIndex + 1;
            int time = 0;
            if (uhf.GetANTWorkTime((byte)ant, ref time))
            {
                txtAntWorkTime.Text = time.ToString();
                msg = "success";
            }

            showMessage(msg);
        }

        private void btnSetANTWorkTime_Click(object sender, EventArgs e)
        {
            string msg = "failure";
            int ant = cmbAntWorkTime.SelectedIndex+1;
            int time = int.Parse(txtAntWorkTime.Text);
            int flag = cbAntWorkTime.Checked ? 1 : 0;
            if (uhf.SetANTWorkTime((byte)ant, (byte)flag, time))
            {
                msg = "success";
            }
            showMessage(msg);
        }
        private void txtWorkTime_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtAntWorkTime.Text = txtAntWorkTime.Text.Trim();
                string workTime = txtAntWorkTime.Text;
                if (!StringUtils.IsNumber(workTime))
                {
                    txtAntWorkTime.Text = "";
                    return;
                }
                int time = int.Parse(workTime);
                if (time > 65535) {
                    txtAntWorkTime.Text = "65535";
                }
            }
            catch (Exception ex)
            {
                txtAntWorkTime.Text = "";
            }
        }
        private void txtWorkTime_LostFocus(object sender, EventArgs e)
        {
            try
            {
                txtAntWorkTime.Text = txtAntWorkTime.Text.Trim();
                string workTime = txtAntWorkTime.Text;
                if (!StringUtils.IsNumber(workTime))
                {
                    txtAntWorkTime.Text = "";
                    return;
                }
                int time = int.Parse(workTime);
                if (time <10)
                {
                    txtAntWorkTime.Text = "10";
                }
            }
            catch (Exception ex)
            {
                txtAntWorkTime.Text = "";
            }
        }
        
        
        #endregion

        #region 区域
        private void btnRegionGet_Click(object sender, EventArgs e)
        {
            //0x01(China1),0x02(China2),0x04(Europe),0x08(USA),0x16(Korea),0x32(Japan)
            string msg = Common.isEnglish?"Get the region failure!":"获取区域失败!";
            byte region=0;
            if (uhf.GetRegion(ref region))
            {
                switch (region) { 
                    case 0x01:
                        cmbRegion.SelectedIndex = 0;
                        break;
                    case 0x02:
                        cmbRegion.SelectedIndex = 1;
                        break;
                    case 0x04:
                        cmbRegion.SelectedIndex = 2;
                        break;
                    case 0x08:
                        cmbRegion.SelectedIndex = 3;
                        break;
                    case 0x16:
                        cmbRegion.SelectedIndex = 4;
                        break;
                    case 0x32:
                        cmbRegion.SelectedIndex = 5;
                        break;
                    case 0x34:
                        cmbRegion.SelectedIndex = 6;
                        break;
                    case 0x33:
                        cmbRegion.SelectedIndex = 7;
                        break;
                    case 0x36:
                        cmbRegion.SelectedIndex = 8;
                        break;
                    case 0x37:
                        cmbRegion.SelectedIndex = 9;
                        break;
                }
                msg=Common.isEnglish?"Get the region success":"获取区域成功!";
            }

            showMessage(msg);
        }

        private void btnRegionSet_Click(object sender, EventArgs e)
        {
            //0x01(China1),0x02(China2),0x04(Europe),0x08(USA),0x16(Korea),0x32(Japan)
            int flag = cbRegionSave.Checked ? 1 : 0;
            int region = -1;
            switch (cmbRegion.SelectedIndex)
            {
                case 0:
                    region = 0x01;
                    break;
                case 1:
                    region = 0x02;
                    break;
                case 2:
                    region = 0x04;
                    break;
                case 3:
                    region = 0x08;
                    break;
                case 4:
                    region = 0x16;
                    break;
                case 5:
                    region = 0x32;
                    break;
                case 6:
                    region = 0x34;
                    break;
                case 7:
                    region = 0x33;
                    break;
                case 8:
                    region = 0x36;
                    break;
                case 9:
                    region = 0x37;
                    break;
                    
            }
            string msg = Common.isEnglish ? "Set the region failure!" : "设置区域失败!";
            if (region >= 0)
            {
                if (uhf.SetRegion((byte)flag, (byte)region))
                {
                    msg=Common.isEnglish? "Set the region success":"设置区域成功!";
                }
                 
            }
            showMessage(msg);
        }
        #endregion
        #region 温度保护
        private void GetTemperatureProtect_Click(object sender, EventArgs e)
        {
            byte flag = 0;
            string msg = Common.isEnglish ? "failure!" : "失败!";
            if (uhf.GetTemperatureProtect(ref flag))
            {
                if (flag == 1)
                {
                    rbEnable.Checked = true;
                    rbDisable.Checked = false;
                      msg = Common.isEnglish ? "success!" : "成功!";
                }
                else if (flag ==0)
                {
                    rbEnable.Checked = false;
                    rbDisable.Checked = true;
                      msg = Common.isEnglish ? "success!" : "成功!";
                }

            }
            showMessage(msg);
        }
        private void btnSetTemperatureProtect_Click(object sender, EventArgs e)
        {
            string msg = Common.isEnglish ? "failure!" : "失败!";
            int flag = 0;
            if (rbDisable.Checked)
            {
                flag = 0;
            }
            else if (rbEnable.Checked)
            {
                flag = 1;
            }
            if (uhf.SetTemperatureProtect((byte)flag))
            {
                msg = Common.isEnglish ? "success!" : "成功!";
            }
            showMessage(msg);
        }
        #endregion

        #region 链路组合
        private void btnRFLinkGet_Click(object sender, EventArgs e)
        {
            string msg = "failure";
            byte mode = 0;
            if (uhf.GetRFLink(ref mode))
            {
                cmbRFLink.SelectedIndex = mode;
                msg = "success";
            }

            showMessage(msg);
        }
        private void btnRFLinkSet_Click(object sender, EventArgs e)
        {
            string msg = "failure";
            int flag = cbRFLink.Checked ? 1 : 0;
            if (cmbRFLink.SelectedIndex >= 0)
            {
                if (uhf.SetRFLink((byte)flag, (byte)cmbRFLink.SelectedIndex)) {
                    msg = "success";
                }
            }

            showMessage(msg);
        }

        #endregion

        #region FastID
        private void btnFastIDGet_Click(object sender, EventArgs e)
        {
            byte flag = 0;
            string msg = "failure";
            if (uhf.GetFastID(ref flag))
            {
                if (flag == 0)
                {
                    rbFastIDEnable.Checked = false;
                    rbFastIDDisable.Checked = true;
                    msg = "success";
                }
                else if (flag == 1)
                {
                    rbFastIDEnable.Checked = true;
                    rbFastIDDisable.Checked = false;
                    msg = "success";
                }
            }
            showMessage(msg);
        }
        private void btnFastIDSet_Click(object sender, EventArgs e)
        {
            int flag = -1;
            string msg = "failure";
            if (rbFastIDEnable.Checked)
            {
                flag = 1;
            }
            else if (rbFastIDDisable.Checked)
            {
                flag = 0;
            }

            if (flag >= 0)
            {
                if (uhf.SetFastID((byte)flag))
                {
                    msg = "success";

                    if (flag == 1) {
                        if (uhf.SetTagfocus(0))
                        {
                            rbTagfocusDisable.Checked = true;
                        }
                        if (uhf.setEPCMode(false))
                        {
                            cbInventoryMode.SelectedIndex = 0;
                        }
                    }

                }
               
            }

            showMessage(msg);
        }
        #endregion

        #region Tagfocus
        private void btnrbTagfocusGet_Click(object sender, EventArgs e)
        {
            string msg = "failure";
            byte flag = 0;
            if (uhf.GetTagfocus(ref flag))
            {
                if (flag == 0)
                {
                    rbTagfocusEnable.Checked = false;
                    rbTagfocusDisable.Checked = true;
                    msg = "success";
                }
                else if (flag == 1)
                {
                    rbTagfocusEnable.Checked = true;
                    rbTagfocusDisable.Checked = false;
                    msg = "success";
                }
            }

            showMessage(msg);
        }
        private void btnrbTagfocusSet_Click(object sender, EventArgs e)
        {
            int flag = -1;
            string msg = "failure";
            if (rbTagfocusEnable.Checked)
            {
                flag = 1;
            }
            else if (rbTagfocusDisable.Checked)
            {
                flag = 0;
            }

            if (flag >= 0)
            {
                if (uhf.SetTagfocus((byte)flag))
                {
                    msg = "success";
                    if (flag == 1)
                    {

                        if (uhf.SetFastID(0))
                        {
                            rbFastIDDisable.Checked = true;
                        }
                 
                        if (uhf.setEPCMode(false))
                        {
                            cbInventoryMode.SelectedIndex = 0;
                        }
                    }
                }
                
            }
            showMessage(msg);
        }
        #endregion
        #region 设置软复位
        private void btnReset_Click(object sender, EventArgs e)
        {
            string msg = Common.isEnglish ? "failure" : "设置软复位失败!";
            if (uhf.SetSoftReset())
            {
                msg = Common.isEnglish ? "success" : "设置软复位成功!";
            }

            showMessage(msg);
        }
        #endregion
        #region DualSingel
        private void btnDualSingelGet_Click(object sender, EventArgs e)
        {
            byte flag = 0;
            string msg = "failure";
            if (uhf.GetDualSingelMode(ref flag))
            {
                if (flag == 0)
                {
                    rbDual.Checked = true;
                    rbSingel.Checked = false;
                    msg = "success";
                }
                else if (flag == 1)
                {
                    rbDual.Checked = false;
                    rbSingel.Checked = true ;
                    msg = "success";
                }
            }
            showMessage(msg);
        }
        private void btnDualSingelSet_Click(object sender, EventArgs e)
        {
            int flag = -1;

            if (rbDual.Checked)
            {
                flag = 0;
            }
            else if (rbSingel.Checked)
            {
                flag =1 ;
            }
            string msg = "failure";
            if (flag >= 0)
            {
                int save = cbDualSingelSave.Checked ? 1 : 0;
                if (uhf.SetDualSingelMode((byte)save, (byte)flag))
                {
                    msg = "success";
                }
            }
            showMessage(msg);
        }
        #endregion

 

        #region 协议
        private void button2_Click_1(object sender, EventArgs e)
        { 
            string msg = Common.isEnglish ? "failure" : "获取失败!";
            int type = uhf.GetProtocol();
            if (type>-1)
            {
                msg = Common.isEnglish ? "success" : "获取成功!";
                cmbProtocol.SelectedIndex = type;
            }
            showMessage(msg);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string msg = Common.isEnglish ? "failure" : "设置失败!";
            int type = cmbProtocol.SelectedIndex;
            if (type >= 0)
            {
                if (uhf.SetProtocol((byte)type))
                {
                    msg = Common.isEnglish ? "success" : "设置成功!";
                }
            }
            showMessage(msg);
        }


        #endregion  

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string data = textBox1.Text.Trim().Replace(" ", "");
            if (data.Length > 0) {

                
                int index=textBox1.SelectionStart-1;
                if (index >= 0)
                {
                    string charData = data.Substring(index, 1);
                    if (charData != "0" && charData != "1" && charData != "2" 
                        && charData != "3" && charData != "4" && charData != "5" && charData != "6" && charData != "7"
                        && charData != "8" && charData != "9" && charData != ".")
                    {
                        textBox1.Text = textBox1.Text.Remove(index, 1);
                        textBox1.SelectionStart = textBox1.Text.Length;
                    }
                }
            }
   
        }



        private void MainForm_eventSwitchUI()
        {
           // groupBox25.Location = new Point(903, 743);
           // groupBox25.Visible = false;
            if (Common.isEnglish)
            {
                int index2 = comboBox2.SelectedIndex;
                comboBox2.Items.Clear();
                comboBox2.Items.Add("low voltage");
                comboBox2.Items.Add("high voltage");
                if (index2 >= 0)
                    comboBox2.SelectedIndex = index2;


                int index3 = comboBox3.SelectedIndex;
                comboBox3.Items.Clear();
                comboBox3.Items.Add("low voltage");
                comboBox3.Items.Add("high voltage");
                if (index2 >= 0)
                    comboBox3.SelectedIndex = index3;


                int index1 = cmbOutStatus.SelectedIndex;
                cmbOutStatus.Items.Clear();
                cmbOutStatus.Items.Add("Disconnect");
                cmbOutStatus.Items.Add("close");
                if (index1>=0)
                cmbOutStatus.SelectedIndex = index1;

                groupBox19.Text = "Protocol";
                label35.Text = "Protocol:";

                label2.Text = "";
                groupBox6.Text = "Power";
                label24.Text = "Output Power:";
               // cbSave.Text = "Save";
             

                groupBox11.Text = "Region";
                label1.Text = "Region:";           
                cbRegionSave.Text = "Save";

                label5.Text = "RFLink:";
                groupBox3.Text = "RFLink";
                cbRFLink.Text = "cbSave";

                groupBox7.Text = "Rrequency";
                label28.Text = "Rrequency:";

                btnReset.Text = "Reset";

                gbAnt.Text = "ANT";
                groupBox8.Text = "TemperatureProtect";
                 label24.Location = new Point(8, 34);
               // label1.Location = new Point(51, 39);
              //  label5.Location = new Point(51, 33);
              //  label28.Location = new Point(27, 42);

                rbFastIDEnable.Text = rbTagfocusEnable.Text = rbEnable.Text =   "Enable";
                rbFastIDDisable.Text = rbTagfocusDisable.Text = rbDisable.Text =  "Disable";

                gbIP.Text = "Local IP";
                gbIp2.Text = "Destination IP";
                label9.Text = label31.Text = "IP:";
                label25.Text = label30.Text = "Port:";
                groupBox9.Text = "Buzzer=";

                int index = workMode.SelectedIndex == -1 ? 0 : workMode.SelectedIndex;
                workMode.Items.Clear();
                workMode.Items.Add("command mode");
                workMode.Items.Add("auto mode");
                workMode.Items.Add("trigger mode");

                groupBox1.Text = "cw";
                
                workMode.SelectedIndex = index;

      
                label39.Text = "input1:";
                label40.Text = "input2:";
                label38.Text = "Relay:";

                gbInventoryMode.Text = "Inventory Mode";
                label45.Text = "Mode:";
                label46.Text = "User Ptr:";
                label47.Text = "User Len:";

                label53.Text = "Subnet mask:";
                label54.Text = "Gateway:";
            }
            else
            {

                label53.Text = "   子网掩码:";
                label54.Text = "   网关:";

                int index1 = cmbOutStatus.SelectedIndex;
                cmbOutStatus.Items.Clear();
                cmbOutStatus.Items.Add("断开");
                cmbOutStatus.Items.Add("闭合");
                if (index1 >= 0)
                cmbOutStatus.SelectedIndex = index1;


                groupBox19.Text = "协议";
                label35.Text = "协议";

                label2.Text = "设置Gen2之前先获取";
                groupBox6.Text = "功率";
                 label24.Text = "输出功率:";
                //cbSave.Text = "保存";

                 
                groupBox11.Text = "区域";
                label1.Text = "区域:";
                cbRegionSave.Text = "保存";

                label5.Text = "链路组合:";
                groupBox3.Text = "链路";
                cbRFLink.Text = "保存";

                groupBox7.Text = "定频";
                label28.Text = "频点:";

                btnReset.Text = "软件复位";

                gbAnt.Text = "天线";
                groupBox8.Text = "温度保护";

                groupBox1.Text = "连续波";

           
                label39.Text = "输入1:";
                label40.Text = "输入2:";
                label38.Text = "继电器:";

                label24.Location = new Point(30, 34);
          //      label1.Location = new Point(63, 39);
          //      label5.Location = new Point(40, 33);
         //       label28.Location = new Point(55, 42);

                rbFastIDEnable.Text = rbTagfocusEnable.Text = rbEnable.Text   = "启用";
                rbFastIDDisable.Text = rbTagfocusDisable.Text = rbDisable.Text   = "禁用";

                gbIP.Text = "本地IP";
                gbIp2.Text = "目标IP";
              //  label9.Text = label31.Text = "IP地址:";
              //  label25.Text = label30.Text = "端口号:";
                groupBox9.Text = "蜂鸣器";


                int index = workMode.SelectedIndex == -1 ? 0 : workMode.SelectedIndex;
                workMode.Items.Clear();
                workMode.Items.Add("命令工作模式");
                workMode.Items.Add("自动工作模式");
                workMode.Items.Add("触发模式");
                workMode.SelectedIndex = index;

                gbInventoryMode.Text = "盘点模式";
                label45.Text = "模式        :";
                label46.Text = "User起始地址:";
                label47.Text = "User长度    :";

            }

        }

        private void showMessage(string msg,int time) {
            if (msg.Contains("失败") || msg.ToLower().Contains("fail"))
            {
                frmWaitingBox f = new frmWaitingBox((obj, args) =>
                {
                    System.Threading.Thread.Sleep(time);
                }, msg);
                f.ShowDialog(this);
            }
        }
        private void showMessage(string msg)
        {
            if (msg.Contains("失败") || msg.ToLower().Contains("fail"))
            {
                frmWaitingBox f = new frmWaitingBox((obj, args) =>
                {
                    System.Threading.Thread.Sleep(500);
                }, msg);
                f.ShowDialog(this);
            }
        }

        
        //设置本地IP
        private void btnSetIPLocal_Click(object sender, EventArgs e)
        {
            string port = txtLocalPort.Text.Trim();

            StringBuilder ip = new StringBuilder();
            ip.Append(ipControlLocal.IpData[0]);
            ip.Append(".");
            ip.Append(ipControlLocal.IpData[1]);
            ip.Append(".");
            ip.Append(ipControlLocal.IpData[2]);
            ip.Append(".");
            ip.Append(ipControlLocal.IpData[3]);


            StringBuilder sbMask = new StringBuilder();
            sbMask.Append(ipControlSubnetMask.IpData[0]);
            sbMask.Append(".");
            sbMask.Append(ipControlSubnetMask.IpData[1]);
            sbMask.Append(".");
            sbMask.Append(ipControlSubnetMask.IpData[2]);
            sbMask.Append(".");
            sbMask.Append(ipControlSubnetMask.IpData[3]);


            StringBuilder gate = new StringBuilder();
            gate.Append(ipGateway.IpData[0]);
            gate.Append(".");
            gate.Append(ipGateway.IpData[1]);
            gate.Append(".");
            gate.Append(ipGateway.IpData[2]);
            gate.Append(".");
            gate.Append(ipGateway.IpData[3]);

            if (!StringUtils.isIP(ip.ToString()) || !StringUtils.isIP(sbMask.ToString()) || !StringUtils.isIP(gate.ToString()))
            {
                string msg = Common.isEnglish ? "failure!" : "设置IP失败!";
                showMessage(msg);
                return;
            }
            if (!StringUtils.IsNumber(port))
            {
                string msg = Common.isEnglish ? "failure!" : "设置IP失败!";
                showMessage(msg);
                return;
            }

            if (ipControlLocal.IpData[0] == "255")
            {
                string msg = Common.isEnglish ? "failure!" : "IP地址不能255开头,设置IP失败!";
                showMessage(msg);
                return;
            }
            if (ipControlSubnetMask.IpData[0] != "255")
            {
                string msg = Common.isEnglish ? "failure!" : "子网掩码必须255开头,设置IP失败!";
                showMessage(msg);
                return;
            }
            if (ipGateway.IpData[0] == "255")
            {
                string msg = Common.isEnglish ? "failure!" : "网关不能255开头,设置IP失败!";
                showMessage(msg);
                return;
            }
            if (!uhf.SetLocalIP(ip.ToString(), int.Parse(port), sbMask.ToString(), gate.ToString()))
            {
                string msg = Common.isEnglish ? "failure!" : "设置IP失败!";
                showMessage(msg);
                return;
            }
        }
        //获取本地IP
        private void btnGetIPLocal_Click(object sender, EventArgs e)
        {
            int startTime = Environment.TickCount;
            StringBuilder sIP=new StringBuilder(20);
            StringBuilder sPort = new StringBuilder(20);
            StringBuilder mask = new StringBuilder(20);
            StringBuilder gate = new StringBuilder(20);
            if (uhf.GetLocalIP(sIP, sPort, mask, gate))
            {
                ipControlLocal.IpData = sIP.ToString().Split('.');// txtLocalIP.Text = sIP.ToString();
                txtLocalPort.Text = sPort.ToString();
                ipGateway.IpData = gate.ToString().Split('.');
                ipControlSubnetMask.IpData = mask.ToString().Split('.');
            }
            else
            {
                string msg = Common.isEnglish ? "failure!" : "获取IP失败!";
                showMessage(msg);
                return;
            }
 
        }
 
        //获取目标IP
        private void btnGetIpDest_Click(object sender, EventArgs e)
        {
            StringBuilder sIP = new StringBuilder();
            StringBuilder sPort = new StringBuilder();
            if (uhf.GetDestIP(sIP, sPort))
            {
               // txtIPDest.Text = sIP.ToString();
                ipControlDest.IpData = sIP.ToString().Split('.');
                txtPortDest.Text = sPort.ToString();
            }
            else
            {
                string msg = Common.isEnglish ? "failure!" : "获取IP失败!";
                showMessage(msg);
                return;
            }
        }

        //设置目标IP
        private void btnSetIpDest_Click(object sender, EventArgs e)
        {
            string port = txtPortDest.Text.Trim();

            string[] tempIp = ipControlDest.IpData;
            StringBuilder sb = new StringBuilder();
            sb.Append(tempIp[0]);
            sb.Append(".");
            sb.Append(tempIp[1]);
            sb.Append(".");
            sb.Append(tempIp[2]);
            sb.Append(".");
            sb.Append(tempIp[3]);
            string ip = sb.ToString();
  
            if (!StringUtils.isIP(ip))
            {
                string msg = Common.isEnglish ? "failure!" : "设置IP失败!";
                showMessage(msg);
                return;
            }
            if (!StringUtils.IsNumber(port))
            {
                string msg = Common.isEnglish ? "failure!" : "设置IP失败!";
                showMessage(msg);
                return;
            }
            if (ipControlDest.IpData[0] == "255")
            {
                string msg = Common.isEnglish ? "failure!" : "IP地址不能255开头，设置IP失败!";
                showMessage(msg);
                return;
            }

            if (!uhf.SetDestIP(ip, int.Parse(port)))
            {
                string msg = Common.isEnglish ? "failure!" : "设置IP失败!";
                showMessage(msg);
                return;
            }
        }

        //获取蜂鸣器
        private void btnGetBuzzer_Click(object sender, EventArgs e)
        {
            byte[] mode=new byte[10];
            if (!uhf.UHFGetBuzzer(mode))
            {
                string msg = Common.isEnglish ? "failure!" : "获取失败!";
                showMessage(msg);
                return;
            }
            else
            {
                if (mode[0] == 0)
                {

                    rbEnableBuzzer.Checked = false;
                    rbDisableBuzzer.Checked = true;
                }
                else if (mode[0] == 1)
                {
                    rbDisableBuzzer.Checked = false;
                    rbEnableBuzzer.Checked = true;
                }

            }
        }
        //设置蜂鸣器
        private void btnSetBuzzer_Click(object sender, EventArgs e)
        {
            //0x00表示关闭蜂鸣器；0x01表示打开蜂鸣器
            byte mode =0;
            if (rbEnableBuzzer.Checked)
            {
                mode = 1;
            }
            else if (rbDisableBuzzer.Checked)
            {
                mode = 0;
            }
            else {

                string msg = Common.isEnglish ? "failure!" : "设置失败!";
                showMessage(msg);
                return;
            }

            if (!uhf.UHFSetBuzzer(mode))
            {
                string msg = Common.isEnglish ? "failure!" : "设置失败!";
                showMessage(msg);
                return;
            }
        }

        #region 工作模式
        private void button2_Click(object sender, EventArgs e)
        {
            //get
            byte[] mode=new byte[2];
            if (uhf.GetWorkMode(mode))
            {
                if (mode[0] == 0)
                {
                    workMode.SelectedIndex = 0;
                }
                else if (mode[0] == 1)
                {
                    workMode.SelectedIndex = 1;
                }
                else if (mode[0] == 2)
                {
                    workMode.SelectedIndex = 2;
                }
            }
            else
            {
                string msg = Common.isEnglish ? "failure!" : "获取失败!";
                showMessage(msg);
                return;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte mode=(byte) workMode.SelectedIndex;
            if (!uhf.SetWorkMode(mode))
            {
                string msg = Common.isEnglish ? "failure!" : "设置失败!";
                showMessage(msg);
                return;
             
            }
  
        }

        private void btnWorkModeParaSet_Click(object sender, EventArgs e)
        {
            UHFAPI uhfAPI = uhf as UHFAPI;
            if (uhfAPI != null)
            {
                if (txtWT.Text.Trim().Length == 0)
                {
                    string msg = Common.isEnglish ? "failure!" : "设置失败!";
                    showMessage(msg);
                }
                if (txtIT.Text.Trim().Length == 0)
                {
                    string msg = Common.isEnglish ? "failure!" : "设置失败!";
                    showMessage(msg);
                }

                int input = cmbInput.SelectedIndex;
                int workTime = int.Parse(txtWT.Text);
                int waitTime = int.Parse(txtIT.Text);
                int receiveMode = comRM.SelectedIndex;
                if (!uhfAPI.SetWorkModePara((byte)input, workTime, waitTime, (byte)receiveMode))
                {
                    string msg = Common.isEnglish ? "failure!" : "设置失败!";
                    showMessage(msg);
                }
            }

        }

        private void btnWorkModeParaGet_Click(object sender, EventArgs e)
        {
            UHFAPI uhfAPI = uhf as UHFAPI;
            if (uhfAPI != null)
            {
                 byte ioControl=0;
                 int workTime=100;
                 int intervalTime=0;
                 byte mode=0;
                if (uhfAPI.GetWorkModePara(ref  ioControl, ref  workTime, ref intervalTime, ref mode))
                {
                    cmbInput.SelectedIndex = ioControl;
                    txtWT.Text = workTime.ToString();
                    txtIT.Text = intervalTime.ToString();
                    comRM.SelectedIndex = mode;

                }
                else
                {
                    string msg = Common.isEnglish ? "failure!" : "失败!";
                    showMessage(msg);
                }            
            }
        }

        #endregion
 

        private void button4_Click(object sender, EventArgs e)
        {

            int temp = uhf.GetTemperatureVal();
            textBox3.Text = temp+"";
            if (temp == -1) {
                string msg = Common.isEnglish ? "failure!" : "获取失败!";
                showMessage(msg);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string temp = textBox3.Text;
                if (temp == null || temp.Length == 0)
                {
                    string msg = Common.isEnglish ? "failure!" : "设置失败!";
                    showMessage(msg);
                    return;
                }
                int t = int.Parse(temp);

                if (t < 50 || t > 75)
                {
                    string msg = Common.isEnglish ? "failure!" : "设置失败!";
                    showMessage(msg);
                    return;
                }


                if (!uhf.SetTemperatureVal((byte)t))
                {
                    string msg = Common.isEnglish ? "failure!" : "设置失败!";
                    showMessage(msg);
                    return;
                }
            }
            catch (Exception ex)
            {
                string msg = Common.isEnglish ? "failure!" : "设置失败!";
                showMessage(msg);
                return;
            }
        }

        #region GPIO
        private void button6_Click(object sender, EventArgs e)
        {
            byte[] data = new byte[2];
            if (!uhf.getIOControl(data))
            {
                string msg = Common.isEnglish ? "failure!" : "获取失败!";
                showMessage(msg);
                return;
            }
            else {
                comboBox2.SelectedIndex = data[0];
                comboBox3.SelectedIndex = data[1];
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int status = cmbOutStatus.SelectedIndex;
            int ouput2 = 1;// cmbOutPut2.SelectedIndex;
            int ouput1 = 1;// cmbOutPut1.SelectedIndex;



            if (!uhf.setIOControl((byte)ouput1, (byte)ouput2, (byte)status))
            {
                string msg = Common.isEnglish ? "failure!" : "设置失败!";
                showMessage(msg);
                return;
            }
        }
        #endregion

        #region 占空比
        private void button9_Click(object sender, EventArgs e)
        {
            int workTime;
            int waitTime;

            if (uhf.getWorkAndWaitTime(out workTime, out waitTime))
            {
                txtWaitTime.Text = waitTime+"";
                txtworkTime.Text = workTime + "";
                string msg = Common.isEnglish ? "failure!" : "获取成功!";
                showMessage(msg);
                return;
            }else{
                string msg = Common.isEnglish ? "failure!" : "获取失败!";
                showMessage(msg);
                return;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //设置
            string waitTime = txtWaitTime.Text;
            string workTime = txtworkTime.Text;
            if (waitTime == "" || workTime == "")
            {
                string msg = Common.isEnglish ? "failure!" : "设置失败!";
                showMessage(msg);
                return;
            }

            try
            {

                int iwaitTime = int.Parse(waitTime);
                int iworkTime = int.Parse(workTime);
                if (uhf.setWorkAndWaitTime(iworkTime, iwaitTime, checkBox1.Checked))
                {

                    string msg = Common.isEnglish ? "failure!" : "设置成功!";
                    showMessage(msg);
                }
                else
                {
                    string msg = Common.isEnglish ? "failure!" : "设置失败!";
                    showMessage(msg);
                }
 
            }
            catch (Exception ex)
            {
                string msg = Common.isEnglish ? "failure!" : "设置失败!";
                showMessage(msg);
            }


        }
        #endregion

        #region  盘点模式设置

        private void button11_Click(object sender, EventArgs e)
        {
            byte userPtr = 0;
            byte userLen = 0;
            int mode=uhf.getEPCTIDUSERMode(ref userPtr,ref userLen);
            switch (mode)
            {
                case 0:
                    cbInventoryMode.SelectedIndex = 0;
                    break;
                case 1:
                    cbInventoryMode.SelectedIndex = 1;
                    break;
                case 2:
                    cbInventoryMode.SelectedIndex = 2;
                    txtUserLen.Text = userLen+"";
                    txtUserPtr.Text = userPtr + "";
                    break;
                default:
                    cbInventoryMode.SelectedIndex = -1;
                    break;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int mode = cbInventoryMode.SelectedIndex;
            bool result = false;
            bool isSave=checkBox2.Checked;
            switch (mode)
            {
                case 0:
                    result=uhf.setEPCMode(isSave);
                    break;
                case 1:
                    result=uhf.setEPCAndTIDMode(isSave);
                    break;
                case 2:
                    int userPtr = int.Parse(txtUserPtr.Text);
                    int userLen = int.Parse(txtUserLen.Text);
                    result = uhf.setEPCAndTIDUSERMode(isSave,(byte)userPtr, (byte)userLen);
                    break;
            }

            if (!result)
            {
                string msg = Common.isEnglish ? "failure!" : "设置失败!";
                showMessage(msg);
                return;
            }
        }

        private void cbInventoryMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbInventoryMode.SelectedIndex == 2)
            {
                txtUserLen.Visible = true;
                txtUserPtr.Visible = true;
                label46.Visible = true;
                label47.Visible = true;
            }
            else
            {
                txtUserLen.Visible = false;
                txtUserPtr.Visible = false;
                label46.Visible = false;
                label47.Visible = false;
            }
        }
        #endregion

    

        private void workMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (workMode.SelectedIndex == 2)
            {
                plWorkModePara.Visible = true;
                btnWorkModeParaGet_Click(null,null);
            }
            else
            {
                plWorkModePara.Visible = false;

            }
        }

        private void btnAntennaConnectionState_Click(object sender, EventArgs e)
        {
            cbANT1_state.Checked = false;
            cbANT2_state.Checked = false;
            cbANT3_state.Checked = false;
            cbANT4_state.Checked = false;
            cbANT5_state.Checked = false;
            cbANT6_state.Checked = false;
            cbANT7_state.Checked = false;
            cbANT8_state.Checked = false;

            string msg = "failure!";
            short[] antstate = new short[1];
            if (uhf.GetANTLinkStatus(antstate))
            {
                short antS = antstate[0];
                if (((antS >> 7) & 1) == 1) cbANT8_state.Checked = true;
                if (((antS >> 6) & 1) == 1) cbANT7_state.Checked = true;
                if (((antS >> 5) & 1) == 1) cbANT6_state.Checked = true;
                if (((antS >> 4) & 1) == 1) cbANT5_state.Checked = true;
                if (((antS >> 3) & 1) == 1) cbANT4_state.Checked = true;
                if (((antS >> 2) & 1) == 1) cbANT3_state.Checked = true;
                if (((antS >> 1) & 1) == 1) cbANT2_state.Checked = true;
                if ((antS & 1) == 1) cbANT1_state.Checked = true;
                msg = "success";
            }
            showMessage(msg);
        }

        private void btnCalibration_Click(object sender, EventArgs e)
        {
            int result = uhf.CalibrationVoltage();
            txtCalibration.Text = result + "";
           
        }
 



    }
}
