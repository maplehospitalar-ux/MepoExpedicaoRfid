using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinForm_Test;
using System.Net;
using System.Threading;
using UHFAPP.r1;

namespace UHFAPP
{
    public partial class R1MainForm : BaseForm
    {
  

        public delegate void DelegateOpen(bool open);
        public static event DelegateOpen eventOpen = null;

        public delegate void DelegateSwitchUI();
        public static event DelegateSwitchUI eventSwitchUI = null;

        string strOpen= "  Open  ";
        string strClose = "  Close  ";

        private int currentType = 0;
        public static bool isOpen = false;
        public R1MainForm mainform = null;
        public R1MainForm()
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IsMdiContainer = true;
            mainform = this;
            toolStripComboBox1.Items.Add("中文简体");
            toolStripComboBox1.Items.Add("English");
            toolStripComboBox1.SelectedIndex = 0;
            toolStripOpen.Text = "  Open  ";
            SwitchShowUI();
 
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
           configToolStripMenuItem_Click(null,null);
           disableControls();
           combCommunicationMode.SelectedIndex = 0;
        }
        public void enableControls()
        {
            configToolStripMenuItem.Enabled = true;
            uHFVersionToolStripMenuItem.Enabled = true;
            toolStripMenuItem1.Enabled = true;
            uHFUpgradeToolStripMenuItem.Enabled = true;
            SetR3ToolStripMenuItem.Enabled = true;
        }
        public void disableControls()
        {
            configToolStripMenuItem.Enabled = false;
            uHFVersionToolStripMenuItem.Enabled = false;
            toolStripMenuItem1.Enabled = false;
            uHFUpgradeToolStripMenuItem.Enabled = false;
            SetR3ToolStripMenuItem.Enabled = false;      
        }

      
     
      
     
        //配置界面的窗体
        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
               Form currForm= this.ActiveMdiChild;
               if (currForm == null || currentType != 0)
                {
                    int old = currentType;
                    currentType = 0;
                    R1UhfConfigForm configForm = new R1UhfConfigForm(isOpen);// 
                    configForm.WindowState = FormWindowState.Maximized;
                    configForm.MdiParent = this;//设置当前窗体为子窗体的父窗体
                    configForm.Show();//显示窗体
                    if (currForm != null)
                    {
                        if (old == 0 || old == 2)
                        {
                            currForm.Close();
                        }
                        else
                        {
                            currForm.Hide();
                        }
                    }
                      
                 
                }
            }
            catch (Exception ex)
            {

            }  
        }
        //UHF版本号
        private void uHFVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isOpen)
            {
                StringBuilder sb = new StringBuilder();
                frmWaitingBox f = new frmWaitingBox((obj, args) =>
                {
                    string hardwareV = uhf.GetHardwareVersion().Replace("\0", "");
                    string softWareV = uhf.GetSoftwareVersion().Replace("\0", "");
                    string mainboardVer = uhf.GetSTM32Version().Replace("\0","") ;
                    string version = uhf.GetAPIVersion().Replace("\0", "");
                    //int id = uhf.GetUHFGetDeviceID();
                    if (Common.isEnglish)
                    {
                        sb.Append("Hardware version:  ");
                        sb.Append(hardwareV);
                        sb.Append("\r\nFirmware  version:  ");
                        sb.Append(softWareV);
                        if (mainboardVer != "")
                        {
                            sb.Append("\r\nMainboard  version:  ");
                            sb.Append(mainboardVer);
                        }
                    }
                    else
                    {
                        sb.Append("固件版本:  ");
                        sb.Append(softWareV);

                        sb.Append("\r\n硬件版本:  ");
                        sb.Append(hardwareV);

                        if (mainboardVer != "")
                        {
                            sb.Append("\r\n主板版本:  ");
                            sb.Append(mainboardVer);
                        }
                    }

                    if (version!=null && version != "")
                    {
                        sb.Append("\r\nAPI Version:  ");
                        sb.Append(version);
                    }


                });
                f.ShowDialog(this);

                MessageBoxEx.Show(this,sb.ToString());
             
            }
        }
   
      
        //打开 
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (toolStripOpen.Text == strOpen)
            {
                bool result = uhf.OpenUsb();
                string msg = Common.isEnglish ? "failure!" : "失败!";
                if (result)
                {
                    toolStripOpen.Text = strClose;
                    isOpen = true;
                    if (eventOpen != null)
                    {
                        eventOpen(true);
                    }
                    enableControls();
                    return;
                }
                showMessage(msg);

            }
            else {
                if (UHFClose())
                {
                    disableControls();
                    toolStripOpen.Text = strOpen;
                    isOpen = false;
                    if (eventOpen != null)
                    {
                        eventOpen(false);
                    }
                }
            }

        }

    

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            UHFClose();
        }

        private bool UHFClose()
        {
            uhf.CloseUsb();
            isOpen = false;
            return true;
        }
 
        private void SwitchShowUI()
        {
            if (Common.isEnglish)
            {
                uHFVersionToolStripMenuItem.Text = "UHF Info";
                configToolStripMenuItem.Text = "UHF Config";
                toolStripMenuItem1.Text = "Temperature";
                uHFUpgradeToolStripMenuItem.Text = "UHF Upgrade";
                toolStripLabel4.Text = "Mode";
                strOpen = "  Open  ";
                strClose = "  Close  ";
                toolStripLabel3.Text = "语言";
                SetR3ToolStripMenuItem.Text = "Device Config";
            }
            else
            {
                toolStripMenuItem1.Text = "温度";
                configToolStripMenuItem.Text = "超高频配置";
                uHFVersionToolStripMenuItem.Text = "UHF信息";
                uHFUpgradeToolStripMenuItem.Text = "UHF固件升级";
                toolStripLabel4.Text = "通信方式";
                strOpen = " 打开 ";
                strClose = " 关闭 ";
                toolStripLabel3.Text = "Language";
                SetR3ToolStripMenuItem.Text = "设备参数配置";

            }

            if (toolStripOpen.Text.Trim() == "Open" || toolStripOpen.Text.Trim() == "打开")
                toolStripOpen.Text = strOpen;
            else
                toolStripOpen.Text = strClose;
        }


        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox1.SelectedIndex == 0)
            {
                Common.isEnglish = false;
            }
            else
            {
                Common.isEnglish = true;
            }
            SwitchShowUI();

            if (eventSwitchUI != null)
            {
                eventSwitchUI();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (isOpen)
            {
                frmWaitingBox f = new frmWaitingBox((obj, args) =>
                {
                    string Temperature = uhf.GetTemperature();
                    string temp = (Common.isEnglish ? "Temperature:" : "温度:") + Temperature + "℃";
                    frmWaitingBox.message = temp;
                    System.Threading.Thread.Sleep(1500);
                });
                f.ShowDialog(this);
            }
        }


        private void uHFUpgradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                UHFUpgradeForm configForm = new UHFUpgradeForm(Common.isEnglish);
                configForm.StartPosition = FormStartPosition.CenterParent;
                configForm.ShowDialog();
            }
            catch (Exception ex)
            {

            }
        }
        private void menuStrip1_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            if (e.Item.Text.Length == 0             //隐藏子窗体图标
              || e.Item.Text == "最小化(&N)"      //隐藏最小化按钮
              || e.Item.Text == "还原(&R)"           //隐藏还原按钮
              || e.Item.Text == "关闭(&C)")         //隐藏关闭按钮
            {
                e.Item.Visible = false;
            }
        }



        private void SetR3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form currForm = this.ActiveMdiChild;
                if (currForm == null || currentType != 1)
                {
                    int old = currentType;
                    currentType = 1;
                    R1DevcieConfig configForm = new R1DevcieConfig(isOpen);// 
                    configForm.WindowState = FormWindowState.Maximized;
                    configForm.MdiParent = this;//设置当前窗体为子窗体的父窗体
                    configForm.Show();//显示窗体
                    if (currForm != null)
                    {
                        if (old == 0 || old == 2)
                        {
                            currForm.Close();
                        }
                        else
                        {
                            currForm.Hide();
                        }
                    }


                }
            }
            catch (Exception ex)
            {

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
