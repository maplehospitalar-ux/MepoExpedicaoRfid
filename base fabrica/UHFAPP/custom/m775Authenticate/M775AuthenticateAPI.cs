using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace UHFAPP.custom.m775Authenticate
{
    class M775AuthenticateAPI
    {
        /*
与上述方法参数一致，区别在于返回值不同，如Impinj M775返回：Challenge：6个字节，Tags Shortened TID：8个字节，Tag Response：8个字节
*/
        
      [DllImport("UHFAPI.dll",CallingConvention = CallingConvention.Cdecl)]
      private extern static int UHFAuthenticateCommon(int password,
                                          byte filterBank,
                                          short filterAddr,
                                          byte[] filterData,
                                          int filterDataLen,
                                          byte keyID,
                                          short tLen,
                                          byte[] tData,
                                          short[] recvLen,
                                          byte[] recvData);


      /// <summary>
      ///  
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
              int result = UHFAuthenticateCommon(password, filterBank, (short)filterAddr, filterData, filterDataLen, keyID, (short)tData.Length, tData, recvLen, recvData);
              if (result == 0)
              {
                  int len = recvLen[0];
                  byte[] data = new byte[len];
                  Array.Copy(recvData, 0, data, 0, len);
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
    }
}
