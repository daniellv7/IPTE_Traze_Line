using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using IPTE_Base_Project.Common.Utils.MediatorPattern;

namespace IPTE_Base_Project.Views.Analize
{
    /// <summary>
    /// Interaction logic for ItemView.xaml
    /// </summary>
    public partial class ItemView : UserControl
    {
        public ItemView()
        {
            InitializeComponent();
         
        }

        private void GuiButton_Click(object sender, RoutedEventArgs e)
        {
            Mediator.NotifyColleagues("OnUpdateCellData", new object[] {  });
            var AvailablRecipes = View_Event_table2.AvailableRecipes;
            View_Event_table2.SelectedDevice = SelectedFilter.Text;
            SelectedRecipe.ItemsSource = AvailablRecipes;
            View_Event_table2.Update();
        }
    }
}
