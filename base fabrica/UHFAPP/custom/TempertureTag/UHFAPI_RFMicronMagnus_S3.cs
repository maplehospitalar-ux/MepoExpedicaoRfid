using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace UHFAPP
{
    public class UHFAPI_RFMicronMagnus_S3:UHFAPI
    {
       

        /**********************************************************************************************************
        * 功能：读取 Sensor Code
        * 输入：epc： EPC号，16个字节

               antNum:  天线号， 1个字节

	           powerValue： 功率值，2个字节

          输出：data， 2个字节

          返回值：2：数据长度    -1：获取失败
        * 
        *********************************************************************************************************/
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int UHFGetSensorCode(byte[] epc, byte antNum, byte[] powerValue, byte[] outData);


        /**********************************************************************************************************
        * 功能：读取 Calibration Data
        * 输入：epc： EPC号，16个字节

               antNum:  天线号， 1个字节

	           powerValue： 功率值，2个字节

          输出：data， 8个字节

          返回值：8：数据长度    -1：获取失败

        * 
        *********************************************************************************************************/
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int UHFGetCalibrationData(byte[] epc, byte antNum, byte[] powerValue, byte[] outData);

 
        /**********************************************************************************************************
        * 功能：读取 On-Chip RSSI
        * 输入：epc： EPC号，16个字节

               antNum:  天线号， 1个字节

	           powerValue： 功率值，2个字节

          输出：data， 2个字节

          返回值：2：数据长度    -1：获取失败

        * 
        *********************************************************************************************************/
      [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
      private extern static int UHFGetOnChipRSSI(byte[] epc, byte antNum, byte[] powerValue, byte[] outData);


        /**********************************************************************************************************
        * 功能：读取 Temperture Code
        * 输入：epc： EPC号，16个字节

               antNum:  天线号， 1个字节

	           powerValue： 功率值，2个字节

          输出：data， 2个字节

          返回值：2：数据长度    -1：获取失败

        * 
        *********************************************************************************************************/
      [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
      private extern static int UHFGetTempertureCode(byte[] epc, byte antNum, byte[] powerValue, byte[] outData);


        /**********************************************************************************************************
        * 功能：读取 On-Chip RSSI+ Temp Code
        * 输入：epc： EPC号，16个字节

               antNum:  天线号， 1个字节

	           powerValue： 功率值，2个字节

          输出：data， 4个字节  ,  RSSI: data[0] data[1]   ,  TempCode: data[2] data[3]  

          返回值：4：数据长度    -1：获取失败

        * 
        *********************************************************************************************************/
      [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
      private extern static int UHFGetOnChipRSSIAndTempCode(byte[] epc, byte antNum, byte[] powerValue, byte[] outData);



        /**********************************************************************************************************
        * 功能：开始   盘点 Calibration Data+ Sensor Code+ On-Chip RSSI+ Tempe Code
        * 输入： 

               antNum:  天线号， 1个字节

	           powerValue： 功率值，2个字节


          返回值：0：发送成功    1：发送失败
        * 
        *********************************************************************************************************/
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int UHFInventoryTempTag(byte antNum, byte[] powerValue);


        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int UHF_GetTempTagReceived(ref int uLenUii, byte[] uUii);
 



        /**********************************************************************************************************
        * 功能：开始   盘点 On-Chip RSSI+ Tempe Code
        * 输入： 

               antNum:  天线号， 1个字节

	           powerValue： 功率值，2个字节


          返回值：0：发送成功    1：发送失败

        * 
        *********************************************************************************************************/
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
         private extern static int UHFInventoryTempTag2(byte antNum, byte[] powerValue);

       [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int UHF_GetTempTagReceived2(ref int uLenUii, byte[] uUii);
 
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
       private extern static int UHFPerformInventory(int mode, byte[] param, int paramlen);

 

        /**********************************************************************************************************
        * 功能：写入 Calibration Data
        * 输入：epc： EPC号，16个字节

               antNum:  天线号， 1个字节

	           powerValue： 功率值，2个字节

               data， Calibration Data , 8个字节

          返回值：0：成功    -1：失败

        * 
        *********************************************************************************************************/
       [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
       private extern static int UHFWriteCalibrationData(byte[] epc,byte antNum,byte[] powerValue,byte[] data);
  
         /**********************************************************************************************************
        * 功能：读取 Calibration Data
        * 输入：mode:功能字
		        epc： EPC号，16个字节
               antNum:  天线号， 1个字节
	           powerValue： 功率值，2个字节

          输出：data， 8个字节
          返回值：8：数据长度    -1：获取失败
        * 
        *********************************************************************************************************/
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int UHFGetCalibrationDataEX(byte mode,byte[] epc,byte antNum,byte[] powerValue,byte[] data);



      
        private static UHFAPI_RFMicronMagnus_S3 uhf = null;
        internal UHFAPI_RFMicronMagnus_S3() { }
        public static UHFAPI_RFMicronMagnus_S3 getInstance()
        {
            if (uhf == null)
                uhf = new UHFAPI_RFMicronMagnus_S3();
            return uhf;
        }

        /// <summary>
        /// 读取 On-Chip RSSI+ Temp Code
        /// </summary>
        /// <param name="epc"></param>
        /// <param name="ant"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public string ReadOnChipRSSIAndTempCode(byte[] epc,int ant,int power)
        {
            int len = 4;
            byte antNum = (byte)ant;
            byte[] powerValue = getPowerToBytes(power);
            byte[] outData = new byte[len];
            len = UHFGetOnChipRSSIAndTempCode(epc, antNum, powerValue, outData);
            if (len != -1)
            {
                string strData = DataConvert.ByteArrayToHexString(outData, len);
                return strData;
            }

            return null;
        }

        /// <summary>
        /// 读取 On-Chip RSSI+ Temp Code+ Calibration Data
        /// </summary>
        /// <param name="epc"></param>
        /// <param name="ant"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public string ReadOnChipRSSI_TempCode_CalibrationData(byte[] epc, int ant, int power)
        {
            int len = 256;
            byte antNum = (byte)ant;
            byte[] powerValue = getPowerToBytes(power);
            byte[] outData = new byte[len];
            len = UHFGetCalibrationDataEX(7,epc, antNum, powerValue, outData);
            if (len != -1)
            {
                string strData = DataConvert.ByteArrayToHexString(outData, len);
                return strData;
            }
            return null;
        }
        /// <summary>
        /// 读取 Sensor Code
        /// </summary>
        /// <param name="epc"></param>
        /// <param name="ant"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public string ReadSensorCode(byte[] epc, int ant, int power)
        {
            int len = 2;
            byte antNum = (byte)ant;
            byte[] powerValue = getPowerToBytes(power);
            byte[] outData = new byte[len];
            len = UHFGetSensorCode(epc, antNum, powerValue, outData);
            if (len!=-1)
            {
                string strData = DataConvert.ByteArrayToHexString(outData, len);
                return strData;
            }

            return null;
        }
        /// <summary>
        /// 读取 Calibration Data
        /// </summary>
        /// <param name="epc"></param>
        /// <param name="ant"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public string ReadCalibrationData(byte[] epc, int ant, int power)
        {
            int len = 8;
            byte antNum = (byte)ant;
            byte[] powerValue = getPowerToBytes(power);
            byte[] outData = new byte[len];
            len=UHFGetCalibrationData(epc, antNum, powerValue, outData);
            if (len!=0)
            {
                string strData = DataConvert.ByteArrayToHexString(outData, len);
                return strData;
            }

            return null;
        }
        /// <summary>
        /// 读取 On-Chip RSSI
        /// </summary>
        /// <param name="epc"></param>
        /// <param name="ant"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public string ReadOnChipRSSI(byte[] epc, int ant, int power)
        {
            int len = 2;
            byte antNum = (byte)ant;
            byte[] powerValue = getPowerToBytes(power);
            byte[] outData = new byte[len];
            len=UHFGetOnChipRSSI(epc, antNum, powerValue, outData);
            if (len!=-1)
            {
                string strData = DataConvert.ByteArrayToHexString(outData, len);
                return strData;
            }

            return null;
        }
        /// <summary>
        /// 读取 Temperture Code
        /// </summary>
        /// <param name="epc"></param>
        /// <param name="ant"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public string ReadTempertureCode(byte[] epc, int ant, int power)
        {
            int len = 2;
            byte antNum = (byte)ant;
            byte[] powerValue = getPowerToBytes(power);
            byte[] outData = new byte[len];
            len=UHFGetTempertureCode(epc, antNum, powerValue, outData);
            if (len!=-1)
            {
                string strData = DataConvert.ByteArrayToHexString(outData, len);
                return strData;
            }

            return null;
        }

        public bool InventoryTempTag(int ant, int power)
        {
            byte antNum = (byte)ant;
            byte[] powerValue = getPowerToBytes(power);

            if (UHFInventoryTempTag(antNum, powerValue) == 0)
            {
                return true;
            }
            return false;
        }
        public bool InventoryTempTag_OnChipRSSI_TempeCode(int ant, int power)
        {
            byte antNum = (byte)ant;
            byte[] powerValue = getPowerToBytes(power);

            if (UHFInventoryTempTag2(antNum, powerValue) == 0)
            {
                return true;
            }
            return false;
        }

        public bool PerformInventory(int ant, int power)
        {
            int mode = 2;
            byte[] p=  new byte[4];
            byte[] powerValue = getPowerToBytes(power);
            p[0] = (byte)3;
            p[1] = (byte)ant;
            p[2] = (byte)powerValue[0];
            p[3] = (byte)powerValue[1];
            if (UHFPerformInventory(mode, p, p.Length) == 0)
            {
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 获取连续盘存标签数据
        /// </summary>
        /// <param name="uLenUii">UII长度</param>
        /// <param name="uUii">UII数据</param>
        /// <returns></returns>
        private bool GetTempTagReceived(ref int uLenUii, ref byte[] uUii)
        {
            if (UHF_GetTempTagReceived(ref uLenUii, uUii) == 0)
            {
                return true;
            }
            return false;
        }
        private bool GetTempTagReceived_OnChipRSSI_TempeCode(ref int uLenUii, ref byte[] uUii)
        {
            if (UHF_GetTempTagReceived2(ref uLenUii, uUii) == 0)
            {
                return true;
            }
            return false;
        }
        //读取epc
        public bool uhfGetTempTagReceived(ref string epc, ref string calibrationData, ref string sensorCode , ref string rssiCode, ref string tempeCode,ref string rssi, ref int ant)
        {
          
                int uLen = 0;
                byte[] bufData = new byte[150];
                if (GetTempTagReceived(ref uLen, ref bufData))//GetTempTagReceived   GetReceived_EX
                {
                    //  uUii = 
                    //  1个字节uii长度+ UII数据+ 
                    //  1个字节TID数据长度+TID数据
                    //  8个字节CalibrationData +
                    //  2个字节Sensor Code + 
                    //  2个字节Rssi code +
                    //  2个字节Tempe code +
                    //  2个字节RSSI +
                    //  1个字节Ant +

                    int uii_len = bufData[0];//uii长度
                    int uii_index = 1;//uii的起始地址
                    int tid_leng = bufData[uii_len + 1];//tid数据长度

                    int tid_idex = uii_len + 1;//tid起始位
                    if (tid_leng > 0)
                    {
                        tid_idex = uii_len + 2;//tid起始位
                    }

                    int calibrationData_index = tid_idex + tid_leng + 1;
                    int sensorcode_index = calibrationData_index + 8;
                    int rssiCode_index = sensorcode_index + 2;
                    int tempe_index = rssiCode_index + 2;
                    int rssi_index = tempe_index + 2;
                    int ant_index = rssi_index + 2;

                    byte[] buffCalibrationData = new byte[8];
                    byte[] buffSensorCode = new byte[2];
                    byte[] buffRssiCode = new byte[2];
                    byte[] buffTempe = new byte[2];
                    byte[] buffRssi = new byte[2];
                    byte[] buffAnt = new byte[1];

                    byte[] buffUii = new byte[uii_len];
                    byte[] buffEPC = new byte[uii_len - 2];

                    Array.Copy(bufData, uii_index, buffUii, 0, buffUii.Length);
                    Array.Copy(buffUii, 2, buffEPC, 0, buffEPC.Length);

                    byte[] buffTid = new byte[tid_leng];
                    if (buffTid.Length > 0)
                    {
                        Array.Copy(bufData, tid_idex, buffTid, 0, buffTid.Length);
                    }
                    Array.Copy(bufData, calibrationData_index, buffCalibrationData, 0, buffCalibrationData.Length);
                    Array.Copy(bufData, sensorcode_index, buffSensorCode, 0, buffSensorCode.Length);
                    Array.Copy(bufData, rssiCode_index, buffRssiCode, 0, buffRssiCode.Length);
                    Array.Copy(bufData, tempe_index, buffTempe, 0, buffTempe.Length);
                    Array.Copy(bufData, rssi_index, buffRssi, 0, buffRssi.Length);
                    Array.Copy(bufData, ant_index, buffAnt, 0, buffAnt.Length);

                    //string strData = BitConverter.ToString(bufData, 0, uLen).Replace("-", "");
                    //epc_data = strData.Substring(6, uii_len * 2 - 4);  //Epc
                    //tid_data = strData.Substring(tid_idex * 2, tid_leng * 2); //Tid
                    //string temp = strData.Substring(rssi_index * 2, 4);
                    //rssi_data = ((Convert.ToInt32(temp, 16) - 65535) / 10).ToString();// RSSI  =  (0xFED6   -65535)/10
                    //ant_data = Convert.ToInt32((strData.Substring(ant_index * 2, 2)), 16).ToString();


                    epc = DataConvert.ByteArrayToHexString(buffEPC, buffEPC.Length);
                    calibrationData = DataConvert.ByteArrayToHexString(buffCalibrationData, buffCalibrationData.Length);
                    sensorCode = DataConvert.ByteArrayToHexString(buffSensorCode, buffSensorCode.Length);
                    rssiCode = DataConvert.ByteArrayToHexString(buffRssiCode, buffRssiCode.Length);
                    tempeCode = DataConvert.ByteArrayToHexString(buffTempe, buffTempe.Length);
                    rssi = (((((buffRssi[0] & 0xFF) << 8) | (buffRssi[1] & 0xFF)) - 65535) / 10).ToString();// RSSI  =  (0xFED6   -65535)/10
                    ant = buffAnt[0];

                    return true;
                }
                else
                {
                    return false;
                }
          
        }
        public bool uhfGetTempTagReceived_OnChipRSSI_TempeCode(ref string epc,   ref string rssiCode, ref string tempeCode, ref string rssi, ref int ant)
        {
            int uLen = 0;
            byte[] bufData = new byte[150];
            if (GetTempTagReceived_OnChipRSSI_TempeCode(ref uLen, ref bufData))//GetTempTagReceived   GetReceived_EX
            {

                string data = DataConvert.ByteArrayToHexString(bufData, bufData.Length); ;
                //  uUii = 
                //  1个字节uii长度+ UII数据+ 
                //  1个字节TID数据长度+TID数据
                //  2个字节Rssi code +
                //  2个字节Tempe code +
                //  2个字节RSSI +
                //  1个字节Ant +

                int uii_len = bufData[0];//uii长度
                int uii_index = 1;//uii的起始地址
                int tid_leng = bufData[uii_len + 1];//tid数据长度

                int tid_idex = uii_len + 1;//tid起始位
                if (tid_leng > 0)
                {
                    tid_idex = uii_len + 2;//tid起始位
                }
                int rssiCode_index = tid_idex + tid_leng + 1;
                int tempe_index = rssiCode_index + 2;
                int rssi_index = tempe_index + 2;
                int ant_index = rssi_index + 2;

                byte[] buffRssiCode = new byte[2];
                byte[] buffTempe = new byte[2];
                byte[] buffRssi = new byte[2];
                byte[] buffAnt = new byte[1];

                byte[] buffUii = new byte[uii_len];
                byte[] buffEPC = new byte[uii_len - 2];

                Array.Copy(bufData, uii_index, buffUii, 0, buffUii.Length);
                Array.Copy(buffUii, 2, buffEPC, 0, buffEPC.Length);

                byte[] buffTid = new byte[tid_leng];
                if (buffTid.Length > 0)
                {
                    Array.Copy(bufData, tid_idex, buffTid, 0, buffTid.Length);
                }

                Array.Copy(bufData, rssiCode_index, buffRssiCode, 0, buffRssiCode.Length);
                Array.Copy(bufData, tempe_index, buffTempe, 0, buffTempe.Length);
                Array.Copy(bufData, rssi_index, buffRssi, 0, buffRssi.Length);
                Array.Copy(bufData, ant_index, buffAnt, 0, buffAnt.Length);

                //string strData = BitConverter.ToString(bufData, 0, uLen).Replace("-", "");
                //epc_data = strData.Substring(6, uii_len * 2 - 4);  //Epc
                //tid_data = strData.Substring(tid_idex * 2, tid_leng * 2); //Tid
                //string temp = strData.Substring(rssi_index * 2, 4);
                //rssi_data = ((Convert.ToInt32(temp, 16) - 65535) / 10).ToString();// RSSI  =  (0xFED6   -65535)/10
                //ant_data = Convert.ToInt32((strData.Substring(ant_index * 2, 2)), 16).ToString();


                epc = DataConvert.ByteArrayToHexString(buffEPC, buffEPC.Length);
                rssiCode = DataConvert.ByteArrayToHexString(buffRssiCode, buffRssiCode.Length);
                tempeCode = DataConvert.ByteArrayToHexString(buffTempe, buffTempe.Length);
                rssi = (((((buffRssi[0] & 0xFF) << 8) | (buffRssi[1] & 0xFF)) - 65535) / 10).ToString();// RSSI  =  (0xFED6   -65535)/10
                ant = buffAnt[0];

                return true;
            }
            else
            {
                return false;
            }
            
        }
       
        private byte[] getPowerToBytes(int power)
        {
            power = power * 100;
            int b1 = (power >> 8) & 0xFF;
            int b2 = power & 0xFF;
            byte[] powerValue = { (byte)b1, (byte)b2 };

            return powerValue;
        }

        public bool WriteCalibrationData(byte[] epc, int antNum, int powerValues, byte[] data)
        {
            byte ant = (byte)antNum;
            byte[] power = getPowerToBytes(powerValues);

            if (UHFWriteCalibrationData(epc, ant, power, data) == 0)
                return true;
            else
                return false;
        }

      
        
    }
}
