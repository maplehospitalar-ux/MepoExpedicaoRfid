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
    public partial class R1UhfConfigForm : BaseForm
    {

        public R1UhfConfigForm()
        {
            InitializeComponent();
        }
        public R1UhfConfigForm(bool isOpen)
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
                getPower(false);
                getRegion(false);
            }
            else
            {
                panel1.Enabled = false;
            }
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            R1MainForm.eventOpen += MainForm_eventOpen;
            R1MainForm.eventSwitchUI += MainForm_eventSwitchUI;
            MainForm_eventSwitchUI();
            if (R1MainForm.isOpen)
            {
                getPower(false);
                getRegion(false);
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
            getPower(true);
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

        private void getPower(bool flag)
        {
            string msg = Common.isEnglish ? "Get the power failure!" : "获取功率失败!";
            byte power = 0;
            if (uhf.GetPower(ref power))
            {
                cmbPower_ANT1.SelectedIndex = power - 1;
                msg = Common.isEnglish ? "Get the power success" : "获取功率成功!";
            }
            if (flag)
            {
                showMessage(msg);
            }
        }
        #endregion


        #region 区域
        private void btnRegionGet_Click(object sender, EventArgs e)
        {
            getRegion(true);
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
                    msg = Common.isEnglish ? "Set the region success" : "设置区域成功!";
                }

            }
            showMessage(msg);
        }

        private void getRegion(bool isFlag)
        {
            string msg = Common.isEnglish ? "Get the region failure!" : "获取区域失败!";
            byte region = 0;
            if (uhf.GetRegion(ref region))
            {
                switch (region)
                {
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
                msg = Common.isEnglish ? "Get the region success" : "获取区域成功!";
            }

            if (isFlag)
            {
                showMessage(msg);
            }
        }
        #endregion

        private void MainForm_eventSwitchUI()
        {
            if (Common.isEnglish)
            {
                groupBox6.Text = "Power";
                label24.Text = "Output Power:";
                groupBox11.Text = "Region";
                label1.Text = "Region:";
                cbRegionSave.Text = "Save";
            }
            else
            {
                groupBox6.Text = "功率";
                label24.Text = "输出功率:";
                groupBox11.Text = "区域";
                label1.Text = "区域:";
                cbRegionSave.Text = "保存";
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
 

    }
}
