using Company.PLC_Omron_V3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IPTE_Base_Project.Common.Utils.Plc
{
    public class PlcOmronAddressList
    {
        [XmlElement("PlcOmronAddress")]
        public List<PlcOmronAddress> addressList;
    }

    public class PlcOmronAddress
    {
        private CPlcOmronEthernet.EMemoryAreaBit? bitArea;
        private CPlcOmronEthernet.EMemoryAreaWord? wordArea;
        private string description;
        private ushort word;
        private ushort? bit;        //Possibility for null value because plc address can be for reading a num bytes not a bit.
        private int length;         //Number of words
        private string type;
        private object value;
        private bool isModified;
        private bool toConfirm;
        private bool usedInThread;

        #region constructors

        public PlcOmronAddress()
        {

        }

        public PlcOmronAddress(string memoryAddress)
        {
            this.MemoryAddress = memoryAddress;
        }

        /// <summary>
        /// Address for a bit position in CIO bit area.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="db"></param>
        /// <param name="word"></param>
        /// <param name="bitPosition"></param>

        public PlcOmronAddress(string description, ushort word, ushort? bit)
        {
            this.bitArea = CPlcOmronEthernet.EMemoryAreaBit.Cio;
            this.description = description;
            this.word = word;
            this.length = 1;
            this.bit = bit;
        }

        /// <summary>
        /// Address for a words in DM area.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="db"></param>
        /// <param name="word"></param>
        /// <param name="length"></param>

        public PlcOmronAddress(string description, ushort word, int length)
        {
            this.wordArea = CPlcOmronEthernet.EMemoryAreaWord.DMArea;
            this.description = description;
            this.word = word;
            this.length = length;
        }

        public PlcOmronAddress(CPlcOmronEthernet.EMemoryAreaBit? bitArea, string description, ushort word, ushort? bit, int length)
        {
            this.bitArea = bitArea;
            this.description = description;
            this.word = word;
            this.length = length;
            this.bit = bit;
        }

        public PlcOmronAddress(CPlcOmronEthernet.EMemoryAreaBit? bitArea, ushort word, ushort? bit, int length)
        {
            this.bitArea = bitArea;
            this.word = word;
            this.length = length;
            this.bit = bit;
        }

        public PlcOmronAddress(CPlcOmronEthernet.EMemoryAreaWord? wordArea, string description, ushort word, ushort? bit, int length)
        {
            this.wordArea = wordArea;
            this.description = description;
            this.word = word;
            this.length = length;
            this.bit = bit;
        }

        public PlcOmronAddress(CPlcOmronEthernet.EMemoryAreaWord? wordArea, ushort word, ushort? bit, int length)
        {
            this.wordArea = wordArea;
            this.word = word;
            this.length = length;
            this.bit = bit;
        }

        #endregion

        #region data accessors
        [XmlIgnore]
        public CPlcOmronEthernet.EMemoryAreaBit? BitArea
        {
            get { return bitArea; }
        }

        [XmlIgnore]
        public CPlcOmronEthernet.EMemoryAreaWord? WordArea
        {
            get { return wordArea; }
        }

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
                setMemoryAddress(value);
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// memoryAddres must be like Byte:length or Byte.bit
        /// For Byte:length is a DMArea.
        /// For Byte.bit is a Cio Area.
        /// </summary>
        /// <param name="memoryAddress"></param>
        private void setMemoryAddress(string memoryAddress)
        {
            string[] split_1;
            split_1 = memoryAddress.Split(':');
            if (split_1.Length > 1)
            {
                this.wordArea = CPlcOmronEthernet.EMemoryAreaWord.DMArea;
                this.word = Convert.ToUInt16(split_1[0]);
                this.length = Convert.ToInt32(split_1[1]);
                return;
            }

            split_1 = memoryAddress.Split('.');
            if (split_1.Length > 1)
            {
                this.bitArea = CPlcOmronEthernet.EMemoryAreaBit.Cio;
                this.word = Convert.ToUInt16(split_1[0]);
                this.bit = Convert.ToUInt16(split_1[1]);
            }
        }
        #endregion
    }
}
