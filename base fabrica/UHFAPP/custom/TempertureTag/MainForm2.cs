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
 
namespace UHFAPP
{
    public partial class MainForm2 : BaseForm
    {
        public delegate void DelegateOpen(bool open);
        public static event DelegateOpen eventOpen = null;

        public delegate void DelegateSwitchUI();
        public static event DelegateSwitchUI eventSwitchUI = null;

        string strOpen1 = "  Open  ";
        string strOpen2 = "  打开  ";
        string strClose1 = "  Close  ";
        string strClose2 = "  关闭  ";

        private int currentType = 0;
        private bool isOpen = false;
        public MainForm2 mainform = null;
        public MainForm2()
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

            UHFAPP.IPConfig.IPEntity  ipEntity= IPConfig.getIPConfig();
            if (ipEntity != null)
            {
                txtPort.Text = ipEntity.Port.ToString();
                ipControl1.IpData = new string[] { ipEntity.Ip[0], ipEntity.Ip[1], ipEntity.Ip[2], ipEntity.Ip[3] };
            }
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
           ScanEPC();
           getComPort();
           MenuItemScanEPC.Enabled = false;
           MenuItemReadWriteTag.Enabled = false;
           configToolStripMenuItem.Enabled = false;
           uHFVersionToolStripMenuItem.Enabled = false;
           killLockToolStripMenuItem.Enabled = false;
           ToolStripMenuItem.Enabled = false;
           toolStripMenuItem1.Enabled = false;
          // MenuItemReceiveEPC.Enabled = false;
           combCommunicationMode.SelectedIndex = 1;
         

        }
        public void enableControls()
        {
            menuStrip1.Enabled = true;
        }
        public void disableControls()
        {
            menuStrip1.Enabled = false;
        }

        //读写数据
        private void MenuItemReadWriteTag_Click(object sender, EventArgs e)
        {
             ReadWriteTag("");
        }
        //扫描EPC
        private void MenuItemScanEPC_Click(object sender, EventArgs e)
        {
            ScanEPC();
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
        //读取EPC窗体
        private void ScanEPC()
        {
            try
            {
                toolStripStatusLabel1.Visible = true;
               Form currForm= this.ActiveMdiChild;
               if (currForm == null || currentType != 0)
                {
                    //currentType = 0;
                    //ReadEPCForm frm_epcScan = new ReadEPCForm(isOpen, mainform);//子窗体实例化
                    //frm_epcScan.WindowState = FormWindowState.Maximized;
                    //frm_epcScan.MdiParent = this;//设置当前窗体为子窗体的父窗体
                    //frm_epcScan.Show();//显示窗体
                    //if (currForm != null)
                    //    currForm.Close();

                    int old = currentType;
                    currentType = 0;
                    ReadEPCForm2 frm_epcScan = new ReadEPCForm2(isOpen, mainform);//子窗体实例化// ReadEPCForm frm_epcScan = (ReadEPCForm)Common.getForm("ReadEPCForm",this);//子窗体实例化
                    frm_epcScan.WindowState = FormWindowState.Maximized;
                    frm_epcScan.MdiParent = this;//设置当前窗体为子窗体的父窗体
                    frm_epcScan.Show();//显示窗体
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

        //读写数据窗体
        public void ReadWriteTag(string tag)
        {
            toolStripStatusLabel1.Visible = false;
            try
            {
                Form currForm = this.ActiveMdiChild;
                if (currForm == null || currentType != 1)
                {
                   
                    //currentType = 1;
                     //ReadWriteTagForm frm_readWriter = new ReadWriteTagForm(isOpen,tag);//子窗体实例化
                    //frm_readWriter.WindowState = FormWindowState.Maximized;
                    //frm_readWriter.MdiParent = this;//设置当前窗体为子窗体的父窗体
                    //frm_readWriter.Show();//显示窗体
                    //if (currForm != null)
                    //    currForm.Close();

                    int old = currentType;
                    currentType = 1;
                    ReadWriteTagForm frm_readWriter = (ReadWriteTagForm)Common.GetForm("ReadWriteTagForm", this);
                    frm_readWriter.SetTAG(isOpen, tag);
                    frm_readWriter.WindowState = FormWindowState.Maximized;
                    frm_readWriter.MdiParent = this;//设置当前窗体为子窗体的父窗体
                    frm_readWriter.Show();//显示窗体
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

        //配置界面的窗体
        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Visible = false;
            try
            {
               Form currForm= this.ActiveMdiChild;
               if (currForm == null || currentType != 2)
                {
                    //currentType =2;
                    //ConfigForm configForm = new ConfigForm(isOpen);//子窗体实例化
                    //configForm.WindowState = FormWindowState.Maximized;
                    //configForm.MdiParent = this;//设置当前窗体为子窗体的父窗体
                    //configForm.Show();//显示窗体
                    //if (currForm != null)
                    //    currForm.Close();

                    int old = currentType;
                    currentType = 2;
                    ConfigForm configForm = new ConfigForm(isOpen);//子窗体实例化 ConfigForm configForm = (ConfigForm)Common.getForm("ConfigForm", this);
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


        //lock窗体
        private void Lock()
        {
            toolStripStatusLabel1.Visible = false;
            try
            {
                Form currForm = this.ActiveMdiChild;
                if (currForm == null || currentType != 3)
                {
                    //currentType = 3;
                    //Kill_LockForm frm_readWriter = new Kill_LockForm(isOpen);//子窗体实例化
                    //frm_readWriter.WindowState = FormWindowState.Maximized;
                    //frm_readWriter.MdiParent = this;//设置当前窗体为子窗体的父窗体
                    //frm_readWriter.Show();//显示窗体
                    //if (currForm != null)
                    //    currForm.close();

                    int old = currentType;
                    currentType = 3;
                    Kill_LockForm frm_readWriter = (Kill_LockForm)Common.GetForm("Kill_LockForm", this);
                    frm_readWriter.WindowState = FormWindowState.Maximized;
                    frm_readWriter.MdiParent = this;//设置当前窗体为子窗体的父窗体
                    frm_readWriter.Show();//显示窗体
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
                    string hardwareV = uhf.GetHardwareVersion();
                    string softWareV = uhf.GetSoftwareVersion();
                    //int id = uhf.GetUHFGetDeviceID();
                    if (Common.isEnglish)
                    {
                        sb.Append("Hardware version:  ");
                        sb.Append(hardwareV);
                        sb.Append("\r\nFirmware  version:  ");
                        sb.Append(softWareV);
                        //sb.Append("\r\nDevice ID:  ");
                        //sb.Append(id);
                    }
                    else
                    {
                        sb.Append("硬件版本:  ");
                        sb.Append(hardwareV);
                        sb.Append("\r\n固件版本:  ");
                        sb.Append(softWareV);
                        //sb.Append("\r\n模块ID:  ");
                        //sb.Append(id);
                    }
                });
                f.ShowDialog(this);

                MessageBoxEx.Show(this,sb.ToString());
             
            }
        }

        //打开串口
        private void toolStripButton1_Click(object sender, EventArgs e)
        {

            if (toolStripOpen.Text == strOpen1 || toolStripOpen.Text == strOpen2)
            {
                //----------------------------
       
                int com = 0;//串口号
                string ip = "";//ip地址
                uint portData = 0;//端口
                int type = combCommunicationMode.SelectedIndex;//0

                if (type == 0)
                {
                    //串口
                    com = int.Parse(cmbComPort.SelectedItem.ToString().Replace("COM", ""));
                }
                else if (type == 1)
                {
                    if (txtPort.Text == "")
                    {
                        MessageBox.Show("fail!");
                        return;
                    }
                    //网口
                    char[] port = txtPort.Text.ToCharArray();
                    for (int k = 0; k < port.Length; k++)
                    {
                        if (port[k] != '0' && port[k] != '1' && port[k] != '2' && port[k] != '3' && port[k] != '4' &&
                            port[k] != '5' && port[k] != '6' && port[k] != '7' && port[k] != '8' && port[k] != '9')
                        {
                            MessageBox.Show("fail!");
                            return;
                        }
                    }
                    portData = uint.Parse(txtPort.Text);
                    ip = cmbComPort.Text;
                }
                else
                {
                    MessageBox.Show("fail!");
                    return;

                }
                //---------------------------
                string msg = Common.isEnglish ? "connecting..." : "连接中...";
                frmWaitingBox f = new frmWaitingBox((obj, args) =>
                {

                    bool result = false;// UHFOpen();

                    if (type == 0)
                    {
                        result = ((UHFAPI)uhf).Open(com);
                    }
                    else
                    {
                        result = ((UHFAPI)uhf).TcpConnect(ip, portData);
                    }
                    if (result)
                    {
                        this.Invoke(new EventHandler(delegate
                        {
                            combCommunicationMode.Enabled = false;
                            cmbComPort.Enabled = false;
                            if (toolStripOpen.Text == strOpen1)
                                toolStripOpen.Text = strClose1;
                            else
                                toolStripOpen.Text = strClose2;

                            isOpen = true;
                            if (eventOpen != null)
                            {
                                eventOpen(true);
                            }

                            MenuItemScanEPC.Enabled = true;
                            MenuItemReadWriteTag.Enabled = true;
                            configToolStripMenuItem.Enabled = true;
                            uHFVersionToolStripMenuItem.Enabled = true;
                            killLockToolStripMenuItem.Enabled = true;
                          //  MenuItemReceiveEPC.Enabled = true;
                            toolStripMenuItem1.Enabled = true;
                            ToolStripMenuItem.Enabled = true;
                        }));
                    }
                    else
                    {
                        //MessageBox.Show("f");
                          frmWaitingBox.message = "fail";
                          Thread.Sleep(1000);
                    }
                }, msg);
                f.ShowDialog(this);

            }
            else {
                if (UHFClose())
                {
                    combCommunicationMode.Enabled = true;
                    cmbComPort.Enabled = true;
                    if (toolStripOpen.Text == strClose1)
                        toolStripOpen.Text = strOpen1;
                    else
                        toolStripOpen.Text = strOpen2;

                    isOpen = false;
                    if (eventOpen != null)
                    {
                        eventOpen(false);
                    }
                    MenuItemScanEPC.Enabled = false;
                    MenuItemReadWriteTag.Enabled = false;
                    configToolStripMenuItem.Enabled = false;
                    uHFVersionToolStripMenuItem.Enabled = false;
                    killLockToolStripMenuItem.Enabled = false;
                    toolStripMenuItem1.Enabled = false;
                    ToolStripMenuItem.Enabled = false;
                   // MenuItemReceiveEPC.Enabled = false;

                
                }
            }

        }
        //设置串口
        private void getComPort()
        {
            string[] ArryPort = System.IO.Ports.SerialPort.GetPortNames();
            cmbComPort.Items.Clear();
            for (int i = 0; i < ArryPort.Length; i++)
            {
                cmbComPort.Items.Add(ArryPort[i]);
            }
            if (cmbComPort.Items.Count > 0)
                cmbComPort.SelectedIndex = cmbComPort.Items.Count-1;

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            UHFClose();
        }

        private void killLockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Lock();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (isOpen)
            {
             
                frmWaitingBox f = new frmWaitingBox((obj, args) =>
                {
                    string Temperature = uhf.GetTemperature();
                    string temp =(Common.isEnglish?"Temperature:":"温度:") + Temperature + "℃";
                    frmWaitingBox.message = temp;
                    System.Threading.Thread.Sleep(1500);
                });
                f.ShowDialog(this);
   
          

            }
        }



      
        private void SwitchShowUI() {
            if (Common.isEnglish)
            {
                toolStripStatusLabel1.Text = "";//"                                                         "+ "                                                          tip: 1. right key can copy the selected label.     2. double-click the selected label can jump to the r/w  UI.";
                MenuItemScanEPC.Text = "ReadEPC";
                MenuItemReadWriteTag.Text = "ReadWriteTag";
                configToolStripMenuItem.Text = "Configuration";
                killLockToolStripMenuItem.Text = "Kill-Lock";
                uHFVersionToolStripMenuItem.Text = "UHF Info";
                toolStripMenuItem1.Text = "Temperature";
                MenuItemReceiveEPC.Text = "UDP-ReceiveEPC";

                toolStripLabel4.Text = "Mode";
                int index = combCommunicationMode.SelectedIndex;//记录上一次的选择记录
                combCommunicationMode.Items.Clear();
                combCommunicationMode.Items.Add("SerialPort");
                combCommunicationMode.Items.Add("network");
                combCommunicationMode.Items.Add("USB");
                combCommunicationMode.SelectedIndex = index;

                if (toolStripOpen.Text == strOpen2)
                {
                    toolStripOpen.Text = strOpen1;
                }
                else if (toolStripOpen.Text ==strClose2)
                {
                    toolStripOpen.Text = strClose1;
                }

                if (btnConnect.Text == strOpen2)
                {
                    btnConnect.Text = strOpen1;
                }
                else if (btnConnect.Text == strClose2)
                {
                    btnConnect.Text = strClose1;
                }
                toolStripLabel3.Text = "语言";
            }
            else {
                toolStripStatusLabel1.Text = "                                                        "
                    + "                                                        提示：1.右键可以复制选中的标签    2.双击选中的标签可以跳转到读写界面";
                MenuItemScanEPC.Text = "盘点EPC";
                MenuItemReadWriteTag.Text = "读写标签";
                configToolStripMenuItem.Text = "配置";
                killLockToolStripMenuItem.Text = "锁标签";
                MenuItemReceiveEPC.Text = "UDP-ReceiveEPC";
                uHFVersionToolStripMenuItem.Text = "UHF信息";
                toolStripMenuItem1.Text = "温度";

                toolStripLabel4.Text = "通信方式";
                int index = combCommunicationMode.SelectedIndex;//记录上一次的选择记录
                combCommunicationMode.Items.Clear();
                combCommunicationMode.Items.Add("串口");
                combCommunicationMode.Items.Add("网络");
                combCommunicationMode.Items.Add("USB");
                combCommunicationMode.SelectedIndex = index;
 

                if (toolStripOpen.Text == strOpen1)
                {
                    toolStripOpen.Text = strOpen2;
                }
                else if (toolStripOpen.Text == strClose1)
                {
                    toolStripOpen.Text = strClose2;
                }


                if (btnConnect.Text == strOpen1)
                {
                    btnConnect.Text = strOpen2;
                }
                else if (btnConnect.Text == strClose1)
                {
                    btnConnect.Text = strClose2;
                }
                toolStripLabel3.Text = "Language";
              
            }
         
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
         
          
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

        private void toolStripComboBox2_TextChanged(object sender, EventArgs e)
        {
            if (combCommunicationMode.SelectedIndex == 0)
            {
                getComPort();
                panel1.Visible = false;
                plUsb.Visible = false;
            }
            else if (combCommunicationMode.SelectedIndex == 1)
            {
                panel1.Visible = true;
                plUsb.Visible = false;
            }
            else if (combCommunicationMode.SelectedIndex == 2)
            {
                plUsb.Visible = true;  
            }
        }

       
        private void getIP() {

            string[] ArryPort = System.IO.Ports.SerialPort.GetPortNames();
            cmbComPort.Items.Clear();

            IPHostEntry myEntry = Dns.GetHostByName(Dns.GetHostName());
            for (int k = 0; k < myEntry.AddressList.Length; k++) {

                IPAddress myIPAddress = new IPAddress(myEntry.AddressList[k].Address);

                cmbComPort.Items.Add(myIPAddress.ToString());
            }

            if (cmbComPort.Items.Count > 0)
                cmbComPort.SelectedIndex = cmbComPort.Items.Count - 1;
        }
          


        private bool UHFOpen()
        {
            bool result = false;
            result = ((UHFAPI)uhf).Open(int.Parse(cmbComPort.SelectedItem.ToString().Replace("COM", "")));
            return result;
        }
    
        private bool UHFClose() {

            if (combCommunicationMode.SelectedIndex == 1)
            {
                ((UHFAPI)uhf).TcpDisconnect();
                return true;
            }
            else
            {
                return uhf.Close();
            }
            return false;
          
        }

     

        private void btnConnect_Click(object sender, EventArgs e)
        {

            if (btnConnect.Text == strOpen1 || btnConnect.Text == strOpen2)
            {
                //----------------------------
                if (txtPort.Text == "")
                {
                    MessageBox.Show("fail!");
                    return;
                }
                char[] port = txtPort.Text.ToCharArray();
                for (int k = 0; k < port.Length; k++)
                {
                    if (port[k] != '0' && port[k] != '1' && port[k] != '2' && port[k] != '3' && port[k] != '4' &&
                        port[k] != '5' && port[k] != '6' && port[k] != '7' && port[k] != '8' && port[k] != '9')
                    {

                        MessageBox.Show("端口号只能输入数字!");
                        return;
                    }
                }
              

                uint portData = uint.Parse(txtPort.Text);
                string[] tempIp = ipControl1.IpData;
                StringBuilder sb = new StringBuilder();
                sb.Append(tempIp[0]);
                sb.Append(".");
                sb.Append(tempIp[1]);
                sb.Append(".");
                sb.Append(tempIp[2]);
                sb.Append(".");
                sb.Append(tempIp[3]);
                string ip = sb.ToString();

          
                //---------------------------
                string msg = Common.isEnglish ? "connecting..." : "连接中...";
                frmWaitingBox f = new frmWaitingBox((obj, args) =>
                {

                    bool result = false;
                    result = ((UHFAPI)uhf).TcpConnect(ip, portData);
                    if (result)
                    {
                        this.Invoke(new EventHandler(delegate
                        {
                            combCommunicationMode.Enabled = false;
                            txtPort.Enabled = false;
                            ipControl1.Enabled = false;
                            if (btnConnect.Text == strOpen1)
                                btnConnect.Text = strClose1;
                            else
                                btnConnect.Text = strClose2;

                            isOpen = true;
                            if (eventOpen != null)
                            {
                                eventOpen(true);
                            }

                            UHFAPP.IPConfig.IPEntity entity = new IPConfig.IPEntity();
                            entity.Port = (int)portData;
                            entity.Ip = ipControl1.IpData;
                            IPConfig.setIPConfig(entity);

                            MenuItemScanEPC.Enabled = true;
                            MenuItemReadWriteTag.Enabled = true;
                            configToolStripMenuItem.Enabled = true;
                            uHFVersionToolStripMenuItem.Enabled = true;
                            killLockToolStripMenuItem.Enabled = true;
                            toolStripMenuItem1.Enabled = true;
                            ToolStripMenuItem.Enabled = true;
                          //  MenuItemReceiveEPC.Enabled = true;
                        }));
                    }
                    else
                    {
                        //MessageBox.Show("f");
                        frmWaitingBox.message = "fail";
                        Thread.Sleep(1000);
                    }
                }, msg);
                f.ShowDialog(this);

            }
            else
            {

                    ((UHFAPI)uhf).TcpDisconnect();
                    combCommunicationMode.Enabled = true;
                    txtPort.Enabled = true;
                    ipControl1.Enabled = true;
                    if (btnConnect.Text == strClose1)
                        btnConnect.Text = strOpen1;
                    else
                        btnConnect.Text = strOpen2;

                    isOpen = false;
                    if (eventOpen != null)
                    {
                        eventOpen(false);
                    }
                    MenuItemScanEPC.Enabled = false;
                    MenuItemReadWriteTag.Enabled = false;
                    configToolStripMenuItem.Enabled = false;
                    uHFVersionToolStripMenuItem.Enabled = false;
                    killLockToolStripMenuItem.Enabled = false;
                    toolStripMenuItem1.Enabled = false;
                    ToolStripMenuItem.Enabled = false;
                    //MenuItemReceiveEPC.Enabled = false;
            }
           
        }

        private void receiveEPCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Visible = false;
            try
            {
                Form currForm = this.ActiveMdiChild;
                if (currForm == null || currentType != 4)
                {
                    //currentType = 4;
                    //ReceiveEPC configForm = new ReceiveEPC();//子窗体实例化
                    //configForm.WindowState = FormWindowState.Maximized;
                    //configForm.MdiParent = this;//设置当前窗体为子窗体的父窗体
                    //configForm.Show();//显示窗体
                    //if (currForm != null)
                    //    currForm.Close();

                    int old = currentType;
                    currentType = 4;
                    ReceiveEPC configForm = (ReceiveEPC)Common.GetForm("ReceiveEPC", this);//子窗体实例化
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

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }
        
        private void btnUsbOpen_Click(object sender, EventArgs e)
        {

            if (btnUsbOpen.Text == strOpen1 || btnUsbOpen.Text == strOpen2)
            {
                //----------------------------
                bool result = false;// uhf.Open(this);
                if (result)
                {
                    if (btnUsbOpen.Text == strOpen1)
                        btnUsbOpen.Text = strClose1;
                    else
                        btnUsbOpen.Text = strClose2;

                    isOpen = true;
                    if (eventOpen != null)
                    {
                        eventOpen(true);
                    }
                    combCommunicationMode.Enabled = false;
                    MenuItemScanEPC.Enabled = true;
                    MenuItemReadWriteTag.Enabled = true;
                    configToolStripMenuItem.Enabled = true;
                    uHFVersionToolStripMenuItem.Enabled = true;
                    killLockToolStripMenuItem.Enabled = true;
                    toolStripMenuItem1.Enabled = true;
                    ToolStripMenuItem.Enabled = true;
                }
                else
                {
                    MessageBox.Show("fail");
                    
                }

            }
            else
            {
                if (UHFClose())
                {
                    if (btnUsbOpen.Text == strClose1)
                        btnUsbOpen.Text = strOpen1;
                    else
                        btnUsbOpen.Text = strOpen2;

                    isOpen = false;
                    if (eventOpen != null)
                    {
                        eventOpen(false);
                    }
                    combCommunicationMode.Enabled = true;
                    MenuItemScanEPC.Enabled = false;
                    MenuItemReadWriteTag.Enabled = false;
                    configToolStripMenuItem.Enabled = false;
                    uHFVersionToolStripMenuItem.Enabled = false;
                    killLockToolStripMenuItem.Enabled = false;
                    toolStripMenuItem1.Enabled = false;
                    ToolStripMenuItem.Enabled = false;
                }
            }
        }

        
        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Visible = false;
            try
            {
                Form currForm = this.ActiveMdiChild;
                if (currForm == null || currentType != 11)
                { 
                    int old = currentType;
                    currentType = 11;
                   // ConfigForm2 config = (ConfigForm2)Common.getForm("ConfigForm2", this);
                    TempertureTagForm config = new TempertureTagForm(isOpen, mainform);
                    config.WindowState = FormWindowState.Maximized;
                    config.MdiParent = this;//设置当前窗体为子窗体的父窗体
                    config.Show();//显示窗体
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
    }
}
