using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IPTE_Base_Project.Common.Utils.Plc
{
    public class PlcSiemensAddress
    {
        private string description;
        private ushort word;
        private ushort? bit;        //Possibility for null value because plc address can be for reading a num bytes not a bit.
        private int length;         //Number of words
        private string type;
        private object value;
        private bool isModified;
        private bool toConfirm;
        private bool usedInThread;


        public PlcSiemensAddress()
        {

        }

        #region data accessors
       
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public bool UsedInThread
        {
            get { return usedInThread; }
            set { usedInThread = value; }
        }

        [XmlIgnore]
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }

        [XmlIgnore]
        public bool IsModified
        {
            get { return isModified; }
            set { isModified = value; }
        }

        [XmlIgnore]
        public bool ToConfirm
        {
            get { return toConfirm; }
            set { toConfirm = value; }
        }

        [XmlIgnore]
        public ushort Word
        {
            get { return word; }
        }

        [XmlIgnore]
        public ushort? Bit
        {
            get { return bit; }
        }

        [XmlIgnore]
        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        public string MemoryAddress
        {
            get
            {
                if (bit == null)
                    return word.ToString() + ":" + length.ToString();
                else
                    return word.ToString() + "." + bit.ToString();
            }
            set
            {
                //setMemoryAddress(value);
            }
        }

        #endregion
    }
}
