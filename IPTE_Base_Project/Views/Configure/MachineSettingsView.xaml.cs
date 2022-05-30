using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IPTE_Base_Project.Variants;
using IPTE_Base_Project.Common;
using Microsoft.Win32;
using Winforms = System.Windows.Forms;

namespace IPTE_Base_Project.Views.Configure
{
    /// <summary>
    /// Lógica de interacción para MachineSettingsView.xaml
    /// </summary>
    public partial class MachineSettingsView : UserControl
    {
        private  MachineSettings SettingsV;
        public MachineSettingsView()
        {
           
            InitializeComponent();
            //Initialize();


        }
       
        public void Initialize(MachineSettings Settings)
        {
           SettingsV = Settings;
           //editor.SetObject(SettingsV, "Settings");
        }

        public void Refresh()
        {
         //   editor.Refresh();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            SettingsV.Save(Paths.MachineSettingsFile);
        }

        //private void Save(object sender, RoutedEventArgs e)
        //{
        //    Controller.Instance.Settings.Save(Paths.MachineSettingsFile);
        //}

        private void FilterUpdated(object sender, RoutedEventArgs e)
        {
          // editor.SetFilter(Searchtext.Text);
        }

        private void GuiButton_Click(object sender, RoutedEventArgs e)
        {
          

        }
    }
}
