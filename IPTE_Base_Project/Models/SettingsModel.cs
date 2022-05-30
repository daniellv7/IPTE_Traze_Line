using System;
using System.Reflection;

namespace IPTE_Base_Project.Models
{
    public class SettingsModel : BaseModel
    {
        #region Data members and accessors

        //Path of the settings.ini file
        //private string SETTINGS_PATH = @"C:\Machine\Config\Settings.ini";
        //Absolute path is necessary to save and load excel files using Microsoft.Office.Interop.Excel.
        public string DATA_PATH;

        //Globals
        public bool startButtonMustBePushed
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        public string debugLevel
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public bool simulatePLC
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        //public bool useMaterialSetup
        //{
        //    get { return GetValue<bool>(); }
        //    set { SetValue(value); }
        //}

        public int StationType
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        //Paths
        //public string validationsFilePath
        //{
        //    get { return GetValue<string>(); }
        //    set { SetValue(value); }
        //}

        /// <summary>
        /// This file is Only used in plcDao and plcDaoV2 datasources
        /// </summary>
        //public string plcTagsFilePath
        //{
        //    get { return GetValue<string>(); }
        //    set { SetValue(value); }
        //}

        /// <summary>
        /// This file is Only used in plcDaoV3 datasource
        /// </summary>
        public string plcOmronAddressFilePath
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        //public string partsMaterialsFilePath
        //{
        //    get { return GetValue<string>(); }
        //    set { SetValue(value); }
        //}
        //public string materialsDataBaseFilePath
        //{
        //    get { return GetValue<string>(); }
        //    set { SetValue(value); }
        //}
        //public string screwsStockFilePath
        //{
        //    get { return GetValue<string>(); }
        //    set { SetValue(value); }
        //}

        //Devices
        public string plcAddress
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public int plcPort
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int plcRefreshTime
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string psAddress
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public int psPort
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public double psVoltageProtection
        {
            get { return GetValue<double>(); }
            set { SetValue(value); }
        }

        public double psCurrentProtection
        {
            get { return GetValue<double>(); }
            set { SetValue(value); }
        }

        public int psSelectedChannel
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string APx500ProjectFilePath
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string APx500ReportsFilePath
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        //Constants

        #endregion

        public SettingsModel()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }

        #region Load/Save/Close/Compare

        public void Load()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);

            //CIniFileManager iniFile;

            //iniFile = new CIniFileManager(SETTINGS_PATH);
            //iniFile.LoadIniFile(false);

            ////Globals
            //startButtonMustBePushed = iniFile.GetBoolean("Globals", "StartButtonMustBePushed", false);
            //debugLevel = iniFile.GetString("Globals", "LogLevel", "ERROR");
            //simulatePLC = iniFile.GetBoolean("Globals", "SimulatePLC", false);
            ////useMaterialSetup = iniFile.GetBoolean("Globals", "useMaterialSetup", true);
            //StationType = iniFile.GetInteger("Globals", "Station", -1);            

            ////Paths
            ////validationsFilePath = iniFile.GetString("Paths", "ValidationsFile", "");
            ////plcTagsFilePath = iniFile.GetString("Paths", "PlcTagsFilePath","");
            //plcOmronAddressFilePath = iniFile.GetString("Paths", "PlcOmronAddressFilePath", "");
            ////partsMaterialsFilePath = iniFile.GetString("Paths", "PartsMaterialsFilePath","");
            ////materialsDataBaseFilePath = iniFile.GetString("Paths", "MaterialsDataBaseFilePath", "");
            ////screwsStockFilePath = iniFile.GetString("Paths", "ScrewsStockFilePath", "");
            //DATA_PATH = iniFile.GetString("Paths", "DATA_PATH", "");
            //APx500ProjectFilePath = iniFile.GetString("Paths", "APx500ProjectFilePath", "");
            //APx500ReportsFilePath = iniFile.GetString("Paths", "APx500ReportsFilePath", "");

            ////Devices
            //plcAddress = iniFile.GetString("Devices", "PlcAddress", "");
            //plcPort = iniFile.GetInteger("Devices", "PlcPort", 9600);
            //plcRefreshTime = iniFile.GetInteger("Devices", "PlcRefreshTime",500);

            //psAddress = iniFile.GetString("Devices", "PowerSupplyAddress", "");
            //psPort = iniFile.GetInteger("Devices", "PowerSupplyPort", 5025);
            //string spsVoltageProtection = iniFile.GetString("Devices", "PowerSupplyVoltageProtection", "15");
            //psVoltageProtection = Double.Parse(spsVoltageProtection);
            //string spsCurrentProtection = iniFile.GetString("Devices", "PowerSupplyCurrentProtection", "2");
            //psCurrentProtection = Double.Parse(spsCurrentProtection);
            //psSelectedChannel = iniFile.GetInteger("Devices", "PowerSupplyChannel", 1);

            //SetDebugLevel(debugLevel);

            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }
        

        #endregion
    }
}
