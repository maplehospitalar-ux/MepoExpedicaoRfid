using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UHFAPP.custom.TempertureTag2;
using BLEDeviceAPI;

namespace UHFAPP
{
    public class TempertureTag2 : UHFAPI
    {
  
       /*
            开始读取标签温度
            return: 0--success, -1--unknow error, others--error code
            mask_bank：掩码的数据区(0x00 为 Reserve 0x01 为 EPC， 0x02 表示 TID， 0x03 表示USR)。
            mask_addr：为掩码的地址(bit为单位)，高字节在前。
            mask_len：为掩码的长度(bit为单位)，高字节在前。
            mask_data：为掩码数据，mask_len为0时，这里没有数据
            min_temp:minum of limited temperature
            max_temp:maxum of limited temperature
            work_delay: start logging after delayed time
            work_interval:interval of working
        */
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int UHFStartLogging(byte mask_bank, byte mask_addr, int mask_len, byte[] mask_data,
                       float min_temp, float max_temp, int work_delay, int work_interval);


        /*
        停止读取标签温度
        return: 0--success, -1--unknow error, others--error code
        mask_bank：掩码的数据区(0x00 为 Reserve 0x01 为 EPC， 0x02 表示 TID， 0x03 表示USR)。
        mask_addr：为掩码的地址(bit为单位)，高字节在前。
        mask_len：为掩码的长度(bit为单位)，高字节在前。
        mask_data：为掩码数据，mask_len为0时，这里没有数据
        password: password,default 0
        */
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int UHFStopLogging (byte mask_bank, int  mask_addr, int mask_len, byte[] mask_data, long password);

        /*
        模式检查
        return: 0--success, -1--unknow error, others--error code
        mask_bank：掩码的数据区(0x00 为 Reserve 0x01 为 EPC， 0x02 表示 TID， 0x03 表示USR)。
        mask_addr：为掩码的地址(bit为单位)，高字节在前。
        mask_len：为掩码的长度(bit为单位)，高字节在前。
        mask_data：为掩码数据，mask_len为0时，这里没有数据
        password: password,default 0
        */
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int UHFCheckOpMode(byte mask_bank, int mask_addr, int mask_len,byte[]  mask_data);


        /**********************************************************************************************************
          * 功能：获取连续盘存标签数据
          * 输出：uLenUii -- UII长度
          * 输出：uUii -- UII数据
         *********************************************************************************************************/
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int UHF_GetReceived_EX(ref int uLenUii, byte[] uUii);


        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int UHFSetEPCTIDUSERMode(byte saveflag, byte memory, byte address, byte lenth);
  
        public TempertureInfo uhfGetReceivedTempertureInfo()
        {
            int uLen = 0;
            byte[] bufData = new byte[150];
            if (GetReceived_EX(ref uLen, ref bufData))
            {
                string epc_data = string.Empty;
                string uii_data = string.Empty;//uii数据
                string tid_data = string.Empty; //tid数据
                string rssi_data = string.Empty;
                string ant_data = string.Empty;
                string user_data = string.Empty;

                int uii_len = bufData[0];//uii长度
                int tid_leng = bufData[uii_len + 1];//tid数据长度
                int tid_idex = uii_len + 2;//tid起始位
                int rssi_index = 1 + uii_len + 1 + tid_leng;
                int ant_index = rssi_index + 2;

                string strData = BitConverter.ToString(bufData, 0, uLen).Replace("-", "");
                epc_data = strData.Substring(6, uii_len * 2 - 4);  //Epc

                if (tid_leng > 12)
                {
                    tid_data = strData.Substring(tid_idex * 2, 24); //Tid
                    user_data = strData.Substring(tid_idex * 2 + 24, (tid_leng - 12) * 2); //Tid
                }
                else
                {
                    tid_data = strData.Substring(tid_idex * 2, tid_leng * 2); //Tid
                }

                string temp = strData.Substring(rssi_index * 2, 4);
                int rssiTemp = Convert.ToInt32(temp, 16) - 65535;
                rssi_data = ((float)rssiTemp / 10.0).ToString();// RSSI  =  (0xFED6   -65535)/10
                if (!rssi_data.Contains("."))
                {
                    rssi_data = rssi_data + ".0";
                }
                ant_data = Convert.ToInt32((strData.Substring(ant_index * 2, 2)), 16).ToString();

                UHFTAGInfo info = new UHFTAGInfo();
                info.Epc = epc_data;
                info.Tid = tid_data;
                info.Rssi = rssi_data;
                info.Ant = ant_data;
               // info.User = user_data;



                TempertureInfo tempInfo = new TempertureInfo();
                tempInfo.UhfTagInfo = info;
                if (user_data.Length == 8)
                {
                    byte[] temperture_data = DataConvert.HexStringToByteArray(user_data);
                    int itemp = temperture_data[0] | ((temperture_data[1] & 3) << 8);
                    tempInfo.Temperture = convert10BitFloat(itemp) + "";
                    int itime = temperture_data[2] | ((temperture_data[3] & 127) << 8);
                    tempInfo.Time = itime + "";
                }
                return tempInfo;


            }
            else
            {
                return null;
            }
        }
     

        /*
        public TempertureInfo uhfGetReceivedTempertureInfo()
        {
            int uLen = 0;
            byte[] bufData = new byte[150];
            if (GetReceived_EX(ref uLen, ref bufData))
            {

                int uii_len = bufData[0];//uii长度
                byte[] epc_data = new byte[uii_len - 2];
                byte[] temperture_data = new byte[4];
                byte[] rssi_data = new byte[2];
                byte[] ant_data = new byte[1];

                int temperture_idex = 3 + epc_data.Length + 1;
                int rssi_index = temperture_idex + temperture_data.Length;
                int ant_index = rssi_index + rssi_data.Length;
                //epc数据
                Array.Copy(bufData, 3, epc_data, 0, epc_data.Length);// strData.Substring(6, uii_len * 2 - 4);  //Epc
                //温度数据
                if (bufData[temperture_idex - 1] > 0)
                {
                    Array.Copy(bufData, temperture_idex, temperture_data, 0, temperture_data.Length);
                }
                else
                {
                    rssi_index = temperture_idex;
                    ant_index = rssi_index + rssi_data.Length;
                }
                //rssi
                Array.Copy(bufData, rssi_index, rssi_data, 0, rssi_data.Length);
                //ant
                Array.Copy(bufData, ant_index, ant_data, 0, ant_data.Length);


                int rssiTemp = ((rssi_data[0] << 8) | (rssi_data[1])) - 65535;
                string strRssi = ((float)rssiTemp / 10.0).ToString();// RSSI  =  (0xFED6   -65535)/10
                if (!strRssi.Contains("."))
                {
                    strRssi = strRssi + ".0";
                }


                UHFTAGInfo info = new UHFTAGInfo();
                info.Epc = DataConvert.ByteArrayToHexString(epc_data);
                info.Rssi = strRssi;
                info.Ant = ant_data[0].ToString();
                TempertureInfo tempInfo = new TempertureInfo();
                tempInfo.UhfTagInfo = info;
                int itemp = temperture_data[0] | ((temperture_data[1] & 3) << 8);
                if (bufData[temperture_idex - 1] > 0)
                {
                    tempInfo.Temperture = convert10BitFloat(itemp) + "";

                    int itime = temperture_data[2] | ((temperture_data[3] & 127) << 8);
                    tempInfo.Time = itime + "";

                }
                return tempInfo;
            }
            else
            {
                return null;
            }
        }
        */
        float convert10BitFloat(int temp)
        {
            sbyte ret = (sbyte)(temp >> 2);
            float f = ret;
            if (f > 0)
            {
                f += (float)((temp & 0x03) * 0.25);
            }
            else
            {
                f -= (float)((temp & 0x03) * 0.25);
                f += 1;
            }
            return f;
        }
        /// <summary>
        /// 开始读温度
        /// </summary>
        /// <param name="filter_bank"></param>
        /// <param name="filter_addr"></param>
        /// <param name="filter_len"></param>
        /// <param name="filter_data"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool StartLogging(byte filter_bank, byte filter_addr, int filter_len, byte[] filter_data,
                       float min_temp, float max_temp, int work_delay, int work_interval)
        {

            int result = UHFStartLogging(filter_bank, filter_addr, filter_len, filter_data,
                          min_temp, max_temp, work_delay, work_interval);
            if (result == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 停止读温度
        /// </summary>
        /// <param name="filter_bank"></param>
        /// <param name="filter_addr"></param>
        /// <param name="filter_len"></param>
        /// <param name="filter_data"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool StopLogging(byte filter_bank, int filter_addr, int filter_len, byte[] filter_data, long password)
        {

            int result = UHFStopLogging(filter_bank, filter_addr, filter_len, filter_data, password);
            if (result == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter_bank"></param>
        /// <param name="filter_addr"></param>
        /// <param name="filter_len"></param>
        /// <param name="filter_data"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int CheckOpMode(byte filter_bank, int filter_addr, int filter_len, byte[] filter_data)
        {

            int result = UHFCheckOpMode(filter_bank, filter_addr, filter_len, filter_data);
            return result;
        }


        public bool setEPCAndTemperature() {
            int result = UHFSetEPCTIDUSERMode((byte)0, (byte)3, (byte)0, (byte)0) ;
            return result == 0;
        }

 
    }
}
