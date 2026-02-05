using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections;
using UHFAPP.interfaces;
using UHFAPP.Receive;
using UHFAPP.utils;
using BLEDeviceAPI;

namespace UHFAPP
{
    public partial class ReceiveEPC : BaseForm
    {
        IAutoReceive iAutoReceive = null;
       private const int max =1024 * 1024;
       private byte[] uhfOriginalData = new byte[max];
       private int wIndex = 0;
       private int rIndex = 0;
       private bool isRuning = true;
       private bool isOpen = false;
       private Thread threadEPC = null;

       int total = 0;
       long beginTime = System.Environment.TickCount;

       List<EpcInfo> epcList = new List<EpcInfo>();
       // 将text更新的界面控件的委托类型
       delegate void GetUHFDataCallback(string epc, string tid, string user,string rssi, string count, string ant);
       GetUHFDataCallback UHFDataCallback;

       delegate void GetRemotelyIPCallback(string remoteip);
       GetRemotelyIPCallback RemotelyIPCallback;
        public ReceiveEPC()
        {
            InitializeComponent();
        }
        private void ReceiveEPC_Load(object sender, EventArgs e)
        {
            cmbMode.SelectedIndex = 0;
            UHFDataCallback = new GetUHFDataCallback(UpdataEPC);
            RemotelyIPCallback = new GetRemotelyIPCallback(GetRemoteIP);
            InitIPAndSerialPort();
        }
        private void ReceiveEPC_FormClosing(object sender, FormClosingEventArgs e)
        {
            isOpen = false;
            isRuning = false;
            DisConnect();
        }

        private void btnScanEPC_Click(object sender, EventArgs e)
        {
            if (btnScanEPC.Text == "Start")
            {
                if (Connect())
                {
                    cmbCom.Enabled = false;
                    cmbMode.Enabled = false;
                    btnScanEPC.Text = "Stop";
                }
                else
                {
                    MessageBox.Show("fail");
                }
            }
            else
            {
                cmbCom.Enabled = true;
                cmbMode.Enabled = true;
                btnScanEPC.Text = "Start";
                DisConnect();
            }
        }

    
        private void InitIPAndSerialPort()
        {
            IPAddress[] ipArray = Dns.GetHostAddresses(Dns.GetHostName());
            if (ipArray != null)
            {
                for (int k = 0; k < ipArray.Length; k++)
                {
                    string ip = ipArray[k].ToString();
                    if (StringUtils.isIP(ip))
                    {
                        cmbIP.Items.Add(ip);
                        cmbIP.SelectedIndex = 0;
                    }
                }
            }
            //--------------------------------------
            string[] ArryPort = System.IO.Ports.SerialPort.GetPortNames();
            cmbCom.Items.Clear();
            for (int i = 0; i < ArryPort.Length; i++)
            {
                cmbCom.Items.Add(ArryPort[i]);
            }
            if (cmbCom.Items.Count > 0)
                cmbCom.SelectedIndex = cmbCom.Items.Count - 1;
           
        }

        private bool Connect()
        {
            if ((iAutoReceive as UdpReceive) != null)
            {
                if (textBox2.Text == "") return false;
                int port = int.Parse(textBox2.Text);
                string ip = cmbIP.SelectedItem.ToString();
                if (!StringUtils.isIP(ip)) return false;
                (iAutoReceive as UdpReceive).SetIP(ip, port);
            }
            else if ((iAutoReceive as SerialPortReceive) != null)
            {
                (iAutoReceive as SerialPortReceive).SetPortName(cmbCom.SelectedItem.ToString());
            }

            if (iAutoReceive.Connect())
            {
                if (threadEPC == null)
                {
                    threadEPC = new Thread(new ThreadStart(ParseUHFData));
                    threadEPC.IsBackground = true;
                    threadEPC.Start();
                }
                return true;
            }
            return false;
        }

        private void DisConnect()
        {
            iAutoReceive.DisConnect();
            isOpen = false;
        }
 
        //解析數據
        private void ParseUHFData()
        {
            try
            {
              const int STATUS_START = 0;
              const int STATUS_5A = 1;
              const int STATUS_LEN_H = 2;
              const int STATUS_LEN_L = 3;
              const int STATUS_CMD = 4;
              const int STATUS_DATA = 5;
              const int STATUS_XOR = 6;
              const int STATUS_END_0D = 7;
              const int STATUS_END_0A = 8;

                
                int tempxor = 0;
                int tempidx = 0;
                int templen = 0;
                int rxstatus = 0;
                bool closeingflag = false;
                byte[] tempbuf=new byte[256];
                while (isRuning)// while (isRuning && (!closeingflag))
                {
                    byte tempdata = 0;
                    if (wIndex > rIndex) {
                          tempdata = uhfOriginalData[rIndex];
                    }
                    else if (wIndex < rIndex)
                    {
                        if (rIndex >= max)
                        {
                            rIndex = 0;
                        }
                          tempdata = uhfOriginalData[rIndex];
                    }
                    else {
                        Thread.Sleep(5);
                        continue;
                    }
                        switch (rxstatus)
                        {
                            case STATUS_START:
                                if (tempdata == 0xA5)
                                {
                                    rxstatus = STATUS_5A;
                                }
                                else
                                {
                                    rxstatus = STATUS_START;
                                }
                                tempxor = 0;
                                tempidx = 0;
                                templen = 0;
                                Array.Clear(tempbuf,0, tempbuf.Length);
                                break;
                            case STATUS_5A:
                                if (tempdata == 0x5A)
                                {
                                    rxstatus = STATUS_LEN_H;
                                }
                                else
                                {
                                    rxstatus = STATUS_START;
                                }
                                break;
                            case STATUS_LEN_H:
                                tempxor = tempxor ^ tempdata;
                                if (tempdata == 0)
                                {
                                    rxstatus = STATUS_LEN_L;
                                }
                                else
                                {
                                    rxstatus = STATUS_START;
                                }
                                break;
                            case STATUS_LEN_L:
                                {
                                    tempxor = tempxor ^ tempdata;
                                    templen = tempdata;
                                    if ((templen < 8) || (templen > 0xFF))
                                    {
                                        rxstatus = STATUS_START;
                                    }

                                    else
                                    {
                                        templen = templen - 8;
                                        rxstatus = STATUS_CMD;
                                    }

                                }
                                break;
                            case STATUS_CMD:
                                {
                                    tempxor = tempxor ^ tempdata;
                                    if (tempdata == 0x83)
                                    {
                                        if (templen > 0)
                                        {
                                            rxstatus = STATUS_DATA;
                                        }
                                        else
                                        {
                                            rxstatus = STATUS_XOR;
                                        }

                                    }
                                    else if ((tempdata == 0x8D) && (templen == 1))
                                    {
                                        rxstatus = STATUS_DATA;
                                        closeingflag = true;
                                    }
                                    else
                                    {
                                        rxstatus = STATUS_START;
                                    }
                                }
                                break;
                            case STATUS_DATA:
                                {
                                    if (closeingflag)
                                    {
                                        if (tempdata != 0)
                                        {
                                           // closeflag = 1;  zp 
                                        }
                                        else
                                        {
                                            // closeflag = 0;  zp 
                                        }
                                        tempxor = tempxor ^ tempdata;
                                        rxstatus = STATUS_XOR;
                                    }
                                    else if (tempidx < templen)
                                    {
                                        tempxor = tempxor ^ tempdata;
                                        tempbuf[tempidx++] = tempdata;
                                        if (tempidx >= templen)
                                        {
                                            rxstatus = STATUS_XOR;
                                        }
                                    }
                                    break;
                                }
                            case STATUS_XOR:
                                {
                                    if (tempxor == tempdata)
                                    {
                                        rxstatus = STATUS_END_0D;
                                    }
                                    else
                                    {
                                        rxstatus = STATUS_START;
                                    }
                                    break;
                                }
                            case STATUS_END_0D:
                                {
                                    if (tempdata == 0x0D)
                                    {
                                        rxstatus = STATUS_END_0A;
                                    }
                                    else
                                    {
                                        rxstatus = STATUS_START;
                                    }
                                }
                                break;
                            case STATUS_END_0A:
                                {
                                    if (tempdata == 0x0A)
                                    {
                                        if (templen <= 0)
                                        {
                                            continue;
                                        }

                                        string epc = "";
                                        string tid = "";
                                        string rssi = "";
                                        string ant = "";
                                        string user = "";
                                        bool result = UHFGetReceived(ref epc, ref tid, ref user, ref rssi, ref ant, tempbuf, templen);
                                        if (result)
                                        {
                                            this.BeginInvoke(UHFDataCallback, new object[] { epc, tid, user, rssi, "1", ant });
                                          //  Console.Out.Write("刷新ui的\n");  
                                        }
                                     
                                        closeingflag = false;
                                    }
                                    rxstatus = STATUS_START;

                                }
                                break;
                            default:
                                {
                                    rxstatus = STATUS_START;
                                }
                                break;

                        }

                        rIndex++;
                  
                    
                }
            }
            catch (Exception ex)
            {

            }

        }
 
     

        private void button1_Click(object sender, EventArgs e)
        {
            epcList.Clear();
            lvEPC.Items.Clear();
            lblTime.Text = "0";
            lblTotal.Text = "0";
            total = 0;
            beginTime = System.Environment.TickCount;
        }



        //读取epc
        public bool UHFGetReceived(ref string epc, ref string tid, ref string user, ref string rssi, ref string ant, byte[] originalData, int len)
        {
            try
            {
                int uLen = len;
                byte[] bufData = originalData;
                if (bufData != null && bufData.Length > 0 && uLen > 0 && (bufData.Length >= uLen))
                {

                    byte[] bUii = null;
                    byte[] bTid = null;
                    byte[] bRssi = null;
                    byte[] bAnt =null;
                    byte[] bUser = null;

                    if (((uLen - 3) < 1) || ((uLen - 3) > 250))
                        return false;

                    int uiilen = ((bufData[0] >> 3) + 1) * 2;

                    if (uiilen == 0) return false;


                    if (len - uiilen > 15) //  uii + tid + user + rssi + ant
                    {

                        bUii = Utils.CopyArray(bufData, 0, uiilen);
                        bTid = Utils.CopyArray(bufData, uiilen, 12);
                        int userLen = uLen - (bUii.Length + bTid.Length + 3);
                        bUser = Utils.CopyArray(bufData, uiilen + 12, userLen);
                        bRssi = Utils.CopyArray(bufData, bUii.Length + bTid.Length + bUser.Length, 2);
                        bAnt = Utils.CopyArray(bufData, bUii.Length + bTid.Length + bUser.Length + bRssi.Length, 1);


                        int tempRSSIH = bRssi[0];
                        int tempRSSIL = bRssi[1];

                        int tempRSSI = tempRSSIH * 256 + tempRSSIL;
                        tempRSSI = 65535 - tempRSSI + 1;
                        if ((tempRSSI < 250) || (tempRSSI > 850))
                        {
                            return false;
                        }
                    }
                    else if (len - uiilen == 15)   //  uii + tid + rssi + ant
                    {

                        bUii = Utils.CopyArray(bufData, 0, uiilen);
                        bTid = Utils.CopyArray(bufData, uiilen, 12);
                        bRssi = Utils.CopyArray(bufData, bUii.Length + bTid.Length, 2);
                        bAnt = Utils.CopyArray(bufData, bUii.Length + bTid.Length + bRssi.Length, 1);
 

                        int tempRSSIH = bRssi[0];
                        int tempRSSIL = bRssi[1];

                        int tempRSSI = tempRSSIH * 256 + tempRSSIL;
                        tempRSSI = 65535 - tempRSSI + 1;
                        if ((tempRSSI < 250) || (tempRSSI > 850))
                        {
                            return false;
                        }
                    }
                    else if ((len - uiilen == 3)) // uii + rssi + ant  \   uii + tid + rssi + ant
                    {

                        bUii= Utils.CopyArray(bufData, 0, uiilen);
                        bRssi = Utils.CopyArray(bufData, bUii.Length , 2);
                        bAnt = Utils.CopyArray(bufData, bUii.Length  + bRssi.Length, 1);
 

                        int tempRSSIH = bRssi[0];
                        int tempRSSIL = bRssi[1];

                        int tempRSSI = tempRSSIH * 256 + tempRSSIL;
                        tempRSSI = 65535 - tempRSSI + 1;
                        if ((tempRSSI < 250) || (tempRSSI > 850))
                        {
                            return false;
                        }
                    } 
 
                    //  uUii = 1byteUII长度+UII数据+1byteTID数据+TID+2bytesRSSI
                    string epc_data = BitConverter.ToString(bUii, 2, bUii.Length-2).Replace("-", "");
                    string uii_data = BitConverter.ToString(bUii, 0, bUii.Length).Replace("-", "");
                    string tid_data = string.Empty; //tid数据
                    string rssi_data = string.Empty;
                    string ant_data = string.Empty;
                    string user_data = string.Empty;
                    if (bTid != null) {
                        tid_data = BitConverter.ToString(bTid, 0, bTid.Length).Replace("-", "");
                    }
                    if (bRssi != null)
                    {
                        string temprssi = BitConverter.ToString(bRssi, 0, bRssi.Length).Replace("-", "");
                        rssi_data = ((Convert.ToInt32(temprssi, 16) - 65535) / 10).ToString();// RSSI  =  (0xFED6   -65535)/10 
                    }
                    if (bAnt != null)
                    {
                        string tempAnt = BitConverter.ToString(bAnt, 0, bAnt.Length).Replace("-", "");
                        ant_data = Convert.ToInt32(tempAnt, 16).ToString();
                    }
                    if (bUser != null)
                    {
                        user_data = BitConverter.ToString(bUser, 0, bUser.Length).Replace("-", "");
                    }
                    epc = epc_data;
                    tid = tid_data;
                    rssi = rssi_data;
                    ant = ant_data;
                    user = user_data;
                 
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex) {
                return false;
            
            }
        }

        private void UpdataEPC(string epc, string tid,string user, string rssi, string count, string ant)
        {
            bool[] exist = new bool[1];
            int id = CheckUtils.getInsertIndex(epcList, epc, exist);
            if (exist[0])
            {
                lvEPC.Items[id].SubItems[2].Text = tid;
                lvEPC.Items[id].SubItems[3].Text = user;
                lvEPC.Items[id].SubItems[4].Text = rssi;
                lvEPC.Items[id].SubItems[5].Text = (int.Parse(lvEPC.Items[id].SubItems[5].Text) + int.Parse(count)).ToString();
                lvEPC.Items[id].SubItems[6].Text = ant;
            }
            else
            {
                total++;
                ListViewItem lv = new ListViewItem();
                int index = lvEPC.Items.Count + 1;
                lv.Text = index.ToString();
                lv.SubItems.Add(epc);
                lv.SubItems.Add(tid);
                lv.SubItems.Add(user);
                lv.SubItems.Add(rssi);
                lv.SubItems.Add(count);
                lv.SubItems.Add(ant);
                lvEPC.Items.Insert(id, lv);
                lblTotal.Text = total.ToString();
                epcList.Insert(id, new EpcInfo(epc, int.Parse(count), DataConvert.HexStringToByteArray(epc)));
            }
            lblTime.Text = ((System.Environment.TickCount - beginTime) / 1000) + "(s)";
        }

        private void GetRemoteIP(string ip)
        {
            textBox1.Text = ip;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                char[] port = textBox2.Text.ToCharArray();
                for (int k = 0; k < port.Length; k++)
                {
                    if (port[k] != '0' && port[k] != '1' && port[k] != '2' && port[k] != '3' && port[k] != '4' &&
                        port[k] != '5' && port[k] != '6' && port[k] != '7' && port[k] != '8' && port[k] != '9')
                    {
                        textBox2.Text = "";
                        return;
                    }
                }
            }
              
        }

      
        private void cmbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMode.SelectedIndex == 0) {
                panel2.Visible = false;
                panel1.Visible = true;
                UdpReceive udpReceive =new UdpReceive();
                udpReceive.ReceiveDataDelegate = ReceiveData;
                iAutoReceive = udpReceive;
            }
            else if (cmbMode.SelectedIndex == 1) {
                panel1.Visible = false;
                panel2.Visible = true;
                SerialPortReceive serialPortReceive = new SerialPortReceive();
                serialPortReceive.ReceiveDataDelegate = ReceiveData;
                iAutoReceive = serialPortReceive;
            }
        }

        private void ReceiveData(byte[] receiveBytes)
        {
            if (receiveBytes != null)
            {
                Console.WriteLine(DataConvert.ByteArrayToHexString(receiveBytes));

                for (int k = 0; k < receiveBytes.Length; k++)
                {
                    uhfOriginalData[++wIndex] = receiveBytes[k];
                    if (wIndex == max - 1)
                    {
                        //达到上限下标重置
                        wIndex = 0;
                    }
                }
                if ((iAutoReceive as UdpReceive) != null) {
                    this.BeginInvoke(RemotelyIPCallback, new object[] { (iAutoReceive as UdpReceive).GetRemoteIP().Address.ToString() });
                }
 

                // Console.Out.Write("刷新ui的\n"); 
            }
        }
    }
}
