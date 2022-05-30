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
using Ipte.TS1.UI.Analytics;

namespace IPTE_Base_Project.Views.Analize
{
    /// <summary>
    /// Interaction logic for EventView.xaml
    /// </summary>
    public partial class EventView : UserControl
    {
        public EventView()
        {
            var TTime = DateTime.Now;
            InitializeComponent();
        }

        private void GuiButton_Click(object sender, RoutedEventArgs e)
        {
           
            View_Event_table.Update();
          

        }

        private void GuiComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            switch (Mode_Selection.SelectedIndex)
            {
                case 0:
                    View_Event_table.Mode = MachineEventChartMode.Frequency;
                    break;
                case 1:
                    View_Event_table.Mode = MachineEventChartMode.Division;
                    break;
                case 2:
                    View_Event_table.Mode = MachineEventChartMode.Density;
                    break;
            }

        }
    }
}
