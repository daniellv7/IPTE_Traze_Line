using IPTE_Base_Project.Common;
using IPTE_Base_Project.Common.Configuration;
using IPTE_Base_Project.Common.Utils.Plc;
using IPTE_Base_Project.DataSources.DeviceConfig;
using IPTE_Base_Project.Devices;
using IPTE_Base_Project.Devices.CommChannel;
using IPTE_Base_Project.Managers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace IPTE_Base_Project.DataSources.Config
{
    public class GlobalConfigDS : Logging
    {
        // <Name, Device configuration>
        public Dictionary<string, IDeviceConfig> DevicesConfig { get; set; }
        public List<PlcConfig> PlcConfigs { get; set; }
        public Dictionary<string, string> CollectorPaths { get; set; }
        public string ReferencesFilePath { get; set; }
        public IConfiguration References { get; set; }
        public IConfiguration TestOrderCheck { get; set; }
        public string RefLayoutPath { get; set; }
        public string ReportsPath { get; set; }

        public GlobalConfigDS()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            LoadFiles();
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }

        public void LoadFiles()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            string executionPath;
            string executionDir;
            string basePath;
            string configPath;
            string generalConfigPath;

            //Get base path for the application
            executionPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            executionDir = Path.GetDirectoryName(executionPath);
            DirectoryInfo auxDir = new DirectoryInfo(executionDir);
            for (int i = 0; i < BaseConfigurationManager.LevelsUp; i++)
            {
                string auxPath = auxDir.FullName;
                auxDir = Directory.GetParent(auxPath);
            }

            //Get configuration folder where the general configuration file is
            basePath = Path.GetFullPath(auxDir.FullName);
            configPath = Path.Combine(basePath, BaseConfigurationManager.ConfigFolder);
            generalConfigPath = Path.Combine(configPath, BaseConfigurationManager.ConfigFile);

            //Process general config file
            var builder = new ConfigurationBuilder();
            DirectoryInfo configDir = new DirectoryInfo(configPath);
            FileInfo[] generalConfigFile = configDir.GetFiles(BaseConfigurationManager.ConfigFile);
            switch (generalConfigFile[0].Extension)
            {
                case Constants.INI:
                    builder.AddIniFile(generalConfigFile[0].FullName);
                    break;
                case Constants.XML:
                    builder.AddXmlFile(generalConfigFile[0].FullName);
                    break;
                case Constants.JSON:
                    builder.AddJsonFile(generalConfigFile[0].FullName);
                    break;
                default:
                    throw new ConfigurationErrorsException("General config file is not .ini, .xml or .json.");
            }

            //Load all types of configuration files   

            var configuration = builder.Build();
            var configFiles = configuration.GetSection("configFile").GetChildren();
            foreach (var configFile in configFiles)
            {
                string file = Path.GetFullPath(configFile["file"]);
                if (configFile["name"] == "References")
                {
                    ReferencesFilePath = file;
                }
                
                bool optional = Convert.ToBoolean(configFile["optional"]);
                bool reloadOnChange = Convert.ToBoolean(configFile["ReloadOnChange"]);
                string path = Path.GetDirectoryName(file);
                DirectoryInfo dir = new DirectoryInfo(path);
                string Filename = Path.GetFileName(file);
                FileInfo[] fiConfigFile = dir.GetFiles(Filename);
                try
                {
                    switch (fiConfigFile[0].Extension)
                    {
                        case Constants.INI:
                            builder.AddIniFile(fiConfigFile[0].FullName, optional, reloadOnChange);
                            break;
                        case Constants.XML:
                            builder.AddXmlFile(fiConfigFile[0].FullName, optional, reloadOnChange);
                            break;
                        case Constants.JSON:
                            builder.AddJsonFile(fiConfigFile[0].FullName, optional, reloadOnChange);
                            break;
                        case Constants.TXT:
                            builder.AddTextFile(fiConfigFile[0].FullName, optional, reloadOnChange);

                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Loading configuration file: {0} {1} # {2}", file, ex.StackTrace, ex.Message));
                }
            }
            try
            {
                configuration = builder.Build();
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Loading configuration files: {0} # {1}", ex.StackTrace, ex.Message));
            }

            #region Instruments
            DevicesConfig = new Dictionary<string, IDeviceConfig>();

            //Configure instruments
            if (configuration.GetSection("instruments").Exists())
            {
                var instrumentsConfigurations = configuration.GetSection("instruments").GetChildren();

                foreach (var instrumentConfig in instrumentsConfigurations)
                {
                    string deviceName = instrumentConfig["DeviceName"];
                    string name = instrumentConfig["name"];
                    CommChannelType commChannelType = (CommChannelType)Enum.Parse(typeof(CommChannelType), instrumentConfig["CommChannelType:name"]);
                    InstrumentType instrumentType = (InstrumentType)Enum.Parse(typeof(InstrumentType), instrumentConfig["instrumenttype"]);
                    switch (instrumentType)
                    {
                        case InstrumentType.Multiplexer:
                            MultiplexerType multiplexerType = (MultiplexerType)Enum.Parse(typeof(MultiplexerType), instrumentConfig["Type"]);
                            string topology = instrumentConfig["Topology"];
                            int monitoringRefreshTime = 500;
                            if (!int.TryParse(instrumentConfig["MonitoringRefreshTime"], out monitoringRefreshTime))
                                log.Error(String.Format("Error InstrumentName:{0} MonitoringRefreshTime TryParse={1}", name, instrumentConfig["MonitoringRefreshTime"]));
                            DeviceConfigMultiplexer multiplexerConfiguration = new DeviceConfigMultiplexer(deviceName, name, instrumentType, multiplexerType, topology, monitoringRefreshTime);
                            if (!DevicesConfig.ContainsKey(name))
                                DevicesConfig.Add(multiplexerConfiguration.Name, multiplexerConfiguration);
                            else
                                log.Warn(String.Format("Device already configured: {0}", name));
                            break;
                        case InstrumentType.PWM:
                            DeviceConfigPWM pwmConfiguration = new DeviceConfigPWM(deviceName, name, instrumentType);
                            if (!DevicesConfig.ContainsKey(name))
                                DevicesConfig.Add(pwmConfiguration.Name, pwmConfiguration);
                            else
                                log.Warn(String.Format("Device already configured: {0}", name));
                            break;
                        default:
                            //SCPI configuration
                            Dictionary<string, DeviceMethod> interfaces = new Dictionary<string, DeviceMethod>();
                            var instrumentInterfaces = instrumentConfig.GetSection("interface").GetChildren();
                            foreach (var instrumentInterface in instrumentInterfaces)
                            {
                                if (!interfaces.ContainsKey(instrumentInterface["method"].ToLowerInvariant()))
                                    interfaces.Add(instrumentInterface["method"].ToLowerInvariant(), new DeviceMethod(instrumentInterface["method"].ToLowerInvariant(), instrumentInterface["commands"].ToLowerInvariant(), instrumentInterface["param"]));
                                else
                                    log.Warn(String.Format("Method already configured: {0} on device {1}", instrumentInterface["method"], name));
                            }
                            Dictionary<string, string> commands = new Dictionary<string, string>();
                            var instrumentCommands = instrumentConfig.GetSection("commands").GetChildren();
                            foreach (var instrumentCommand in instrumentCommands)
                            {
                                if (!commands.ContainsKey(instrumentCommand["name"].ToLowerInvariant()))
                                    commands.Add(instrumentCommand["name"].ToLowerInvariant(), instrumentCommand["command"]);
                                else
                                    log.Warn(String.Format("SCPI command already configured: {0} on device {1}", instrumentCommand["name"], name));
                            }
                            string address = instrumentConfig["commChannelType:configuration:address"];
                            DeviceConfigSCPI scpiConfiguration;
                            if (instrumentType == InstrumentType.Multimeter)
                            {
                                List<double> rangeDCVoltage = instrumentConfig["RangeDCVoltage"]?.Replace(".", ",").Split(' ').Select(double.Parse).ToList();
                                List<double> rangeDCCurrent = instrumentConfig["RangeDCCurrent"]?.Replace(".", ",").Split(' ').Select(double.Parse).ToList();
                                List<double> rangeRes = instrumentConfig["RangeRes"]?.Replace(".", ",").Split(' ').Select(double.Parse).ToList();
                                List<double> rangeFRes = instrumentConfig["RangeFRes"]?.Replace(".", ",").Split(' ').Select(double.Parse).ToList();
                                List<double> nplc = instrumentConfig["NPLC"]?.Replace(".", ",").Split(' ').Select(double.Parse).ToList();

                                scpiConfiguration = new DeviceConfigMultimeter(deviceName, name, address, commChannelType, instrumentType, interfaces, commands, rangeDCVoltage, rangeDCCurrent, rangeRes, rangeFRes, nplc);
                            }
                            else
                            {
                                scpiConfiguration = new DeviceConfigSCPI(deviceName, name, address, commChannelType, instrumentType, interfaces, commands);
                            }
                            if (!DevicesConfig.ContainsKey(name))
                                DevicesConfig.Add(scpiConfiguration.Name, scpiConfiguration);
                            else
                                log.Warn(String.Format("Device already configured: {0}", name));
                            break;
                    }
                }
            }
            #endregion

            #region PLC Config
            PlcConfigs = new List<PlcConfig>();
            if (configuration.GetSection("PlcConfig").Exists())
            {
                var plcConfigs = configuration.GetSection("PlcConfig").GetChildren();
                foreach (var plcConfig in plcConfigs)
                {
                    string plcName = plcConfig["Name"];
                    string plcIp = plcConfig["Ip"];
                    string plcPort = plcConfig["Port"];
                    string plcType = plcConfig["Type"];
                    int plcMonitoringRefreshTime = int.Parse(plcConfig["MonitoringRefreshTime"]);
                    bool plcIsSimulated = Convert.ToBoolean(plcConfig["IsSimulated"]);

                    var tagConfigs = plcConfig.GetSection("SiemensAddress").GetChildren();
                    Dictionary<string, PlcSiemensAddress> tags = new Dictionary<string, PlcSiemensAddress>();

                    foreach (var tagConfig in tagConfigs)
                    {
                        string tagName = tagConfig["Name"];
                        string tagType = tagConfig["Type"];
                        string tagMemoryAddress = tagConfig["MemoryAddress"];
                        bool tagUsedInThread = Convert.ToBoolean(tagConfig["UsedInThread"]);
                        PlcSiemensAddress tag = new PlcSiemensAddress
                        {
                            Description = tagName,
                            Type = tagType,
                            MemoryAddress = tagMemoryAddress,
                            UsedInThread = tagUsedInThread
                        };
                        tags.Add(tagName, tag);
                    }
                    PlcConfigs.Add(new PlcConfig(plcName, plcIp, plcPort, plcType, plcIsSimulated, plcMonitoringRefreshTime, tags));
                }
            }
            #endregion

            //#region Collector Paths
            //DatabaseConfigs = new List<DbConfig>();
            //if (configuration.GetSection("DataBaseConfig").Exists())
            //{
            //    var DBConfigs = configuration.GetSection("DataBaseConfig").GetChildren();
            //    foreach (var DBConfig in DBConfigs)
            //    {
            //        string Name = DBConfig["Name"];
            //        string Ip = DBConfig["Ip"];
            //        string Server = DBConfig["Server"];
            //        string Database = DBConfig["Database"];
            //        string User = DBConfig["User"];
            //        string Password = DBConfig["Password"];
                   

            //        var DBCellConfigs = DBConfig.GetSection("CellConfig").GetChildren();
            //        Dictionary<string, List<string>> CellsParam = new Dictionary<string, List<string>>();
            //        foreach (var CellData in DBCellConfigs)
            //        {
            //            List<string> ParameterList = new List<string>();
            //            string CellName = CellData["Name"];
            //            int parameters = Convert.ToInt32(CellData["Parameters"]);
            //            for (int i = 1; i <= parameters; i++)
            //            {
            //                ParameterList.Add(CellData["Parameter_"+i]);
            //            }
            //            CellsParam.Add(CellName, ParameterList);
            //        }

            //        var DBQuerys = DBConfig.GetSection("QueryConfig").GetChildren();
            //        Dictionary<string, List<string>> Querylist = new Dictionary<string, List<string>>();
            //        foreach (var Query in DBQuerys)
            //        {
            //            List<string> QueryItem = new List<string>();
            //            string QueryName = Query["Name"];
            //            QueryItem.Add(Query["Query"]);
            //            QueryItem.Add(Query["MainTable"]);
            //            Querylist.Add(QueryName, QueryItem);
            //        }

            //        DatabaseConfigs.Add(new DbConfig(Name, Ip, Server, Database, User, Password, CellsParam, Querylist));
            //    }
            //}
            //#endregion

            #region Database Config
            CollectorPaths = new Dictionary<string, string>();
            if (configuration.GetSection("CollectorPath").Exists())
            {
                var Collectorpaths = configuration.GetSection("CollectorPath").GetChildren();
                foreach (var Collectorpath in Collectorpaths)
                {
                    string CellName = Collectorpath["Name"];
                    string CellPathFolder = Collectorpath["Path"];
                    try
                    {
                        CollectorPaths.Add(CellName, CellPathFolder);
                    }
                    catch
                    {
                        log.Error(String.Format("The collector path for cell" + CellName + " is already set"));
                    }
                }
            }
            #endregion

            #region References
            try
            {
                References = configuration.GetSection("References");
                ReportsPath = configuration["Reports:Path"];
                TestOrderCheck = configuration.GetSection("TEST_ORDER_CHECK");
            }
            catch (Exception ex)
            {
                log.Error(String.Format("References Load: {0} # {1}", ex.StackTrace, ex.Message));
            }
            #endregion

            #region Ref_Layout
            RefLayoutPath = configuration["Ref_Layout:Path"];
            #endregion


            /*#region Settings
            try
            {
                Settings = new Dictionary<TestType, List<SettingAction>>();
                if (configuration.GetSection("Test").Exists())
                {
                    var settingsConfiguration = configuration.GetSection("Test").GetChildren();
                    foreach (var test in settingsConfiguration)
                    {
                        List<SettingAction> actions = new List<SettingAction>();
                        TestType testType = (TestType)Enum.Parse(typeof(TestType), test["Name"]);
                        foreach (var action in test.GetSection("Action").GetChildren())
                        {
                            actions.Add(new SettingAction()
                            {
                                Order = int.Parse(action["name"]),
                                Method = action["Method"],
                                Device = action["Device"],
                                Variable = action["Variable"],
                                VariableType = action["VariableType"] != null ? action["VariableType"] : "Int",
                                Value = action["Value"],
                                TimeOut = action["TimeOut"] != null ? action["TimeOut"] : "20"
                            });
                        }
                        actions = actions.OrderBy(x => x.Order).ToList();
                        Settings.Add(testType, actions);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Settings Load: {0} # {1}", ex.StackTrace, ex.Message));
            }
            #endregion*/

            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }
    }
}
