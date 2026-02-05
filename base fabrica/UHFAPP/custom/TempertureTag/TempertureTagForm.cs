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
using System.Collections;
using UHFAPP.multidevice;

namespace UHFAPP
{
    public partial class TempertureTagForm : Form
    {
        UHFAPI_RFMicronMagnus_S3 uhf = UHFAPI_RFMicronMagnus_S3.getInstance();
        MainForm2 mainform;
        int total = 0;
        bool isDelete = false;
        bool isRuning = false;
        bool isComplete = true;
        int beginTime = 0;
        Hashtable epcList = new Hashtable();
        // 将text更新的界面控件的委托类型
        delegate void SetTextCallback(string  epc, string  calibrationData, string  sensorCode , string  rssiCode, string  tempeCode,string  rssi, string  ant,string count);//(string epc, string tid, string rssi, string count, string ant);
        SetTextCallback setTextCallback;

        //0  CalibrationData+SensorCode+ On-ChipRSSI+TempeCode
        //1  On-ChipRSSI+ TempeCode
        int mode = 0;


        public TempertureTagForm()
        {
            InitializeComponent();
        }
        public TempertureTagForm(bool isOpen, MainForm2 mainform)
        {
            this.mainform = mainform;
            InitializeComponent();     
        }
        private void ConfigForm2_Load(object sender, EventArgs e)
        {
            cmbAnt.SelectedIndex = 0;
            cmbPower.SelectedIndex = 19;

            cmbWAnt.SelectedIndex = 0;
            cmbWPower.SelectedIndex = 19;
            cmbMode.SelectedIndex = 0;


            cmbAnt_2.SelectedIndex = 0;
            cmbPower_2.SelectedIndex = 19;

            rbSensorCode.Checked = true;
            label2.Text = "0";

            MainForm2.eventOpen += MainForm_eventOpen;
            setTextCallback = new SetTextCallback(UpdataEPC);

            MainForm2.eventSwitchUI += MainForm_eventSwitchUI;
            MainForm_eventSwitchUI();

          //  uhf.SetDebug(true);
        }
        private void btnRead_Click(object sender, EventArgs e)
        {
            string epc = txtEPC.Text.Replace(" ", "");

            if (epc.Length != 32)
            {
                MessageBox.Show("请输入16个字节的EPC数据!");
                return;
            }

            int ant = cmbAnt.SelectedIndex + 1;
            int power = cmbPower.SelectedIndex + 1;
            byte[] buffEPC = DataConvert.HexStringToByteArray(epc);

            string data = "";
            if (rbSensorCode.Checked)
            {
                data = UHFAPI_RFMicronMagnus_S3.getInstance().ReadSensorCode(buffEPC, ant, power);
            }
            else if (rbRssi.Checked)
            {
                data = UHFAPI_RFMicronMagnus_S3.getInstance().ReadOnChipRSSI(buffEPC, ant, power);
            }
            else if (rbTempertureCode.Checked)
            {
                data = UHFAPI_RFMicronMagnus_S3.getInstance().ReadTempertureCode(buffEPC, ant, power);
            }
            else if (rbCalibrationData.Checked)
            {
                data = UHFAPI_RFMicronMagnus_S3.getInstance().ReadCalibrationData(buffEPC, ant, power);
            }
            else if (rbRssiTempCode.Checked)
            {
                data = UHFAPI_RFMicronMagnus_S3.getInstance().ReadOnChipRSSIAndTempCode(buffEPC, ant, power);
            }
            else if (rbOnChipRSSI_TempCode_CalibrationData.Checked)
            {
                data = UHFAPI_RFMicronMagnus_S3.getInstance().ReadOnChipRSSI_TempCode_CalibrationData(buffEPC, ant, power);
            }
            string msg = "";
            int time = 500;
            if (data == null || data.Length == 0)
            {
                msg = Common.isEnglish ? "Read failure!" : "读取数据失败!";
                txtData.Text = "";
            }
            else
            {
                time = 100;
                txtData.Text = data;
                msg = Common.isEnglish ? "Read success!" : "读取数据成功!";
            }

            frmWaitingBox f = new frmWaitingBox((obj, args) =>
            {
                System.Threading.Thread.Sleep(time);
            }, msg);
            f.ShowDialog(this);
        }

        private void txtEPC_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txtEPC);
            string data = txtEPC.Text.Replace(" ", "");
            if (data.Length > 0)
            {
                label2.Text = ((data.Length / 2) + ((data.Length % 2) == 0 ? 0 : 1)).ToString();  // txtRead_Length.Text = ((data.Length / 4) + ((data.Length % 4) == 0 ? 0 : 1)).ToString();
            }
            else
            {
                label2.Text = "0";
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

       
        private void ConfigForm2_KeyDown(object sender, KeyEventArgs e)
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

        private void disableControls() {
            groupBox1.Enabled = false;
            cmbAnt_2.Enabled = false;
            cmbPower_2.Enabled = false;
            cmbMode.Enabled = false;
            groupBox3.Enabled = false;
            mainform.disableControls();

        }
        private void enableControls() {
            groupBox1.Enabled = true;
            cmbAnt_2.Enabled = true;
            cmbPower_2.Enabled = true;
            cmbMode.Enabled = true;
            groupBox3.Enabled = true;
            mainform.enableControls();
        }
        private void btnScanEPC_Click(object sender, EventArgs e)
        {
            if (btnScanEPC.Text == "Stop")
            {
                StopEPC(true);
            }
            else
            {
                mode = cmbMode.SelectedIndex;
                int power = cmbPower_2.SelectedIndex + 1;
                int ant = cmbAnt_2.SelectedIndex + 1;

                if (!isRuning && isComplete)
                {
                    disableControls();
                    isRuning = true;
                    isComplete = false;

                    bool reault = false;// reault = uhf.InventoryTempTag(ant, power);
                    if (mode == 0)
                    {
                         reault = uhf.InventoryTempTag(ant, power);
                    }
                    else if (mode == 1)
                    {
                        reault = uhf.InventoryTempTag_OnChipRSSI_TempeCode(ant, power);
                    }
                    else if (mode == 2) {
                        reault = uhf.PerformInventory(ant, power);
                    }
                    
                    if (reault)
                    {
                        StartEPC();
                    }
                    else
                    {
                        MessageBoxEx.Show(this, "Inventory failure!");
                        isRuning = false;
                        isComplete = true;
                    }
                }
            }
        }

      

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
        private void StartEPC()
        {
            btnScanEPC.Text = "Stop";
            new Thread(new ThreadStart(delegate { ReadEPC(); })).Start();

        }
        //停止读取epc
        private void StopEPC(bool isStop)
        {
            UHFAPI_RFMicronMagnus_S3.getInstance().StopGet();
            isRuning = false;
            btnScanEPC.Text = "Start";
            enableControls();
        }

        //获取epc
        private void ReadEPC()
        {
            try
            {
                beginTime = System.Environment.TickCount;
                while (isRuning)
                {
                    string epc = "";
                    string rssi = "";

                    string calibrationData = "";
                    string sensorCode = "";
                    string rssiCode = "";
                    string tempeCode = "";
                    int ant = 0;

                    bool result = false;
                    if (mode == 0)
                    {
                        result=uhf.uhfGetTempTagReceived(ref  epc, ref  calibrationData, ref  sensorCode, ref  rssiCode, ref  tempeCode, ref  rssi, ref  ant); 
                    }
                    else if(mode ==1)
                    {
                        result = uhf.uhfGetTempTagReceived_OnChipRSSI_TempeCode(ref  epc,  ref  rssiCode, ref  tempeCode, ref  rssi, ref  ant);
                    }
                    else if (mode == 2)
                    {
                        //SensorCode+On-ChipRSSI
                        TagInfo tagInfo = uhf.getTagData();
                        if (tagInfo != null && tagInfo.UhfTagInfo != null)
                        {
                            ant = int.Parse(tagInfo.UhfTagInfo.Ant);
                            epc = tagInfo.UhfTagInfo.Epc;
                            sensorCode = tagInfo.UhfTagInfo.Sensor;
                            rssi = tagInfo.UhfTagInfo.Rssi;
                            result = true;
                        }
                       
    
                    }

                    if (result)
                    {

                        this.BeginInvoke(setTextCallback, new object[] { epc, calibrationData, sensorCode, rssiCode, tempeCode, rssi, ant + "", "1" });
                    }
                    else
                    {
                        Thread.Sleep(10);
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
        private void UpdataEPC(string epc, string calibrationData, string sensorCode, string rssiCode, string tempeCode, string rssi, string ant, string count)
        {
            long time = System.Environment.TickCount - beginTime;
            lblTime.Text = (time) + "ms";  
            label6.Text = (tempCount += int.Parse(count)).ToString();
            if (epcList[epc] != null)
            {
                for (int i = 0; i < lvEPC.Items.Count; i++)
                {
                    if (this.lvEPC.Items[i].SubItems[1].Text == epc)
                    {
                        lvEPC.Items[i].SubItems[2].Text = "tid";
                        lvEPC.Items[i].SubItems[3].Text = calibrationData;
                        lvEPC.Items[i].SubItems[4].Text = sensorCode;
                        lvEPC.Items[i].SubItems[5].Text = rssiCode;
                        lvEPC.Items[i].SubItems[6].Text = tempeCode;
                        lvEPC.Items[i].SubItems[7].Text = rssi;
                        lvEPC.Items[i].SubItems[8].Text = (int.Parse(lvEPC.Items[i].SubItems[8].Text) + int.Parse(count)).ToString();
                        lvEPC.Items[i].SubItems[9].Text = ant;
                        break;
                    }
                }
            }
            else
            {
                total++;
                ListViewItem lv = new ListViewItem();
                lv.Text = (lvEPC.Items.Count + 1).ToString();
                lv.SubItems.Add(epc);
                lv.SubItems.Add("tid");


                lv.SubItems.Add(calibrationData);
                lv.SubItems.Add(sensorCode);
                lv.SubItems.Add(rssiCode);
                lv.SubItems.Add(tempeCode);
                lv.SubItems.Add(rssi);
                lv.SubItems.Add(count);
                lv.SubItems.Add(ant);



                lvEPC.Items.Add(lv);
                epcList.Add(epc, count);
                lblTotal.Text = epcList.Count + "";
            }
        }

        private void ConfigForm2_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainForm2.eventOpen -= MainForm_eventOpen;
            MainForm2.eventSwitchUI -= MainForm_eventSwitchUI;
        }

     
        void MainForm_eventOpen(bool open)
        {
            if (open)
            {
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
            }
            else
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                if (btnScanEPC.Text == "Stop")
                {
                    StopEPC(true);
                }
            }
        }
        void MainForm_eventSwitchUI()
        {
            if (Common.isEnglish)
            { }
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

        

        private void btnWrite_Click(object sender, EventArgs e)
        {
            string epc = txtWEPC.Text.Replace(" ","") ;
            string data = txtWData.Text.Replace(" ", "");
            int ant = cmbWAnt.SelectedIndex + 1;
            int power = cmbWPower.SelectedIndex + 1;
            if (epc.Length != 32)
            {
                MessageBox.Show("请输入16个字节的EPC数据!");
                return;
            }
            if (data.Length %2 !=0)
            {
                MessageBox.Show("请输入16进制数据!");
                return;
            }
            byte[] buffEPC = DataConvert.HexStringToByteArray(epc);
            byte[] buffData = DataConvert.HexStringToByteArray(data);

            bool result = UHFAPI_RFMicronMagnus_S3.getInstance().WriteCalibrationData(buffEPC, ant, power, buffData);
            string msg = "";
            int time = 500;
            if (!result)
            {
                msg = Common.isEnglish ? "failure!" : "失败!";
            }
            else
            {
                time = 100;
                msg = Common.isEnglish ? "success!" : "成功!";
            }

            frmWaitingBox f = new frmWaitingBox((obj, args) =>
            {
                System.Threading.Thread.Sleep(time);
            }, msg);
            f.ShowDialog(this);

        }

        private void txtWEPC_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txtWEPC);
            string data = txtWEPC.Text.Replace(" ", "");
            if (data.Length > 0)
            {
                label12.Text = ((data.Length / 2) + ((data.Length % 2) == 0 ? 0 : 1)).ToString();  // txtRead_Length.Text = ((data.Length / 4) + ((data.Length % 4) == 0 ? 0 : 1)).ToString();
            }
            else
            {
                label12.Text = "0";
            }
        }

        private void txtWData_TextChanged(object sender, EventArgs e)
        {
            FormatHex(txtWData);
            string data = txtWData.Text.Replace(" ", "");
            if (data.Length > 0)
            {
                label13.Text = ((data.Length / 2) + ((data.Length % 2) == 0 ? 0 : 1)).ToString();  // txtRead_Length.Text = ((data.Length / 4) + ((data.Length % 4) == 0 ? 0 : 1)).ToString();
            }
            else
            {
                label13.Text = "0";
            }
        }

        private void cmbMode_SelectedValueChanged(object sender, EventArgs e)
        {
            button1_Click(null,null);
        }
    
    }
}
