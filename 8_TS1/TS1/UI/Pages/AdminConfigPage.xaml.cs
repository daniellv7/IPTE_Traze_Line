using System.Windows;
using System.Windows.Controls;
using Ipte.Machine;
using Ipte.Machine.Configuration;

namespace Ipte.UI.Pages
{
    /// <summary>
    /// Interaction logic for AdminConfigPage.xaml
    /// </summary>
    public partial class AdminConfigPage : UserControl
    {
        public AdminConfigPage()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            editor.SetObject(Controller.Instance.Settings, "Settings");
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            editor.Refresh();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            Controller.Instance.Settings.Save(Paths.MachineSettingsFile);
        }

        private void FilterUpdated(object sender, RoutedEventArgs e)
        {
            editor.SetFilter(txtFilter.Text);
        }
    }
}