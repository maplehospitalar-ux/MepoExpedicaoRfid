using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using BLEDeviceAPI;

namespace UHFAPP.RFID_HF
{
    class PSAMAPI
    {
        /**
         *@brief: initial smart card module 
         *@Param[in] cSlotNum:slot number,value(0/1) 
         *@Return:0->success,others->failure
         */
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int SmartCardInit(byte cSlotNum);

        /**
         *@brief: free smart card module 
         *@Param[in] cSlotNum:slot number,value(0/1/2) 
         *@Return:0->success,others->failure
         */
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int SmartCardFree(byte cSlotNum);


        /**
         *@brief: reset smart card 
         *@Param[in] cSlotNum:slot number,value(0/1/2) 
         *@Param[out] pcATR:the point of out data
         *@Param[out] pcATRLen:the point of out data length
         *@Return:0->success,others->failure
         */
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int SmartCardReset(byte cSlotNum, byte[] pcATR, byte[] pcATRLen);

        /**
         *@brief: smart card transfer command
         *@Param[in] cSlotNum:slot number,value(0/1/2) 
         *@Param[in] pcInCmd:the point of command data 
         *@Param[in] cLen:command length
         *@Param[out] pcOutCmd:the point of out data
         *@Param[out] pcOutLen:the point of out data length
         *@Return:0->success,others->failure
         */
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int SmartCardTransferCmd(byte cSlotNum, byte[] pcInCmd, byte cLen, byte[] pcOutCmd, byte[] pcOutLen);

        const int SUCCESS = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSlotNum">cSlotNum:slot number,value(0/1) </param>
        /// <returns></returns>
        public bool Init(byte cSlotNum)
        {
            return SmartCardInit(cSlotNum) == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSlotNum"></param>
        /// <returns></returns>
        public bool Free(byte cSlotNum)
        {
            return SmartCardFree(cSlotNum) == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSlotNum"></param>
        /// <returns></returns>
        public byte[] Reset(byte cSlotNum)
        {
            byte[] pcATR=new byte[256];
            byte[] pcATRLen=new byte[1];
            int result= SmartCardReset(  cSlotNum , pcATR,   pcATRLen);
            if (result != SUCCESS)
            {
                return null;
            }
            return Utils.CopyArray(pcATR, pcATRLen[0]);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSlotNum"></param>
        /// <param name="pcInCmd"></param>
        /// <returns></returns>
        public byte[] TransferCmd(byte cSlotNum, byte[] pcInCmd)
        {
            byte[] pcOutCmd = new byte[256];
            byte[] pcOutLen = new byte[1];

            int result = SmartCardTransferCmd(cSlotNum, pcInCmd, (byte)pcInCmd.Length, pcOutCmd, pcOutLen);
            if (result != SUCCESS)
            {
                return null;
            }
            return Utils.CopyArray(pcOutCmd, pcOutLen[0]);
        }
    }
}
