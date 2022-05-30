using System;
using System.IO;
using System.Windows;

namespace Ipte.Machine.Configuration
{
    public static class Paths
    {
        public static string CellName = Application.ResourceAssembly.GetName().Name; //set the assembly name

        private static string _vendorDirectory;
        private static string _rootDirectory;
        private static string _productDirectory;
        private static string _dataDirectory;
        private static string _settingsDirectory;
        private static string _logDirectory;
        private static string _databaseDirectory;
        private static string _plcDirectory;
        private static string _helpDirectory;

        public static string VendorDirectory
        {
            get
            {
                if (_vendorDirectory == null)
                {
                    _vendorDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\IPTE";
                    if (!Directory.Exists(_vendorDirectory))
                    {
                        Directory.CreateDirectory(_vendorDirectory);
                    }
                }

                return _vendorDirectory;
            }
        }

        public static string RootDirectory
        {
            get
            {
                if (_rootDirectory == null)
                {
                    _rootDirectory = VendorDirectory + "\\" + CellName;
                    if (!Directory.Exists(_rootDirectory))
                    {
                        Directory.CreateDirectory(_rootDirectory);
                    }
                }

                return _rootDirectory;
            }
        }

        public static string ProductDirectory
        {
            get
            {
                if (_productDirectory == null)
                {
                    _productDirectory = RootDirectory + "\\Products";
                    if (!Directory.Exists(_productDirectory))
                    {
                        Directory.CreateDirectory(_productDirectory);
                    }
                }

                return _productDirectory;
            }
        }

        public static string DataDirectory
        {
            get
            {
                if (_dataDirectory == null)
                {
                    _dataDirectory = RootDirectory + "\\Data";
                    if (!Directory.Exists(_dataDirectory))
                    {
                        Directory.CreateDirectory(_dataDirectory);
                    }
                }

                return _dataDirectory;
            }
        }

        public static string LogDirectory
        {
            get
            {
                if (_logDirectory == null)
                {
                    _logDirectory = RootDirectory + "\\Logs";
                    if (!Directory.Exists(_logDirectory))
                    {
                        Directory.CreateDirectory(_logDirectory);
                    }
                }

                return _logDirectory;
            }
        }

        public static string SettingsDirectory
        {
            get
            {
                if (_settingsDirectory == null)
                {
                    _settingsDirectory = RootDirectory + "\\Settings";
                    if (!Directory.Exists(_settingsDirectory))
                    {
                        Directory.CreateDirectory(_settingsDirectory);
                    }
                }

                return _settingsDirectory;
            }
        }

        public static string DatabaseDirectory
        {
            get
            {
                if (_databaseDirectory == null)
                {
                    _databaseDirectory = RootDirectory + "\\Databases";
                    if (!Directory.Exists(_databaseDirectory))
                    {
                        Directory.CreateDirectory(_databaseDirectory);
                    }
                }

                return _databaseDirectory;
            }
        }

        public static string PlcDirectory
        {
            get
            {
                if (_plcDirectory == null)
                {
                    _plcDirectory = RootDirectory + "\\PLC";
                    if (!Directory.Exists(_plcDirectory))
                    {
                        Directory.CreateDirectory(_plcDirectory);
                    }
                }

                return _plcDirectory;
            }
        }

        public static string HelpDirectory
        {
            get
            {
                if (_helpDirectory == null)
                {
                    _helpDirectory = RootDirectory + "\\Help";
                    if (!Directory.Exists(_helpDirectory))
                    {
                        Directory.CreateDirectory(_helpDirectory);
                    }
                }

                return _helpDirectory;
            }
        }

        public static string MachineSettingsFile
        {
            get
            {
                return SettingsDirectory + "\\settings.cfg";
            }
        }

        public static string StatisticsDatabase
        {
            get
            {
                return DatabaseDirectory + "\\analytics2.sqlite";
            }
        }
    }
}
