using IPTE_Base_Project.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IPTE_Base_Project.ViewModels.Configure
{
    public class OtherSettingsViewModel : BaseViewModel
    {
        private string filePath;

        public string FileContent
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public OtherSettingsViewModel(string referenteFilePath)
        {
            filePath = referenteFilePath;
            FileContent = System.IO.File.ReadAllText(referenteFilePath);
        }


        RelayCommand saveFileCommand;
        public ICommand SaveFileCommand
        {
            get
            {
                if (saveFileCommand == null)
                {
                    saveFileCommand = new RelayCommand(param => SaveFile(param), CanExe);
                }
                return saveFileCommand;
            }
        }

        public void SaveFile(object o)
        {
            System.IO.File.WriteAllText(filePath, FileContent);
        }

        public bool CanExe(object o)
        {
            return true;
        }
    }
}
