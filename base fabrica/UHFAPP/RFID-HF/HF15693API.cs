using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using BLEDeviceAPI;
 

namespace UHFAPP.RFID_HF
{
    class HF15693API
    {
        /**
         *@brief: 15693 inventory
         *@Param[in] cMode:inventory cMode,0~3
         *@Param[in] AFI:AFI value
         *@Param[out] pcData:the point of receive data
         *@Param[out] dataLen:the length of received
         *@Return:0->success,others->failure
         */
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int HF15693Inventory(byte cMode, byte AFI, byte[] pcData, byte[] dataLen);
        /**
         *@brief: ISO15693 Get card system information 
         *@Param[in] cMode:less than 10
         *@Param[in] pcUid:the point of pcUid
         *@Param[in] cUidLen:the length of pcUid
         *@Param[out] pcInfo:the point of system infoemation 
         *@Param[out] pcInfoLen:the point of system information length
         *@Return:0->success,others->failure
         */
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int HF15693GetSystemInfo(byte cMode, byte[] pcUid, byte cUidLen, byte[] pcInfo, byte[] pcInfoLen);

        /**
         *@brief: ISO15693 read
         *@Param[in] cMode:less than 10
         *@Param[in] pcUid:the point of pcUid
         *@Param[in] cUidLen:the length of pcUid
         *@Param[in] iStartBlock:start address of cBlock
         *@Param[in] cBlockNum:cBlock number
         *@Param[out] pData:the point of receive data
         *@Param[out] pDataLen:the length of received data
         *@Return:0->success,others->failure
         */
        [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int HF15693Read(byte cMode, byte[] pcUid, int cUidLen, int iStartBlock, int cBlockNum, byte[] pData, byte[] pDataLen);


        /**
         *@brief: ISO15693 Write
         *@Param[in] cMode:less than 10
         *@Param[in] pcUid:the point of pcUid
         *@Param[in] cUidLen:the length of pcUid
         *@Param[in] iStartBlock:address cBlock start
         *@Param[in] cBlockNum:cBlock number
         *@Param[in] pwData:the point of write data
         *@Param[in] wLen:the length of write data
         *@Return:0->success,others->failure
         */
         [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
          private extern static int HF15693Write(byte cMode, byte[] pcUid, int cUidLen, int iStartBlock, int cBlockNum, byte[] pwData, byte wLen);


         /**
   *@brief: ISO15693 stay quite
   *@Return:0->success,others->failure
   */
         [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
         private extern static int HF15693StayQuite();

 
         /**
          *@brief: ISO15693 Lock cBlock
          *@Param[in] cMode:less than 10
          *@Param[in] pcUid:the point of pcUid
          *@Param[in] cUidLen:the length of pcUid
          *@Param[in] iStartBlock:address cBlock start
          *@Param[in] cBlockNum:cBlock number
          *@Return:0->success,others->failure
          */
         [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
         private extern static int HF15693LockBlock(byte cMode, byte[] pcUid, byte cUidLen, int iStartBlock, byte cBlockNum);

         /**
          *@brief: ISO15693 select card
          *@Param[out] pcInfo:the point of card information 
          *@Param[out] pcInfoLen:the point of card information length
          *@Return:0->success,others->failure
          */
         [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
         private extern static int HF15693Select(byte[] pcInfo, byte[] pcInfoLen);

         /**
          *@brief: ISO15693 reset to ready
          *@Param[out] pcInfo:the point of card information 
          *@Param[out] pcInfoLen:the point of card information length
          *@Return:0->success,others->failure
          */
         [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
         private extern static int HF15693ResetReady(byte[] pcInfo, byte[] pcInfoLen);

         /**
  *@brief: ISO15693 Write AFI
  *@Param[in] cMode:less than 10
  *@Param[in] pcUid:the point of pcUid
  *@Param[in] cUidLen:the length of pcUid
  *@Param[in] cAFI:AFI value
  *@Return:0->success,others->failure
  */
         [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
         private extern static int HF15693WriteAFI(byte cMode, byte[] pcUid, int cUidLen, byte cAFI);

         /**
          *@brief: ISO15693 Lock AFI
          *@Param[in] cMode:less than 10
          *@Param[in] pcUid:the point of pcUid
          *@Param[in] cUidLen:the length of pcUid
          *@Return:0->success,others->failure
          */
         [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
         private extern static int HF15693LockAFI(byte cMode, byte[] pcUid, byte cUidLen);

        /**
 *@brief: ISO15693 write DSFID
 *@Param[in] cMode:less than 10
 *@Param[in] pcUid:the point of pcUid
 *@Param[in] cUidLen:the length of pcUid
 *@Param[in] cDSFID:DSFID value
 *@Return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
private extern static int  HF15693WriteDsfid(byte  cMode, byte[] pcUid, byte  cUidLen, byte  cDSFID);

/**
 *@brief: ISO15693 Lock DSFID
 *@Param[in] cMode:less than 10
 *@Param[in] pcUid:the point of pcUid
 *@Param[in] cUidLen:the length of pcUid
 *@Return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
private extern static int  HF15693LockDSFID(byte  cMode, byte[] pcUid, byte  cUidLen);

/**
 *@brief: ISO15693 get multiple blocks
 *@Param[in] cMode:less than 10
 *@Param[in] pcUid:the point of pcUid
 *@Param[in] cUidLen:the length of pcUid
 *@Param[in] iStartBlock:address cBlock start
 *@Param[out] cBlockNum:cBlock number
 *@Param[out] pcData:the point of receive data
 *@Return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
private extern static int  HF15693GetMultipleBlock(byte  cMode, byte[] pcUid, byte  cUidLen, int iStartBlock, byte  cBlockNum, byte[] pcData, byte[] pcDataLen);

/**
 *@brief: ISO15693 transfer command
 *@Param[in] pcInCmd:the point of command data 
 *@Param[in] cLen:command length
 *@Param[out] pcOutCmd:the point of out data
 *@Param[out] pcOutLen:the point of out data length
 *@Return:0->success,others->failure
 */
 [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
private extern static int  HF15693TransferCmd(byte[] pcInCmd, byte  cLen, byte[] pcOutCmd, byte[] pcOutLen);


        private const int SUCCESS = 0;

     
        /// <summary>
        /// 寻卡
        /// </summary>
        public ISO15693Entity Inventory()
        {
            byte cMode = 1;
            byte AFI = 0;

            byte[] pcData = new byte[512];
            byte[] dataLen = new byte[10];
            if (HF15693Inventory(cMode, AFI, pcData, dataLen) != SUCCESS)
            {
                return null;
            }
            pcData = Utils.CopyArray(pcData, dataLen[0]);
            // 声明uid数组
            byte[] cUID = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                cUID[i] = pcData[i + 2];
            }

            ISO15693Entity entity = new ISO15693Entity();

            ISO15693Entity.TagType type = ISO15693Entity.TagType.UNKNOWN;

            if (pcData.Length > 8)
            {
                switch (pcData[8])
                {
                    case 4:
                        type = ISO15693Entity.TagType.ICODE2;
                        break;
                    case 7:
                        type = ISO15693Entity.TagType.TI2048;
                        break;
                    case 2:
                        type = ISO15693Entity.TagType.STLRIS64K;
                        break;
                    case 22: // 0x16 EM4033
                        type = ISO15693Entity.TagType.EM4033;
                        break;

                    default:
                        break;
                }
            }

            entity.Type = type;
            entity.Uid = cUID;
 
            if (entity.Type == ISO15693Entity.TagType.EM4033)
            {
                return entity;
            }
            byte[] cInfo;
            if (entity.Type == ISO15693Entity.TagType.STLRIS64K)
            {
                cInfo = GetSystemInfo(8, null);
            }
            else
            {
                cInfo = GetSystemInfo(0, null);
            }

            // 获取信息成功
            if (cInfo != null && cInfo.Length > 10)
            {
                entity.Afi = cInfo[10];
                entity.Desfid = cInfo[9];
            }
            return entity;
        }
         
        /// <summary>
        /// 
        /// </summary>
         /// <param name="cMode">less than 10</param>
        /// <param name="pcUid"></param>
        /// <returns></returns>
         public byte[] GetSystemInfo(byte cMode, byte[] pcUid)
         {
             byte[] pcInfo = new byte[512];
             byte[] pcInfoLen = new byte[10];
             int len = 0;
             if (pcUid != null)
             {
                 len = pcUid.Length;
             }

             int result = HF15693GetSystemInfo(cMode, pcUid, (byte)len, pcInfo, pcInfoLen);
             if (result != SUCCESS)
             {
                 return null;
             }
             return Utils.CopyArray(pcInfo, pcInfoLen[0]);
         }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="cMode">less than 10</param>
         /// <param name="pcUid">the point of pcUid</param>
         /// <param name="iStartBlock">start address of cBlock</param>
         /// <param name="cBlockNum">cBlock number</param>
         /// <returns></returns>
         public byte[] Read(ISO15693Entity entity, int block)
         {

             byte[] origUID = entity.Uid;
             if (entity.Type == ISO15693Entity.TagType.EM4033)
             {
                 return null;
             }
             int result = -1;
             byte[] pData = new byte[512];
             byte[] pDataLen = new byte[10];
             if (entity.Type == ISO15693Entity.TagType.STLRIS64K)
             {
                 result =  HF15693Read(0, origUID, origUID.Length, block, 1, pData, pDataLen);
             }
             else
             {
                 result = HF15693Read(0, origUID, origUID.Length, block, 1, pData, pDataLen);
             }

             if (result != SUCCESS)
             {
                 return null;
             }
             return Utils.CopyArray(pData, pDataLen[0]);

         }
         /// <summary>
         /// 写数据
         /// </summary>
         /// <param name="cMode">less than 10</param>
         /// <param name="pcUid">the point of pcUid</param>
         /// <param name="iStartBlock">address cBlock start</param>
         /// <param name="cBlockNum">cBlock number</param>
         /// <param name="pwData"></param>
         /// <returns></returns>
         public bool Write(ISO15693Entity entity, int block, byte[] pszData)
         {
 

             byte[] uid = entity.Uid;
             int iRes = -1;
             if (entity.Type == ISO15693Entity.TagType.ICODE2)
             {
                 iRes = HF15693Write((byte)0, uid, 0, block, 1,  pszData, (byte)pszData.Length);

             }
             else if (entity.Type == ISO15693Entity.TagType.TI2048)
             {
                 iRes = HF15693Write((byte)4, uid, 0, block, 1, pszData, (byte)pszData.Length);

             }
             else if (entity.Type == ISO15693Entity.TagType.STLRIS64K)
             {
                 iRes = HF15693Write((byte)0, uid, 0, block, 1, pszData, (byte)pszData.Length);

             }
             else
             {
                 iRes = HF15693Write((byte)0, uid, 0, block, 1, pszData, (byte)pszData.Length);
             }


             return iRes == SUCCESS;

         }
          /// <summary>
          /// 写afi
          /// </summary>
          /// <param name="entity"></param>
          /// <param name="afi"></param>
          /// <returns></returns>
         public bool WriteAFI(ISO15693Entity entity, byte afi)
         {
             int res = -1;
             if (entity.Type == ISO15693Entity.TagType.ICODE2)
             {
                 res = HF15693WriteAFI(0, null, 0, afi);
             }
             else if (entity.Type == ISO15693Entity.TagType.TI2048)
             {
                 res = HF15693WriteAFI(4, null, 0, afi);
             }
             else if (entity.Type == ISO15693Entity.TagType.STLRIS64K)
             {
                 res = HF15693WriteAFI(0, null, 0, afi);
             }
             else
             {
                 res = HF15693WriteAFI(0, null, 0, afi);
             }

             return res == SUCCESS;
         }
        /// <summary>
        ///锁afi
        /// </summary>
        /// <returns></returns>
         public bool LockAFI()
         {
             int res = HF15693LockAFI(0, null, 0);

             return res == SUCCESS;
         }
         /// <summary>
         /// 写dsfid
         /// </summary>
         /// <param name="entity"></param>
         /// <param name="dsfid"></param>
         /// <returns></returns>
         public bool WriteDsfid(ISO15693Entity entity, byte dsfid)
         {
             int res = -1;
             if (entity.Type == ISO15693Entity.TagType.ICODE2)
             {
                 res = HF15693WriteDsfid(0, null, 0, dsfid);
             }
             else if (entity.Type == ISO15693Entity.TagType.TI2048)
             {
                 res = HF15693WriteDsfid(4, null, 0, dsfid);
             }
             else if (entity.Type == ISO15693Entity.TagType.STLRIS64K)
             {
                 res = HF15693WriteDsfid(0, null, 0, dsfid);
             }
             else
             {
                 res = HF15693WriteDsfid(0, null, 0, dsfid);
             }

             return res == SUCCESS;
         }
         /// <summary>
         /// 锁Dsfid
         /// </summary>
         /// <returns></returns>
         public bool LockDsfid()
         {
             int res = HF15693LockDSFID(0, null, 0);

             return res == SUCCESS;
         }

    }
}
