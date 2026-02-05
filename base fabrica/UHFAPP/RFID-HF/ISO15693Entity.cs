using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UHFAPP.RFID_HF
{
    class ISO15693Entity
    {
        private byte[] uid;
        private byte afi = 0;
        private byte desfid = 0;
        private TagType type;

            /**
         * 标签类型<br>
         * Tag type<br>
         *
         * @author liuruifeng
         */
        public enum TagType
        {
            ICODE2 = 0,
            TI2048 = 4,
            STLRIS64K = 8,
            EM4033 = 12,
            UNKNOWN = 100

        }

        public byte Afi
        {
            get { return afi; }
            set { afi = value; }
        }
        

        public byte Desfid
        {
            get { return desfid; }
            set { desfid = value; }
        }

        public byte[] Uid
        {
            get { return uid; }
            set { uid = value; }
        }

        public TagType Type
        {
            get { return type; }
            set { type = value; }
        }


    }
}
