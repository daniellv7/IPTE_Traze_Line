using IPTE_Base_Project.Common.Utils.MediatorPattern;
using Ipte.TS1.UI.i18n;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace IPTE_Base_Project.Views.Analize
{
    /// <summary>
    /// Lógica de interacción para CellStatistics.xaml
    /// </summary>
    public partial class ErrorEvents : UserControl
	{
		public ErrorEvents()
		{
			InitializeComponent();
            Mediator.Register("EventChartUpdate", OnUpdateGraph);
            Mediator.Register("EventChartMessageUpdate", MessageBoxShow);
        }

        public void OnUpdateGraph(object[] param)
        {            
            Dispatcher.Invoke(new Action(() =>
            {
                ErrorEventChart.ItemsSource = (KeyValuePair<string,long>[])param[0];
            }));
        }

        private void MessageBoxShow(object[] param)
        {
            MessageBox.Show(this.i18nTranslate((string)param[0]));
        }

    }
}
