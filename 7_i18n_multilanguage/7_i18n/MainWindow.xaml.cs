using System;
using System.Windows;
using Ipte.TS1.UI.i18n;

namespace _7_i18n
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string hola = this.i18nTranslate("xxx1");
            MessageBox.Show(hola);

            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.SetCultureCommand.Execute("et");
        }
    }
}
