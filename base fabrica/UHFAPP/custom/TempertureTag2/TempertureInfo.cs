using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLEDeviceAPI;

namespace UHFAPP.custom.TempertureTag2
{
   public class TempertureInfo
    {
        private UHFTAGInfo uhfTagInfo;

        public UHFTAGInfo UhfTagInfo
        {
            get { return uhfTagInfo; }
            set { uhfTagInfo = value; }
        }
        private string temperture;
        private string time;

        public string Time
        {
            get { return time; }
            set { time = value; }
        }

     
        public string Temperture
        {
            get { return temperture; }
            set { temperture = value; }
        }



    }
}
