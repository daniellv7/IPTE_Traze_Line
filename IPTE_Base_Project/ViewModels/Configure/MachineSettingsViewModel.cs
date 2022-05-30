using IPTE_Base_Project.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using IPTE_Base_Project.Common.Utils.MediatorPattern;

namespace IPTE_Base_Project.ViewModels.Configure
{
    public class MachineSettingsViewModel : BaseViewModel
    {
        public MachineSettings Settings { get; set; }
        public MachineSettings settingsSnapshot { get; set; }
        public MachineSettingsViewModel(MachineSettings settings)
        {
            Settings = settings;
            settingsSnapshot = Settings.Clone() as MachineSettings;
            OnPropertyChanged("Settings");

        }

        public ICommand SelectFolder
        {
            get
            {
                if (selectfolder == null)
                {

                    selectfolder = new RelayCommand(param => FolderSelect(param), CanExe);
                }
                return selectfolder;
            }
        }
        private RelayCommand selectfolder;

        public ICommand ReloadSettings
        {
            get
            {
                if (reloadsettings == null)
                {

                    reloadsettings = new RelayCommand(param => Reload(param), CanExe);
                }
                return reloadsettings;
            }
        }
        private RelayCommand reloadsettings;

        public ICommand SaveSettings
        {
            get
            {
                if (savesettings == null)
                {

                    savesettings = new RelayCommand(param => Save(param), CanExe);
                }
                return savesettings;
            }
        }
        private RelayCommand savesettings;

        public ICommand SelectLanguage
        {
            get
            {
                if (seleclanguage == null)
                {

                    seleclanguage = new RelayCommand(param => Language(param), CanExe);
                }
                return seleclanguage;
            }
        }
        private RelayCommand seleclanguage;

        private void Language(object o)
        {
            var Lang = Int32.Parse((string)o);
            settingsSnapshot.Station_Settings.Language = Lang;
        }

        public bool CanExe(object o)
        {
            return true;
        }

        public void FolderSelect(object o)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            string folderpath = dialog.SelectedPath;
            switch ((string)o)
            {
                //case "collector":
                //    if (folderpath != null && folderpath != "")
                //    {
                //        settingsSnapshot.Reporter_settings.Path = folderpath;
                //        OnPropertyChanged("settingsSnapshot");
                //    }
                //    break;
                //case "Archive":
                //    if (folderpath != null && folderpath !="")
                //    {
                //        settingsSnapshot.Db_settings.Report_Save_Path = folderpath;
                //        OnPropertyChanged("settingsSnapshot");
                //    }
                //    break;
                //case "Storage":
                //    if (folderpath != null && folderpath != "")
                //    {
                //        settingsSnapshot.Db_settings.Storage_Path = folderpath;
                //        OnPropertyChanged("settingsSnapshot");
                //    }
                //    break;
                //case "EOL":
                //    if (folderpath != null && folderpath != "")
                //    {
                //        settingsSnapshot.Db_settings.EOL_Storage_Path = folderpath;
                //        OnPropertyChanged("settingsSnapshot");
                //    }
                //    break;
                default:
                    //settingsSnapshot.Plc_settings.IP
                    break;
            }
        }

        private void Reload(object o)
        {
            settingsSnapshot = Settings.Clone() as MachineSettings;
            OnPropertyChanged("settingsSnapshot");
        }

        private void Save(object o)
        {
            Settings = settingsSnapshot;
            Settings.Save(Paths.MachineSettingsFile);
            Mediator.NotifyColleagues("OpenPopUp", new object[] { "Saved" });
            OnPropertyChanged("settingsSnapshot");
        }
    }
}
