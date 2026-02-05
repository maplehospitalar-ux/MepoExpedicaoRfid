using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using UHFAPP.utils;

namespace UHFAPP
{
    public class ReadEPCFormData
    {
        public static string filter_Data = "";
        public static string filter_Ptr = "32";
        public static string filter_len = "0";
        public static int filter_bank = 1;
        public static bool filter_save = false;
        public static string Total = "0";
        public static string selectedText = "";
        public static string Time = "0";
        public static List<EpcInfo> epcList = new List<EpcInfo>();
        public static List<ListViewItem> listviewdata = new List<ListViewItem>();
    }
}
