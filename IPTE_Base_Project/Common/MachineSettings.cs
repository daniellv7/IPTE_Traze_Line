using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;

namespace IPTE_Base_Project.Common
{
    
    

    public class MachineSettings : ICloneable
    {
        public MachineSettings()
        {
            Station_Settings = new Station_Settings();
            Itac_settings = new Itac_Settings();
            Plc_settings = new PLC_Settings();
            Plc_settings_2 = new PLC_Settings();
        }
        public Station_Settings Station_Settings { get; set; }
        public Itac_Settings Itac_settings { get; set; }
        public PLC_Settings Plc_settings { get; set; }
        public PLC_Settings Plc_settings_2 { get; set; }
       


        #region Metods
        /// <summary>
        /// Loads settings from file. Throws exception when fails;
        /// </summary>
        /// <remarks>Relies on xml serialization</remarks>
        /// <returns>Loaded instance, may also be null or throw exception if fails.</returns>
        public static MachineSettings Load(string path)
        {
           
            if (!File.Exists(path)) return new MachineSettings();
            using (FileStream stream = File.OpenRead(path))
            {
                return Load(stream);
            }
        }

        /// <summary>
        /// Loads current configuration from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static MachineSettings Load(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MachineSettings));
            var result = serializer.Deserialize(stream) as MachineSettings;
            return result;
        }

        /// <summary>
        /// Saves current configuration to an xml file.
        /// </summary>
        /// <remarks>Relies on xml serialization</remarks>
        public void Save(string path)
        {
            using (FileStream file = File.Open(path, FileMode.Create))
            {
                Save(file);
            }

        }

        /// <summary>
        /// Saves the current configuration into stream.
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(GetType());
            serializer.Serialize(stream, this);
        }
        #endregion

        public object Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Save(stream);
                stream.Position = 0;
                return Load(stream);
            }
        }

    }

    public class Station_Settings
    {
        [Description("Tartget Name for Destiny PLC")]
        public string Cell_Number { get; set; } = "PLC_1";
        [Description("Tartget Name for Destiny PLC")]
        public bool PLC { get; set; } = false;
        public int Language { get; set; } = 0;

    }

    public class Itac_Settings
    {
        [Description("Enable Itac")]
        public String ItacStCode { get; set; } = "";

        [Description("Enable Itac")]
        public bool Enable { get; set; } = true;
        [Description("Property cluster Node")]
        public string Cluster_Node { get; set; } = "PLC_1";
        [Description("Property App Id")]
        public string AppID { get; set; } = "";
        [Description("Property App Id")]
        public string PropDir { get; set; } = "";

    }

    public class PLC_Settings
    {
        [Description("Tartget IP for Destiny PLC")]
        public string IP { get; set; } = "192.168.13.46";

        [Description("Tartget Name for Destiny PLC")]
        public string PLC_Name { get; set; } = "PLC_1";

        [Description("Simulated Option")]
        public bool PLC_Simulated { get; set; } = false;

        [Description("Read DB Number")]
        public int PLC_Read_DB { get; set; } = 0;

        [Description("Read Bites Number")]
        public int PLC_Read_Bites { get; set; } = 0;

        [Description("Read Bite Start Number")]
        public int PLC_Read_Start { get; set; } = 0;

        [Description("Write DB Number")]
        public int PLC_Write_DB{ get; set; } = 0;

        [Description("Write Bites Number")]
        public int PLC_Write_Bites { get; set; } = 0;

        [Description("Write Bite Start Number")]
        public int PLC_Write_Start { get; set; } = 0;

        [Description("Read Delay Time (miliseconds)")]
        public int PLC_Delay_Time { get; set; } = 0;

        [Description("Autoconnect option when disconnect")]
        public bool AutoConnect { get; set; } = true;

        public string Input_Index { get; set; } = "";
        public string Output_Index { get; set; } = "";

    }

}
