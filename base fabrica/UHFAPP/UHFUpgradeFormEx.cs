using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace UHFAPP
{
    public partial class UHFUpgradeFormEx : BaseForm
    {
        string path = "";
        public UHFUpgradeFormEx(bool isEnglish)
        {
            InitializeComponent();
            if (isEnglish)
            {
                label1.Text = "path:";
                btnPath.Text = "Select file";
                btnStart.Text = "Upgrade";
            }
            else
            {
                label1.Text = "路径:";
                btnPath.Text = "选择文件";
                btnStart.Text = "升级";
            }
          
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "bin|*.bin";
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                // 显示文件路径名
                txtPath.Text = openDlg.FileName;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            path = txtPath.Text;
            if (path == null || path.Length == 0)
            {
                MessageBox.Show("fail");
                return;
            }
            if (!rbReaderApplication.Checked && !rbUHFModule.Checked && !rbReaderBootloader.Checked && !rbEx10SDKFirmware.Checked)
            {
                MessageBox.Show("fail");
                return;
            }

            btnPath.Enabled = false;
            btnStart.Enabled = false;
            this.ControlBox = false;
            new Thread(new ThreadStart(startUpdate)).Start();
        }
        
        private void startUpdate()
        {
            setMsg("Updating......", true);

            FileStream stream = null;
            BinaryReader binary = null;
            byte type = 255;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                setPprogress(0);
                stream = new FileStream(path, FileMode.Open);
                binary = new BinaryReader(stream);

                long uFileSize = stream.Length;
                int packageCount = (int)(uFileSize / 64) + (uFileSize % 64 > 0 ? 1 : 0);
                string strversion = "";

                this.Invoke(new EventHandler(delegate {
                    if (rbReaderApplication.Checked)
                    {
                        type = 0;
                        strversion = "uhf version:" + uhf.GetSTM32Version();
                        this.Invoke(new EventHandler(delegate
                        {
                            label2.Text = strversion;
                        }));
                    }
                    else if (rbUHFModule.Checked)
                    {
                        type = 1;
                        strversion = "uhf version:" + uhf.GetSoftwareVersion();
                        this.Invoke(new EventHandler(delegate
                        {
                            label2.Text = strversion;
                        }));
                    }
                    else if (rbReaderBootloader.Checked)
                    {
                        type = 2;
                    }
                    else if (rbEx10SDKFirmware.Checked)
                    {
                        type = 3;
                    }
                }));
                

                if (!uhf.jump2Boot(type))
                {
                    setMsg("uhfJump2Boot fail",true);
                    return;
                }
                Thread.Sleep(2000);



                if (MainForm.MODE==1)
                {
                    if (type != 1)
                    {
                        setMsg("断开连接", true);
                        uhf.TcpDisconnect();
                        Thread.Sleep(1000);
                        setMsg("开始连接", true);
                        bool result = uhf.TcpConnect(MainForm.ip, MainForm.portData);
                        if (!result)
                        {
                            setMsg("TcpConnect fail", true);
                            return;
                        }
                        setMsg("连接成功!", true);
                        Thread.Sleep(1000);
                    }
                }
                else if (MainForm.MODE == 2)
                {
                   // uhf.Close();
                   // Thread.Sleep(1000);
                   // uhf.Open();
                }

                if (!uhf.startUpd())
                {
                    setMsg("uhfStartUpdate fail", true);
                    return;
                }
                Thread.Sleep(2000);
                for (int k = 0; k < packageCount; k++)
                {
                    try
                    {
                        byte[] data = binary.ReadBytes(64);
                        setMsg("uhfUpdating  packageCount=" + packageCount + "       " + k, false);
                        if (uhf.updating(data, data.Length))
                        {
                            double r = Math.Round(((double)(k + 1) / (double)packageCount), 2) * 100;
                            setPprogress((int)r);
                        }
                        else
                        {
                            setMsg("uhfUpdating 失败 ,package=" + k, true);
                            uhf.stopUpdate();
                            return;
                        }
                        Thread.Sleep(5);
                    }
                    catch (Exception e)
                    {
                        setMsg("ex=" + e.Message,true);
                    }

                }
                setPprogress(100);

            }
            catch (Exception ex)
            {
                setMsg("ex=" + ex.Message,true);
            }
            finally
            {
                try
                {
                    if (uhf.stopUpdate())
                    {
                        setMsg("升级完成!", true);
                    }
                    else
                    {
                        setMsg("升级失败!", true);
                    }
                    Thread.Sleep(2000);

                    btnPath.Invoke(new EventHandler(delegate
                    {
                        btnPath.Enabled = true;
                        btnStart.Enabled = true;
                        ControlBox = true;
                    }));

                    getVersion(type);
                    if (binary != null)
                    {
                        binary.Close();
                    }
                    if (stream != null)
                    {
                        stream.Close();
                    }
                    Cursor.Current = Cursors.Default;
                }
                catch (Exception e)
                {
                    setMsg("222 ex=" + e.Message, true);
                }
               
            }

            

        }

        private void setPprogress(int progress)
        {
            progressBar1.Invoke(new EventHandler(delegate
            {
                progressBar1.Value = progress;
            }));
        }

      
        private void setMsg(string msg,bool isAppend)
        {
            textBox1.Invoke(new EventHandler(delegate
            {
                if (isAppend)
                {
                    if (textBox1.Text.Length > 2000)
                    {
                        textBox1.Text = msg;
                    }
                    else
                    {
                        textBox1.AppendText("\r\n");
                        textBox1.AppendText(msg);
                      
                    }
                }
                else
                {
                    textBox1.Text = msg;
                }
            }));
        }

        private void getVersion(byte type)
        {

            if (type == 255)
            {
                return;
            }
 
            label2.Invoke(new EventHandler(delegate
            {
                if (type==1)
                {
                    MessageBox.Show("uhf version:" + uhf.GetSoftwareVersion());
                    label2.Text = "uhf version:" + uhf.GetSoftwareVersion();
                }
                else
                {
                    if (MainForm.MODE == 1)
                    {
                        if (type != 1)
                        {
                            setMsg("断开连接", true);
                            uhf.TcpDisconnect();
                            Thread.Sleep(1000);
                            setMsg("开始连接", true);
                            bool result = uhf.TcpConnect(MainForm.ip, MainForm.portData);
                            if (!result)
                            {
                                setMsg("TcpConnect fail", true);
                                return;
                            }
                        }
                    }

                    MessageBox.Show("uhf version:" + uhf.GetSTM32Version());
                    label2.Text = "uhf version:" + uhf.GetSTM32Version();
                }
            }));
           
        }

        void MainForm_eventOpen(bool open)
        {
             
        }

        private void UHFUpgradeForm_Load(object sender, EventArgs e)
        {
             
        }

       
    }
}
