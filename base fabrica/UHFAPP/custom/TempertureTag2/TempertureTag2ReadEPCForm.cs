using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using UHFAPP.utils;
using UHFAPP.custom.TempertureTag2;
using BLEDeviceAPI;
using WinForm_Test;

namespace UHFAPP
{
    public partial class TempertureTag2ReadEPCForm : BaseForm
    {
        TempertureTag2 tempertureTag = new TempertureTag2(); 
        TempertureTag2MainForm mainform;
        string strStart = "盘点温度";
        string strStart2 = "盘点温度";
        string strStop = "停止盘点";
        string strStop2 = "停止盘点";
        bool isRuning = false;
        bool isComplete = true;
        long beginTime = 0;
        int total = 0;
        List<EpcInfo> epcList = new List<EpcInfo>();
        // 将text更新的界面控件的委托类型
        delegate void SetTextCallback(TempertureInfo taginfo);
        SetTextCallback setTextCallback;
        public TempertureTag2ReadEPCForm()
        {
            InitializeComponent();

        }
        public TempertureTag2ReadEPCForm(bool isOpen)
        {
            InitializeComponent();
            if (isOpen)
            {
                panel1.Enabled = true;
            }
            else {
                panel1.Enabled = false;
            }
        }
        public TempertureTag2ReadEPCForm(bool isOpen, TempertureTag2MainForm mainform)
        {
            InitializeComponent();
            if (isOpen)
            {
                panel1.Enabled = true;
            }
            else {
                panel1.Enabled = false;
            }
            this.mainform = mainform;
        }
        
        void MainForm_eventOpen(bool open)
        {
            if (open)
            {
                panel1.Enabled = true;
                button11_Click(null,null);
            }
            else
            {
                panel1.Enabled = false;
                if (btnScanEPC.Text == strStop)
                {
                    StopEPC(true);
                }
            }
        }
        void MainForm_eventSwitchUI()
        {
            if (Common.isEnglish)
            {
                contextMenuStrip1.Items[0].Text = "Copy Tag";
                groupBox8.Text = "Filter";
                label29.Text = "Data:";
                label30.Text = "Ptr:";
                lto.Text = "Tag Count:";
                label7.Text = "Total:";
                label2.Text = "Time:";
                button1.Text = "Clear";
                label1.Text = "len:";
               
                if (btnScanEPC.Text == strStart2)
                {
                    btnScanEPC.Text = strStart;
                }
                else if (btnScanEPC.Text == strStop2)
                {
                    btnScanEPC.Text = strStop;
                }

              
               // label1.Location = new Point(785, 34); 
            }
            else
            {
                contextMenuStrip1.Items[0].Text = "复制标签";
                groupBox8.Text = "过滤";
                label29.Text = "数据:";
                //label30.Text = "起始地址:";
      
                lto.Text = "标签数:";
                label7.Text = "次数:";
                label2.Text = "时间:";
                button1.Text = "清空";
                //label1.Text = "长度:";
            
                if (btnScanEPC.Text == strStart)
                {
                    btnScanEPC.Text = strStart2;
                }
                else if (btnScanEPC.Text == strStop)
                {
                    btnScanEPC.Text = strStop2;
                }
              //  label30.Location = new Point(640, 33);
              //  label1.Location = new Point(801, 33);
            }
        }
        

        private void ScanEPCForm_Load(object sender, EventArgs e)
        {
            TempertureTag2MainForm.eventOpen += MainForm_eventOpen;
            setTextCallback = new SetTextCallback(UpdataEPC);

            TempertureTag2MainForm.eventSwitchUI += MainForm_eventSwitchUI;
             MainForm_eventSwitchUI();

             if (mainform.isOpen)
             {
                 panel1.Enabled = true;
                 button11_Click(null, null);
             }
     
        }

        private void ScanEPCForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm2.eventOpen -= MainForm_eventOpen;
  
            if (btnScanEPC.Text == strStop || btnScanEPC.Text == strStop2)
            {
                StopEPC(true);
            }
        }
        #region  设置过滤

  
        private void txtData_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txtData);
            string data = txtData.Text.Replace(" ", "");
            if (data.Length > 0)
            {
                label5.Text = ((data.Length / 2) + ((data.Length % 2) == 0 ? 0 : 1)).ToString();  // txtRead_Length.Text = ((data.Length / 4) + ((data.Length % 4) == 0 ? 0 : 1)).ToString();
            }
            else
            {
                label5.Text = "0";
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
        private void txtPtr_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string ptr = txtPtr.Text;
                if (!StringUtils.IsNumber(ptr))
                {
                    if (rbEPC.Checked)
                    {
                        txtPtr.Text = "32";
                    }
                    else
                    {
                        txtPtr.Text = "0";
                    }
                    return;
                }

            }
            catch (Exception ex)
            {
                if (rbEPC.Checked)
                {
                    txtPtr.Text = "32";
                }
                else
                {
                    txtPtr.Text = "0";
                }
            }
        }
        #endregion
        //start
        private void btnScanEPC_Click(object sender, EventArgs e)
        {


            if (btnScanEPC.Text == strStop || btnScanEPC.Text == strStop2)
            {
                StopEPC(true);
                string msg = Common.isEnglish ? "wait..." : "正在停止...";
                frmWaitingBox f = new frmWaitingBox((obj, args) =>
                {
                    Thread.Sleep(1000);

                }, msg);
                f.ShowDialog(this);
            }
            else
            {
                if (!isRuning && isComplete)
                {
                    mainform.disableControls();
                    isRuning = true;
                    isComplete = false;
                    if (uhf.Inventory())
                    {
                      
                        StartEPC();
                    }
                    else
                    {
                        MessageBoxEx.Show(this, "Inventory failure!");
                        isRuning = false;
                        isComplete = true;
                        mainform.enableControls();
                    }
                }
            }
         
        }
      

  
        //Clear
        private void button1_Click(object sender, EventArgs e)
        {
            tempCount = 0;
            label6.Text = "0";
            epcList.Clear();
            lvEPC.Items.Clear();
            lblTime.Text = "0";
            lblTotal.Text = "0";
            total = 0;
            beginTime = System.Environment.TickCount;
        }

        //开始读取epc
        private void StartEPC() {
            groupBox8.Enabled = false;
            btnScanEPC.Text = Common.isEnglish ? strStop : strStop2;
            new Thread(new ThreadStart(delegate { ReadEPC(); })).Start();

            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
            groupBox4.Enabled = false;
            groupBox5.Enabled = false;
            gbInventoryMode.Enabled = false;
           
        }
        //停止读取epc
        private void StopEPC(bool isStop)
        {

            bool reuslt = uhf.StopGet();
            if (!reuslt)
            {
                MessageBox.Show("停止失败");
            }
            Thread.Sleep(50);
            isRuning = false;
            groupBox8.Enabled = true;
            btnScanEPC.Text = Common.isEnglish ? strStart : strStart2;
            mainform.enableControls();

            groupBox2.Enabled = true;
            groupBox3.Enabled = true;
            groupBox4.Enabled = true;
            groupBox5.Enabled = true;
            gbInventoryMode.Enabled = true;
        }

        //获取epc
        private void ReadEPC()
        {
            try
            {
                beginTime = System.Environment.TickCount;
                while (isRuning)
                {

                    TempertureInfo result = tempertureTag.uhfGetReceivedTempertureInfo();
                    if (result!=null)
                    {
                        this.BeginInvoke(setTextCallback, new object[] { result });
                    }
                }
            }
            catch (Exception ex)
            {

            }
            isComplete = true;
         
        }

        int tempCount = 0;
        StringBuilder sb = new StringBuilder(100);
        private void UpdataEPC(TempertureInfo info)
        {
            string count = "1";
            UHFTAGInfo uhfinfo = info.UhfTagInfo;
            long time = System.Environment.TickCount - beginTime;
            lblTime.Text = (time) + "ms"; ;// (System.Environment.TickCount - beginTime) + "ms";//((System.Environment.TickCount - beginTime) / 1000) + "(s)";// sb.ToString();//
            label6.Text = (tempCount += int.Parse(count)).ToString();

            bool[] exist = new bool[1];
            int index = CheckUtils.getInsertIndex(epcList, uhfinfo.Epc, exist);
            if (exist[0])
            {
                lvEPC.Items[index].SubItems[2].Text = info.UhfTagInfo.Tid;
                lvEPC.Items[index].SubItems[3].Text = info.Temperture ;
                lvEPC.Items[index].SubItems[4].Text = info.Time;
                lvEPC.Items[index].SubItems[5].Text = uhfinfo.Rssi;
                lvEPC.Items[index].SubItems[6].Text = (int.Parse(lvEPC.Items[index].SubItems[6].Text) + int.Parse(count)).ToString();
                lvEPC.Items[index].SubItems[7].Text = uhfinfo.Ant;
            }else{
                total++;
                ListViewItem lv = new ListViewItem();
                lv.Text = (lvEPC.Items.Count+1).ToString();
                lv.SubItems.Add(uhfinfo.Epc);
                lv.SubItems.Add(uhfinfo.Tid);
                lv.SubItems.Add(info.Temperture);
                lv.SubItems.Add(info.Time);
                lv.SubItems.Add(uhfinfo.Rssi);
                lv.SubItems.Add(count);
                lv.SubItems.Add(uhfinfo.Ant);
                lvEPC.Items.Add(lv);
                epcList.Add(new EpcInfo(uhfinfo.Epc, int.Parse(count), DataConvert.HexStringToByteArray(uhfinfo.Epc)));
                lblTotal.Text = epcList.Count + "";
            }
           
     

        }

  
 

        private void lvEPC_DoubleClick(object sender, EventArgs e)
        {
            if (lvEPC.SelectedItems.Count <= 0) {
                return;
            }
            if (btnScanEPC.Text == strStop || btnScanEPC.Text == strStop2)
            {
                StopEPC(true);
            }
            string tag = lvEPC.SelectedItems[0].SubItems[1].Text;
            Common.tag = tag;
            mainform.ReadWriteTag(tag);
        }
 

        private void contextMenuStrip1_Click(object sender, EventArgs e)
        {
            if (lvEPC.SelectedItems.Count <= 0)
            {
                return;
            }
            string str = lvEPC.SelectedItems[0].SubItems[1].Text;
            Clipboard.SetDataObject(str);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string ptr = txtfilerLen.Text;
                if (!StringUtils.IsNumber(ptr))
                {
                    txtfilerLen.Text = "0";
                    return;
                }
            }
            catch (Exception ex)
            {
                txtfilerLen.Text = "0";
            }
        }

        private void rbTID_Click(object sender, EventArgs e)
        {
            txtPtr.Text = "0";
        }

        private void rbUser_Click(object sender, EventArgs e)
        {
            txtPtr.Text = "0";
        }

        private void rbEPC_Click(object sender, EventArgs e)
        {
            txtPtr.Text = "32";
        }
 
        bool isDelete = false;
        private void ReadEPCForm_KeyDown(object sender, KeyEventArgs e)
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

  

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = (System.Environment.TickCount - beginTime) + "(ms)";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            byte filter_bank = 1;
            if (rbTID.Checked)
            {
                filter_bank = 2;
            }
            else if (rbUser.Checked)
            {
                filter_bank = 3;
            }
            byte filter_addr = byte.Parse(txtPtr.Text);
            byte filter_len = byte.Parse(txtfilerLen.Text);
            byte[] filter_data = DataConvert.HexStringToByteArray("00");
            if (filter_len > 0)
            {
                filter_data = DataConvert.HexStringToByteArray(txtData.Text);
            }


            float min_temp = float.Parse(txtMin.Text);
            float max_temp = float.Parse(txtMax.Text);
            int work_delay = int.Parse(txtdelay.Text);
            int work_interval = int.Parse(txtinterval.Text);

            
            if (tempertureTag.StartLogging(filter_bank, filter_addr, filter_len, filter_data, min_temp, max_temp, work_delay, work_interval))
            {
                showMessage("成功 !");
            }
            else
            {
                showMessage("失败!");  
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            byte filter_bank = 1;
            if (rbTID.Checked)
            {
                filter_bank = 2;
            }
            else if (rbUser.Checked)
            {
                filter_bank = 3;
            }
            byte filter_addr = byte.Parse(txtPtr.Text);
            byte filter_len = byte.Parse(txtfilerLen.Text);
            byte[] filter_data = DataConvert.HexStringToByteArray("00");
            if (filter_len > 0)
            {
                filter_data = DataConvert.HexStringToByteArray(txtData.Text);
            }
            byte[] data = DataConvert.HexStringToByteArray(txtPwd.Text);
            int password = data[3];
            password = (data[2] << 8) | password;
            password = (data[1] << 16) | password;
            password = (data[0] << 24) | password;
            //txtPwd
            if (tempertureTag.StopLogging(filter_bank, filter_addr, filter_len, filter_data, password))
            {
                showMessage("停止成功!");  

            }
            else
            {
                showMessage("停止失败!");
   
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

            string filerData = txtData.Text.Replace(" ", "");
        

            //过滤----------------------------------
            int filerBank = 1;
            byte[] filerBuff = DataConvert.HexStringToByteArray(filerData);
            int filerPtr = int.Parse(txtPtr.Text);
            int filerLen = int.Parse(txtfilerLen.Text);

            if ((filerLen / 8 + (filerLen % 8 == 0 ? 0 : 1)) * 2 > filerData.Length)
            {
                MessageBox.Show(Common.isEnglish ? "filter data length error!" : "过滤数据和长度不匹配!");  //to do
                return;
            }

            if (rbTID.Checked)
                filerBank = 2;
            if (rbUser.Checked)
                filerBank = 3;
            //-----------------------------------------

            bool result = uhf.InitRegFile((byte)filerBank, filerPtr, filerLen, filerBuff);
            string msg = "";
            int time = 500;
            if (result)
            {
                time = 100;
                msg = Common.isEnglish ? " success!" : "初始化成功!";
            }
            else
            {
                msg = Common.isEnglish ? " failure!" : "初始化失败!";
            }

            frmWaitingBox f = new frmWaitingBox((obj, args) =>
            {
                System.Threading.Thread.Sleep(time);
            }, msg);
            f.ShowDialog(this);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string filerData = txtData.Text.Replace(" ", "");
        

            //过滤----------------------------------
            int filerBank = 1;
            byte[] filerBuff = DataConvert.HexStringToByteArray(filerData);
            int filerPtr = int.Parse(txtPtr.Text);
            int filerLen = int.Parse(txtfilerLen.Text);

            if ((filerLen / 8 + (filerLen % 8 == 0 ? 0 : 1)) * 2 > filerData.Length)
            {
                MessageBox.Show(Common.isEnglish ? "filter data length error!" : "过滤数据和长度不匹配!");  //to do
                return;
            }

            if (rbTID.Checked)
                filerBank = 2;
            if (rbUser.Checked)
                filerBank = 3;
            //-----------------------------------------
            float[] outtemp = new float[10];
            bool result = uhf.ReadTagTemperature((byte)filerBank, filerPtr, filerLen, filerBuff, outtemp);
            if (result)
            {
                label39.Text = "温度:" + outtemp[0] + "℃";
            }
            else
            {
                string msg = Common.isEnglish ? "failure!" : "失败!";
                showMessage(msg);
                label39.Text = "温度:--";
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int mode = cbInventoryMode.SelectedIndex;
            bool result = false;
 
            switch (mode)
            {
                case 0:
                    result = uhf.setEPCMode(false);
                    break;
                case 1:
                    result = tempertureTag.setEPCAndTemperature();
                    break;
            }

            if (!result)
            {
                string msg = Common.isEnglish ? "failure!" : "设置失败!";
                showMessage(msg);
                return;
            }
            else
            {
                string msg = Common.isEnglish ? "success!" : "设置成功!";
                showMessage(msg);
            }
        }
        private void showMessage(string msg)
        {
            frmWaitingBox f = new frmWaitingBox((obj, args) =>
            {
                System.Threading.Thread.Sleep(500);
            }, msg);
            f.ShowDialog(this);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            byte userPtr = 0;
            byte userLen = 0;
            int mode = uhf.getEPCTIDUSERMode(ref userPtr, ref userLen);
            switch (mode)
            {
                case 0:
                    cbInventoryMode.SelectedIndex = 0;
                    break;
                case 3:
                    cbInventoryMode.SelectedIndex = 1;
                    break;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
             //过滤----------------------------------
            string filerData = txtData.Text.Replace(" ", "");
            int filerBank = 1;
            byte[] filerBuff = DataConvert.HexStringToByteArray(filerData);
            int filerPtr = int.Parse(txtPtr.Text);
            int filerLen = int.Parse(txtfilerLen.Text);

            if ((filerLen / 8 + (filerLen % 8 == 0 ? 0 : 1)) * 2 > filerData.Length)
            {
                MessageBox.Show(Common.isEnglish ? "filter data length error!" : "过滤数据和长度不匹配!");  //to do
                return;
            }

            if (rbTID.Checked)
                filerBank = 2;
            if (rbUser.Checked)
                filerBank = 3;
            //-----------------------------------------
            int result =  tempertureTag.CheckOpMode((byte)filerBank, filerPtr, filerLen, filerBuff);

            if (result > 0)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("状态:(0x" + DataConvert.DecimalToHexString(result) + ")\r\n");
                if ((result & 1) == 1 || (result & 2) == 2 || (result & 4) == 4 || (result & 8) == 8 ||
                    (result & 16) == 16 || (result & 32) == 32 || (result & 64) == 64 || (result & 128) == 128 ||
                    (result & 1024) == 1024 || (result & 16384) == 16384 || (result & 32768) == 32768)
                {
                   // msg.Append("RFU\r\n"); 
                }
                if ((result & 256) == 256)
                {
                    msg.Append("电池电压高于0.9V\r\n");  
                }
                if ((result & 512) == 512)
                {
                    msg.Append("光强超过预设值\r\n"); 
                }
                if ((result & 2048) == 2048)
                {
                    msg.Append("一次测温流程被打断\r\n");  
                }
                if ((result & 4096) == 4096)
                {
                    msg.Append("当前处于rtc测温流程\r\n");  
                }
                if ((result & 8192) == 8192)
                {
                   // msg.Append("当前用户权限无效");  
                }
                //frmWaitingBox f = new frmWaitingBox((obj, args) =>
                //{
                //    System.Threading.Thread.Sleep(4000);
                //}, msg.ToString());
                //f.ShowDialog(this);
                MessageBoxEx.Show(msg.ToString());

            }
            else {
                string msg = "返回：" + result;
                showMessage(msg);
            }
            
 
        }
       

     

    }
}
