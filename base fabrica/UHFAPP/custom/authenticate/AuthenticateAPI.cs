using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace UHFAPP.custom.authenticate
{
    class AuthenticateAPI
    {
        /**********************************************************************************************************
        * 功能：验证标签
        * 输入： 
        password -- 访问密码
        bank -- 掩码的数据区(0x00 为 Reserve 0x01 为 EPC，0x02 表示 TID，0x03 表示 USR)。
        addr -- 掩码的地址
        mDataLen -- 掩码的长度
        mData -- 掩码数据
        keyID -- Authenticate命令用的KeyID，默认为0x00
        tLen -- IChallenge_TAM1数据长度,固定为10
        tData -- IChallenge_TAM1数据
        *输出
        recvLen -- 输出数据长度
        recvData -- 输出数据
        返回值：0：执行成功    1：发送失败
        * 
        *********************************************************************************************************/

         [DllImport("UHFAPI.dll",CallingConvention = CallingConvention.Cdecl)]
         private extern static int UHFAuthenticate(  int password,
                                                     byte filterBank,
                                                     short filterAddr,
                                                     byte[] filterData,
                                                     int filterDataLen,
                                                     byte keyID,
                                                     short tLen, 
                                                     byte[] tData,
                                                     short[] recvLen, 
                                                     byte[] recvData);



                     

        //function:AES encrypto or decrypto data
        //in
        //isEnc -- 1 encrypto  0,decrypto
        //keylen == shoulde be 16 or  24 or 32
        //in-out inbuf -- in date 
        //inlen -- the length of input bytes,must be N*16
        //return -1--key length error, others -- the length of inbuf return
         [DllImport("UHFAPI.dll",CallingConvention = CallingConvention.Cdecl)]
         private extern static int AESHandle(byte isEnc, byte[] key, int keylen, byte[] inbuf, long inLen);

 

         /// <summary>
         /// 加密
         /// </summary>
         /// <param name="password"> 访问密码</param>
         /// <param name="filterBank">掩码的数据区(0x00 为 Reserve 0x01 为 EPC，0x02 表示 TID，0x03 表示 USR)。</param>
         /// <param name="filterAddr">掩码的地址</param>
         /// <param name="filterDataLen">掩码的长度</param>
         /// <param name="filterData">掩码数据</param>
         /// <param name="keyID">Authenticate命令用的KeyID，默认为0x00</param>
         /// <param name="tData">IChallenge_TAM1数据， IChallenge_TAM1数据长度,固定为10</param>
         /// <param name="recvLen">输出数据长度</param>
         /// <param name="recvData">输出数据</param>
         /// <returns></returns>
         public byte[] UHFAuthenticate(int password, byte filterBank, int filterAddr, int filterDataLen, byte[] filterData, byte keyID, byte[] tData)
         {
             try
             {
                 short[] recvLen = new short[215];
                 byte[] recvData = new byte[1024];
                 int result = UHFAuthenticate(password, filterBank, (short)filterAddr, filterData, filterDataLen, keyID, (short)tData.Length, tData, recvLen, recvData);
                 if (result == 0)
                 {
                     int len = recvLen[0];
                      byte[] data = new byte[len];
                     Array.Copy(recvData, 0, data,0,len);
                     return data;
               
                 }
                 else
                 {
                     return null;
                 }
             }
             catch (Exception ex)
             {

             }
             return null;
         }

         public byte[] AesDecrypto(byte[] key, byte[] data)
         {
             int result = AESHandle(0, key, 16, data, data.Length);
             if (result == -1)
             {
                 return null;
             }
             return data;
         }

   
    }
}
