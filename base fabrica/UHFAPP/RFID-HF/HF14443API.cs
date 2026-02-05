using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using BLEDeviceAPI;
using UHFAPP.RFID_HF;

namespace UHFAPP.RFID
{
   class HF14443API
   {

   
   [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UsbOpen();
   [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static void UsbClose();

/**
 *@brief: get HF module version
 *@param[out] pcVer:the point of HF module version
 *@param[out] pcVerLen:the point of HF module version length
 *@return:0->success,others->failure
 */
   [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
   private extern static int HFGetVer(byte[] pcVer, byte[] pcVerLen);

 
/**
 *@brief: Turn on HF antenna
 *@return:0->success,others->failure
 */
  [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
   private extern static int HFTurnOnRF();

/**
 *@brief: Turn off HF antenna
 *@return:0->success,others->failure
 */
  [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
   private extern static int HFTurnOffRF();

/**
 *@brief: request 14443A card
 *@param[in] cMode:0x26 request idle, 0x52 request all
 *@param[out] pcCardType:card type (2 bytes)
 *@return:0->success,others->failure
 */
  [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
  private extern static int HFRequestTypeA(int cMode, byte[] pcCardType);

/**
 *@brief: Anticoll 14443A card
 *@param[out] pcSnr:the point of card serial number(less than 8 bytes)
 *@param[out] pcSnrLen:the point of card serial number length
 *@return:0->success,others->failure
 */
   [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
  private extern static int HFAnticollTypeA(byte[] pcSnr, byte[] pcSnrLen);

/**
 *@brief: Select 14443A card
 *@param[out] SAK:the point of card type(1 byte)
 *@return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
  private extern static int HFSelectTypeA(byte[] SAK);

/**
 *@brief: Activate 14443A card,include request¡¢anticoll¡¢select,
 *@param[in] cMode:0x26 request idle, 0x52 request all
 *@param[out] pcATQA:type0~1(2bytes)+pcUid length(1bytes)+pcUid(n bytes)+type2(1bytes)
 *@return:0->success,others->failure
 */
  [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
  private extern static int HFActivateTypeA(int cMode, byte[] pcATQA);

/**
 *@brief: Halt 14443A card
 *@return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
  private extern static int HFHaltTypeA();

/**
 *@brief: Authentication 14443A card
 *@Param[in] cMode:0x60 A type key, 0x61 B type key
 *@Param[in] cBlock:the cBlock of card,such as M1 card value 0~63
 *@Param[in] pcKey:6 bytes key value
 *@Return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
  private extern static int HFAuthentication(byte cMode,byte cBlock,byte[] pcKey);

/**
 *@brief: Read 14443A card
 *@Param[in] cBlock:the cBlock of card,such as S50 card value 0~63,S70 0~255
 *@Param[out] bdata:16 bytes cBlock data
 *@Return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
  private extern static int  HFReadBlock(byte cBlock, byte[] bdata);

/**
 *@brief: Write 14443A card
 *@Param[in] cBlock:the cBlock of card,such as S50 card value 0~63,S70 0~255
 *@Param[in] pcBlockData:16 bytes cBlock data
 *@Return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
  private extern static int   HFWriteBlock(byte cBlock, byte[] pcBlockData);

/**
 *@brief: initial E wallet value
 *@Param[in] cBlock:the cBlock of card,such as S50 card value 0~63,S70 0~255
 *@Param[in] lValue:32bit
 *@Return:0->success,others->failure
 */
[DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
 private extern static int   HFInitValue(byte cBlock,long lValue);

/**
 *@brief: read E wallet value
 *@Param[in] cBlock:the cBlock of card,such as S50 card value 0~63,S70 0~255
 *@Param[out] plValue:point of 32bit value
 *@Return:0->success,others->failure
 */
[DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
 private extern static int   HFReadValue(byte cBlock,long[] plValue);

/**
 *@brief: decrease E wallet value
 *@Param[in] blockValue:the cBlock of value saved 
 *@Param[in] blockResult:the cBlock of operate
 *@Param[in] value:32bit value
 *@Return:0->success,others->failure
 */
[DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
 private extern static int HFDecValue(byte blockValue,byte blockResult,long value);

/**
 *@brief: increase E wallet value
 *@Param[in] blockValue:the cBlock of value saved 
 *@Param[in] blockResult:the cBlock of operate
 *@Param[in] value:32bit value
 *@Return:0->success,others->failure
 */
[DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
private extern static int HFIncValue(byte blockValue,byte blockResult,long value);

/**
 *@brief: Restore E wallet value
 *@Param[in] cBlock:the cBlock address
 *@Return:0->success,others->failure
 */
[DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
private extern static int HFRestore(byte cBlock);

/**
 *@brief: Transfer E wallet value
 *@Param[in] cBlock:the cBlock address
 *@Return:0->success,others->failure
 */
[DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
private extern static int HFTransfer(byte cBlock);

/**
 *@brief: aticoll ul card
 *@Param[out] pcSnr:the point of card serial number
 *@Param[out] pcSnrLen:the point of card serial number length
 *@Return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
private extern static int HFUlAnticoll(byte[] pcSnr,byte[]pcSnrLen);

/**
 *@brief: write ul card
 *@Param[in] cBlock:the address of card
 *@Param[in] pcWriteData:the point of write data
 *@Param[in] cWriteLen:the length of write  data
 *@Return:0->success,others->failure
 */
  [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
private extern static int HFUlWrite(byte cBlock,byte[] pcWriteData, byte cWriteLen);

/**
 *@brief: reset cpu card
 *@Param[out] cardType:the point of card type
 *@Param[out] pcUid:the point of write data
 *@Param[out] cUidLen:the point of pcUid length
 *@Param[out] pcATR:the point of card reset data
 *@Param[out] pcATRLen:the length of cpu card atq data
 *@Return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
private extern static int HFResetTypeA(byte[] cardType, byte[] pcUid, byte[] cUidLen, byte[] pcATR, byte[] pcATRLen);

/**
 *@brief: rats typeA cpu card
 *@Param[out] pcATR:the point of card reset data
 *@Param[out] pcInfoLen:the length of cpu card atq data
 *@Return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
private extern static int HFRatsTypeA(byte[] pcATR, byte[] pcATRLen);

/**
 *@brief: Halt 14443A card
 *@return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
private extern static int HFHaltTypeB();

/**
 *@brief: reset type B card
 *@Param[out] pcInfo:the point of receive command
 *@Param[out] pcInfoLen:the length of received
 *@Return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
private extern static int HFResetTypeB(byte[] pcInfo, byte[] pcInfoLen);


/**
 *@brief: get pcUid of type B card
 *@Param[out] pcUid:the point of type B card pcUid
 *@Param[out] cUidLen:the length of pcUid
 *@Return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
private extern static int HFGetUidTypeB(byte[] pcUid, byte[] cUidLen);

/**
 *@brief: execute cpu command
 *@Param[in] pcInCos:the point of send command
 *@Param[in] cInLen:the length of send command
 *@Param[out] pcOutCos:the point of receive command
 *@Param[out] pcOutLen:the length of received
 *@Return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
private extern static int HFCpuCommand(byte[] pcInCos, byte  cInLen, byte[] pcOutCos, byte[] pcOutLen);

 



   const int SUCCESS = 0;
   #region usb
      public bool OpenUsb()
      {
         int result = UsbOpen();
         if (result == SUCCESS)
         {
             return true;
         }
         return false;
      }
      public void CloseUsb()
      {
          UsbClose();
      }

      /// <summary>
      /// 获取版本号
      /// </summary>
      /// <returns></returns>
      public string GetRFIDVerion()
      {
          byte[] version = new byte[100];
          byte[] outLen = new byte[1];
          int result = HFGetVer(version, outLen);
          if (result != SUCCESS)
          {
              return null;
          }
          return System.Text.ASCIIEncoding.ASCII.GetString(version, 0, outLen[0]);
      }
      /// <summary>
      /// Turn on HF antenna
     /// </summary>
      /// <returns></returns>
      public bool TurnOn()
      {
          return HFTurnOnRF() == SUCCESS;
      }
       /// <summary>
      /// Turn off HF antenna
       /// </summary>
       /// <returns></returns>
      public bool TurnOff()
      {
          return HFTurnOffRF() == SUCCESS;
      }
  
       /// <summary>
       /// 寻卡
       /// </summary>
      /// <param name="mode">0x26 request idle, 0x52 request all</param>
      /// <returns>pcCardType:card type (2 bytes)</returns>
      public byte[] RequestTypeA(int mode)
      {
           byte[] pcCardType=new byte[2];
           int result=  HFRequestTypeA(mode,pcCardType);
           if (result != SUCCESS)
           {
               return null;
           }
           return pcCardType;
      }

      /// <summary>
      /// Anticoll 14443A card
      /// </summary>
      /// <returns>卡号ID</returns>
      public byte[] AnticollTypeA()
      {
          byte[] pcSnr = new byte[100];
          byte[] pcSnrLen = new byte[1];
          int result = HFAnticollTypeA(pcSnr, pcSnrLen);
          if (result != SUCCESS)
          {
              return null;
          }
          return Utils.CopyArray(pcSnr, pcSnrLen[0]);
      }

       /// <summary>
       /// 获取卡片类型
       /// </summary>
       /// <returns></returns>
      public int SelectTypeA()
      {
          byte[] SAK = new byte[1];
          int result = HFSelectTypeA(SAK);
          if (result != SUCCESS)
          {
              return -1;
          }
          return SAK[0];
      }
     

      /// <summary>
      ///  认证
      /// </summary>
      /// <param name="cMode">cMode:0x60 A type key, 0x61 B type key</param>
      /// <param name="cBlock">cBlock:the cBlock of card,such as M1 card value 0~63</param>
      /// <param name="pcKey">pcKey:6 bytes key value</param>
      /// <returns>success,others->failure</returns>

      public bool Authentication(byte cMode, byte cBlock, byte[] pcKey)
      {
          return HFAuthentication(cMode, cBlock, pcKey)==0;
      }
       /// <summary>
       /// 读卡
       /// </summary>
      /// <param name="cBlock">the cBlock of card,such as S50 card value 0~63,S70 0~255</param>
      /// <returns>16 bytes cBlock data</returns>
      public byte[] ReadBlock(byte cBlock)
      {
          byte[] buff = new byte[16];
          int result = HFReadBlock(cBlock, buff);
          if (result != SUCCESS)
          {
              return null;
          }
          return buff;
      }
       /// <summary>
       /// 写卡
       /// </summary>
       /// <param name="cBlock"></param>
       /// <param name="pcBlockData"></param>
       /// <returns></returns>
      public bool WriteBlock(byte cBlock, byte[] pcBlockData)
      {

          return HFWriteBlock(cBlock, pcBlockData) == SUCCESS;
      }

       /// <summary>
      /// rats typeA cpu card
       /// </summary>
       /// <param name="pcATR"></param>
       /// <param name="pcATRLen"></param>
      public byte[] RatsTypeA()
      {
          byte[] pcATR = new byte[512];
          byte[] pcATRLen = new byte[1];

          int result = HFRatsTypeA(pcATR, pcATRLen);
          if (result != SUCCESS)
          {
              return null;
          }

          return Utils.CopyArray(pcATR, pcATRLen[0]);
      }

       /// <summary>
      /// execute cpu command
       /// </summary>
       /// <param name="cmd"></param>
       /// <returns></returns>
      public byte[] CpuCommand(byte[] cmd)
      {
          byte[] pcOutCos = new byte[512];
          byte[] pcOutLen = new byte[1];
          int result = HFCpuCommand(cmd, (byte)cmd.Length, pcOutCos, pcOutLen);
          if (result != SUCCESS)
          {
              return null;
          }
          return Utils.CopyArray(pcOutCos, pcOutLen[0]);
      }

       /// <summary>
      /// get pcUid of type B card
       /// </summary>
       /// <returns></returns>
      public byte[] GetUidTypeB()
      {
          byte[] pcUid = new byte[512];
          byte[] cUidLen = new byte[1];
          int result = HFGetUidTypeB(pcUid, cUidLen);
          if (result != SUCCESS)
          {
              return null;
          }
          return Utils.CopyArray(pcUid, cUidLen[0]);
      }
      public byte[] ResetTypeB()
      {

          byte[] pcInfo = new byte[100];
          byte[] pcInfoLen = new byte[10];

          int result = HFResetTypeB(pcInfo, pcInfoLen);
          if (result != SUCCESS)
          {
              return null;
          }
          return Utils.CopyArray(pcInfo, pcInfoLen[0]);
      }
     
   #endregion


 
    }
}
