using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UHFAPP.utils
{
    public class EpcInfo
    {
        public EpcInfo(string epc,int count,byte[] epcBytes) {
            this.epc = epc;
            this.count = count;
            this.epcBytes=epcBytes;
        }
        string epc;

        public string Epc
        {
            get { return epc; }
            set { epc = value; }
        }
        int count;

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        byte[] epcBytes;

        public byte[] EpcBytes
        {
            get { return epcBytes; }
            set { epcBytes = value; }
        }
    }
}
