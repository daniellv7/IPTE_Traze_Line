using System.Deployment.Application;
using System.Windows;
using Ipte.TS1.UI.i18n;

namespace Ipte.UI
{
    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    public partial class AboutBox
    {
        public static DependencyProperty SoftwareVersionProperty = DependencyProperty.Register(
            "SoftwareVersion", typeof(string), typeof(AboutBox));

        public static DependencyProperty MachineTypeProperty = DependencyProperty.Register(
            "MachineType", typeof(string), typeof(AboutBox));

        public static DependencyProperty SupportNumberProperty = DependencyProperty.Register(
            "SupportNumber", typeof(string), typeof(AboutBox));

        public AboutBox()
        {
            SupportNumber = "";

           try
            {
                //SoftwareVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch (InvalidDeploymentException)
            {
                SoftwareVersion = this.i18nTranslate("N/A");
            }

            MachineType = Application.ResourceAssembly.GetName().Name;
            this.InitializeComponent();
        }

        public string SoftwareVersion
        {
            get
            {
                return (string)GetValue(SoftwareVersionProperty);
            }
            set
            {
                SetValue(SoftwareVersionProperty, value);
            }
        }

        public string MachineType
        {
            get
            {
                return (string)GetValue(MachineTypeProperty);
            }
            set
            {
                SetValue(MachineTypeProperty, value);
            }
        }


        public string SupportNumber
        {
            get
            {
                return (string)GetValue(SupportNumberProperty);
            }
            set
            {
                SetValue(SupportNumberProperty, value);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
