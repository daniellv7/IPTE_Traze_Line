using IPTE_Base_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTE_Base_Project.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {
        private SettingsModel model;
        
        public SettingsModel Model
        {
            get
            {
                return model;
            }

            set
            {
                model = value;
            }
        }

        public SettingsViewModel()
        {
            Model = new SettingsModel();
            Model.Load();

            //Set the Log level for the application
            SetDebugLevel(Model.debugLevel);
        }
    }
}
