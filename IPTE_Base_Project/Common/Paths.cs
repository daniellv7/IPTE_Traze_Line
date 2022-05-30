using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTE_Base_Project.Common
{
    class Paths
    {
        #region Directories
        private static string _settingsDirectory;
        public static string SettingsDirectory
        {
            get
            {
                if (_settingsDirectory == null)
                {
                  //  string PFolder = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
                    string PFolder = AppDomain.CurrentDomain.BaseDirectory;
                    _settingsDirectory = PFolder + "Config";
                    if (!Directory.Exists(_settingsDirectory))
                    {
                        Directory.CreateDirectory(_settingsDirectory);
                    }
                }

                return _settingsDirectory;
            }
        }

        public static string ProyectDataDirectory
        {
            get
            {
                if (_settingsDirectory == null)
                {
                    //  string PFolder = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
                    string PFolder = AppDomain.CurrentDomain.BaseDirectory;
                    _settingsDirectory = PFolder + "Data";
                    if (!Directory.Exists(_settingsDirectory))
                    {
                        Directory.CreateDirectory(_settingsDirectory);
                    }
                }

                return _settingsDirectory;
            }
        }

        public static string MachineSettingsFile
        {
            get
            {
                return SettingsDirectory + "\\settings.cfg";
            }
        }

        public static string DBQueryList
        {
            get
            {
                return SettingsDirectory + "\\QueryList.xml";
            }
        }

        public static string CellDBQueryList
        {
            get
            {
                return SettingsDirectory + "\\CellQueryList.xml";
            }
        }

        public static string CollectorFolderList
        {
            get
            {
                return SettingsDirectory + "\\CollectorList.xml";
            }
        }

        public static string IOList
        {
            get
            {
                return SettingsDirectory + "\\IOList.xml";
            }
        }

        public static string ErrorList
        {
            get
            {
                return SettingsDirectory + "\\ErrorList.xml";
            }
        }

        public static string MachineErrorList
        {
            get
            {
                return SettingsDirectory + "\\MachineErrorList.xml";
            }
        }

        public static string ErrorList_0
        {
            get
            {
                return SettingsDirectory + "\\ErrorList_0.xml";
            }
        }

        public static string MachineErrorList_0
        {
            get
            {
                return SettingsDirectory + "\\MachineErrorList_0.xml";
            }
        }

        public static string ProcessMessage
        {
            get
            {
                return SettingsDirectory + "\\ProcessMessages.xml";
            }
        }

        public static string ProcessMessage_0
        {
            get
            {
                return SettingsDirectory + "\\ProcessMessages_0.xml";
            }
        }

        public static string ParameterList
        {
            get
            {
                return SettingsDirectory + "\\ParameterList.xml";
            }
        }

        public static string ItemFolder
        {
            get
            {
                return SettingsDirectory + "\\ParameterList.xml";
            }
        }

        public static string VariantsDB
        {
            get
            {
                return SettingsDirectory + "\\Variants.sqlite";
            }
        }

        public static string PrinterFIle
        {
            get
            {
                return SettingsDirectory + "\\PrinterFile.txt";
            }
        }

        public static string ProcessErrorFIle
        {
            get
            {
                return SettingsDirectory + "\\ProcessErrorList.xml";
            }
        }

        public static string BackupPath
        {
            get
            {
                return "D:";
            }
        }

        public static string BypassListPath
        {
            get
            {
                return SettingsDirectory + "\\BypassNameList.xml";
            }
        }

        public static string QueryList
        {
            get
            {
                return SettingsDirectory + "\\CellQueryList.xml";
            }
        }

        public static string ImageCell
        {
            get
            {
                string PFolder = AppDomain.CurrentDomain.BaseDirectory;
                //string PFolder = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
                PFolder += "\\Common\\Resources\\Images";
                return PFolder;
            }
        }
        public static string ImageProcess
        {
            get
            {
               // string PFolder = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
                string PFolder = AppDomain.CurrentDomain.BaseDirectory;
                PFolder += "\\Common\\Resources\\Images\\Process";
                return PFolder;
            }
        }
        #endregion
    }
}
