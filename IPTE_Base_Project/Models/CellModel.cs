using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IPTE_Base_Project.Common;
using IPTE_Base_Project.Variants;

namespace IPTE_Base_Project.Models
{
    public class CellModel: BaseModel
    {
        private string _celllabel="Trace Line";
        public string CellLabel
        {
            get
            {
                return _celllabel;
            }
            set
            {
                _celllabel = value;
                OnPropertyChanged();
            }
        }

        private string _imagepath ;//@"C:\Users\CNG1402\Documents\Proyectos\Line_PC_B\Line_PC\IPTE_Base_Project\Common\Resources\Images\Cell1200.png";
        public string ImagePath
        {
            get
            {
                return _imagepath;
            }
            set
            {
                _imagepath = value;
                OnPropertyChanged();
            }
        }

        public bool Monitoring;

        private bool _plcactive;
        public bool PLCActive
        {
            get
            {
                return _plcactive;
            }
            set
            {
                _plcactive = value;
                OnPropertyChanged();
            }
        }

        private bool _serveractive;
        public bool ServerActive
        {
            get
            {
                return _serveractive;
            }
            set
            {
                _serveractive = value;
                OnPropertyChanged();
            }
        }

        private bool _printeractive;
        public bool PrinterActive
        {
            get
            {
                return _printeractive;
            }
            set
            {
                _printeractive = value;
                OnPropertyChanged();
            }
        }

        private bool _searchactive;
        public bool SearchActive
        {
            get
            {
                return _searchactive;
            }
            set
            {
                _searchactive = value;
                OnPropertyChanged();
            }
        }

        private bool _itacconnection;
        public bool ITACConnection
        {
            get
            {
                return _itacconnection;
            }
            set
            {
                _itacconnection = value;
                OnPropertyChanged();
            }
        }

        private bool _plc_1;
        public bool PLC_1
        {
            get
            {
                return _plc_1;
            }
            set
            {
                _plc_1 = value;
                OnPropertyChanged();
            }
        }

        public DateTime ServerTime { get; set; }

    }
}
