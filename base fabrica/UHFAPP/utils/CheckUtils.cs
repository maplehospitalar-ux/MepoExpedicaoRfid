using BLEDeviceAPI;
using System.Collections;
using System.Collections.Generic;
using UHFAPP.utils;
namespace UHFAPP
{
    public class CheckUtils
    {
        public static int getInsertIndex(List<EpcInfo> listEpc, string Epc, bool[] exists)
        {
         
            int startIndex = 0;
            int endIndex = listEpc.Count;
            int judgeIndex;
            int ret;
            if (endIndex == 0)
            {
                exists[0] = false;
                return 0;
            }
            endIndex--;
            while (true)
            {
                judgeIndex = (startIndex + endIndex) / 2;
                ret = compareBytes(DataConvert.HexStringToByteArray(Epc), listEpc[judgeIndex].EpcBytes);
                if (ret > 0)
                {
                    if (judgeIndex == endIndex)
                    {
                        exists[0] = false;
                        return judgeIndex + 1;
                    }
                    startIndex = judgeIndex + 1;
                }
                else if (ret < 0)
                {
                    if (judgeIndex == startIndex)
                    {
                        exists[0] = false;
                        return judgeIndex;
                    }
                    endIndex = judgeIndex - 1;
                }
                else
                {
                    exists[0] = true;
                    return judgeIndex;
                }
            }
            
        }

        //return 1,2 b1>b2
        //return -1,-2 b1<b2
        //retrurn 0;b1 == b2
        private static int compareBytes(byte[] b1, byte[] b2)
        {
            int len = b1.Length < b2.Length ? b1.Length : b2.Length;
            int value1;
            int value2;
            for (int i = 0; i < len; i++)
            {
                value1 = b1[i] & 0xFF;
                value2 = b2[i] & 0xFF;
                if (value1 > value2)
                {
                    return 1;
                }
                else if (value1 < value2)
                {
                    return -1;
                }
            }
            if (b1.Length > b2.Length)
            {
                return 2;
            }
            else if (b1.Length < b2.Length)
            {
                return -2;
            }
            else
            {
                return 0;
            }
        }
    }
}



