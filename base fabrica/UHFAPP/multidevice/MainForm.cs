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
using BLEDeviceAPI;
using UHFAPP.utils;

namespace UHFAPP.multidevice
{
    public partial class MainForm : BaseForm
    {

        private bool isRuning = false;
        private delegate void SetTextCallback(string epc,  string rssi, string count, string ant,  string ip);
        private SetTextCallback setTextCallback;
        private List<EpcInfo> epcList = new List<EpcInfo>();
        bool FlagInventory1 = false;
        bool FlagInventory2 = false;
        public MainForm()
        {
            InitializeComponent();
            setTextCallback = new SetTextCallback(UpdataEPC);
        }
        /// <summary>
        /// IP1连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConn_Click(object sender, EventArgs e)
        {
             string ip = txtIP.Text;
             uint port = uint.Parse(txtPort.Text);
             btnConn.Enabled = false;
             frmWaitingBox f = new frmWaitingBox((obj, args) =>
             {
                 bool result = uhf.TcpConnect(ip, port);
                 if (!result)
                 {
                     frmWaitingBox.message = "fail";
                     Thread.Sleep(1000);
                     this.Invoke(new EventHandler(delegate
                     {
                         btnConn.Enabled = true;
                     }));
                 }
                 else {
                     this.Invoke(new EventHandler(delegate
                     {
                         btnStart1.Enabled = true;
                         btnDisConn.Enabled = true;
                     }));

                 }
                  
             }, "connecting...");
             f.ShowDialog(this);

        }
        /// <summary>
        /// IP1断开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDisConn_Click(object sender, EventArgs e)
        {
            if (FlagInventory1)
            {
                btnStart1.Text = "Start";
                int id = getId(txtIP.Text);
                if (id >= 0)
                {
                    bool result = uhf.StopById(id);
                }
                FlagInventory1 = false;
                StopThread();
            }

            if (SelectDevice(txtIP.Text))
            {
                uhf.TcpDisconnect();
            }
            btnDisConn.Enabled = false;
            btnStart1.Enabled = false;
            btnConn.Enabled = true;

        }
        /// <summary>
        /// IP2连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConn2_Click(object sender, EventArgs e)
        {
            string ip = txtIP2.Text;
            uint port = uint.Parse(txtPort2.Text);
            btnConn2.Enabled = false;

            frmWaitingBox f = new frmWaitingBox((obj, args) =>
            {
                bool result = uhf.TcpConnect(ip, port);
                if (!result)
                {
                    frmWaitingBox.message = "fail";
                    Thread.Sleep(1000);
                    this.Invoke(new EventHandler(delegate
                    {
                        btnConn2.Enabled = true;
                    }));
                }
                else {
                    this.Invoke(new EventHandler(delegate
                    {
                        btnDisConn2.Enabled = true;
                        btnStart2.Enabled = true;
                    }));

                }
              
            }, "connecting...");
            f.ShowDialog(this);
        }
        /// <summary>
        /// IP2断开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDisConn2_Click(object sender, EventArgs e)
        {
            if (FlagInventory2)
            {
                btnStart2.Text = "Start";
                int id = getId(txtIP2.Text);
                if (id >= 0)
                {
                    bool result = uhf.StopById(id);
                }
                FlagInventory2 = false;
                StopThread();
            }

            if (SelectDevice(txtIP2.Text))
            {
                uhf.TcpDisconnect();
            }
            btnConn2.Enabled = true;
            btnDisConn2.Enabled = false;
            btnStart2.Enabled = false;
        }


      
        /// <summary>
        /// IP1开始盘点、停止盘点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart1_Click(object sender, EventArgs e)
        {
            if (btnStart1.Text == "Start")
            {
                int id = getId(txtIP.Text);
                if (id >= 0)
                {
                    if (uhf.InventoryById(id)) {
                        btnStart1.Text = "Stop";
                        FlagInventory1 = true;
                        StartThread();
                        return;
                    }
                }
                MessageBox.Show("失败!");
            }
            else
            {
                if (FlagInventory1)
                {
                    btnStart1.Text = "Start";
                    int id = getId(txtIP.Text);
                    if (id >= 0)
                    {
                        bool result = uhf.StopById(id);
                    }
                    FlagInventory1 = false;
                    StopThread();
                }
            }


        }
        /// <summary>
        /// IP2开始盘点、停止盘点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart2_Click(object sender, EventArgs e)
        {
            if (btnStart2.Text == "Start"){
            
                int id = getId(txtIP2.Text);
                if (id >= 0)
                {
                    bool result = uhf.InventoryById(id);
                    if (result) {
                        btnStart2.Text = "Stop";
                        FlagInventory2 = true;
                        StartThread();
                        return;
                    }
                }
                MessageBox.Show("失败!");
            }
            else
            {
                if (FlagInventory2)
                {
                    btnStart2.Text = "Start";
                    int id = getId(txtIP2.Text);
                    if (id >= 0)
                    {
                        bool result = uhf.StopById(id);
                    }
                    FlagInventory2 = false;
                    StopThread();
                }
            }
        }
        /// <summary>
        /// 开始盘点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartThread()
        {
            if (!isRuning)
            {
                isRuning = true;
                new Thread(new ThreadStart(delegate { ReadEPC(); })).Start();
            }
        }
        /// <summary>
        /// 结束盘点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopThread()
        {
            if (!FlagInventory1 && !FlagInventory2)
            {
                isRuning = false;
                string msg = Common.isEnglish ? "wait..." : "正在停止...";
                frmWaitingBox f = new frmWaitingBox((obj, args) =>
                {
                    Thread.Sleep(1000);

                }, msg);
                f.ShowDialog(this);
            }
    
        }
        
        /// <summary>
        /// 获取功率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPowerGet_Click(object sender, EventArgs e)
        {
            if (SelectDevice())
            {
                string msg = "failure";
                byte power = 0;
                if (uhf.GetPower(ref power))
                {
                    cmbPower.SelectedIndex = power - 1;
                    msg = "success";
                }
                showMessage(msg);
            }
        }
        /// <summary>
        /// 设置功率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPowerSet_Click(object sender, EventArgs e)
        {
            if (SelectDevice())
            {
                string msg = "failure";
                if (cmbPower.SelectedIndex >= 0)
                {
                    byte power1 = (byte)(cmbPower.SelectedIndex + 1);

                    int save = cbPower.Checked ? 1 : 0;
                    if (uhf.SetPower((byte)save, power1))
                    {
                        msg = "success";
                    }

                }
                showMessage(msg);
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
        private bool SelectDevice(string ip)
        {
            List<DeviceInfo> list = uhf.LinkGetDeviceInfo();
            if (list != null)
            {
                for (int k = 0; k < list.Count; k++)
                {
                    if (list[k].Ip == ip)
                    {
                        uhf.LinkSelectDevice(list[k].Id);
                        return true;
                    }
                }

            }
            return false;
        }

        private bool SelectDevice()
        {
            List<DeviceInfo> list = uhf.LinkGetDeviceInfo();
            if (list == null || list.Count == 0)
            {
                MessageBoxEx.Show(this, "请先连接设备!");
                return false;
            }
            DeviceListForm f = new DeviceListForm(list);
            f.ShowDialog();
            if (!SelectDevice(f.ip))
            {
                MessageBoxEx.Show(this, "failure!");
                return false;
            }
            return true;
        }
        public int getId(string ip)
        {
            List<DeviceInfo> deviceList = uhf.LinkGetDeviceInfo();
            if (deviceList != null && deviceList.Count > 0)
            {
                for (int k = 0; k < deviceList.Count; k++)
                {
                    DeviceInfo deviceInfo = deviceList[k];
                    if (ip == deviceInfo.Ip)
                    {
                        return deviceInfo.Id;
                    }
                }
            }
            return -1;
        }
        //获取epc
        private void ReadEPC()
        {
    
            while (true)
            {
                TagInfo tagInfo = uhf.getTagData();
                if (tagInfo != null && tagInfo.UhfTagInfo != null)
                {
                    string ip = getIP(tagInfo.Id);  
                    UHFTAGInfo info = tagInfo.UhfTagInfo;
                    string data = info.Epc;
                    if (info.Tid != null && info.Tid.Length > 0)
                    {
                        data = "EPC:" + data;
                        data = data + "\r\nTID:" + info.Tid;
                    }
                    if (info.User != null && info.User.Length > 0)
                    {
                        data = data + "\r\nUSER:" + info.User;
                    }
                    if (this.IsHandleCreated)
                    {
                        this.BeginInvoke(setTextCallback, new object[] { data, info.Rssi, "1", info.Ant, ip });
                    }
                }
                else
                {
                    if (isRuning)
                        Thread.Sleep(5);
                    else
                        break;
                }
            }

        }

        private string getIP(int id) {
            List<DeviceInfo> deviceList = uhf.LinkGetDeviceInfo();
            for (int k = 0; deviceList!=null && k < deviceList.Count; k++)
            {
                if (deviceList[k].Id == id)
                {
                    return deviceList[k].Ip;
                }
            }
            return "";
        }

        private void UpdataEPC(string epc, string rssi, string count, string ant, string ip)
        {
            bool[] exist = new bool[1];
            int index = CheckUtils.getInsertIndex(epcList, epc, exist);
            if (exist[0])
            {
                lvEPC.Items[index].SubItems["RSSI"].Text = rssi;
                lvEPC.Items[index].SubItems["COUNT"].Text = (int.Parse(lvEPC.Items[index].SubItems["COUNT"].Text) + int.Parse(count)).ToString();
                lvEPC.Items[index].SubItems["ANT"].Text = ant;
                lvEPC.Items[index].SubItems["IP"].Text = ip;
            }
            else
            {
                ListViewItem lv = new ListViewItem();
                lv.Text = (index + 1).ToString();
                ListViewItem.ListViewSubItem itemEPC = new ListViewItem.ListViewSubItem();
                itemEPC.Name = "EPC";
                itemEPC.Text = epc;
                lv.SubItems.Add(itemEPC);

                ListViewItem.ListViewSubItem itemRssi = new ListViewItem.ListViewSubItem();
                itemRssi.Name = "RSSI";
                itemRssi.Text = rssi;
                lv.SubItems.Add(itemRssi);

                ListViewItem.ListViewSubItem itemCount = new ListViewItem.ListViewSubItem();
                itemCount.Name = "COUNT";
                itemCount.Text = count;
                lv.SubItems.Add(itemCount);

                ListViewItem.ListViewSubItem itemAnt = new ListViewItem.ListViewSubItem();
                itemAnt.Name = "ANT";
                itemAnt.Text = ant;
                lv.SubItems.Add(itemAnt);

                ListViewItem.ListViewSubItem itemIP = new ListViewItem.ListViewSubItem();
                itemIP.Name = "IP";
                itemIP.Text = ip;
                lv.SubItems.Add(itemIP);

          
                lvEPC.Items.Insert(index, lv);// Add(lv);
                epcList.Insert(index, new EpcInfo(epc, int.Parse(count), DataConvert.HexStringToByteArray(epc)));
            }
            lblCount.Text = (int.Parse(lblCount.Text) + 1).ToString();
 
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            isRuning = false; 
            uhf.LinkDisConnectAllDevice();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            epcList.Clear();
            lvEPC.Items.Clear();
            lblCount.Text = "0";
        }

       
    }
}
