using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IPTE_Base_Project.Common;
using IPTE_Base_Project.Models;
using IPTE_Base_Project.Common.Utils.MediatorPattern;

namespace IPTE_Base_Project.ViewModels.Analitics
{
    public class AnaliticsViewModel:BaseViewModel
    {
        public AnaliticsModel Model { get; set; }

        public AnaliticsViewModel(AnaliticsModel model)
        {
            Model = model; 
            
        }

        public ICommand UpdateGraphs
        {
            get
            {
                if (updategraphs == null)
                {

                    updategraphs = new RelayCommand(param => UpdateGraph(param), CanExe);
                }
                return updategraphs;
            }
        }
        private RelayCommand updategraphs;

        public ICommand UpdateItemGraphs
        {
            get
            {
                if (updateitemgraphs == null)
                {

                    updateitemgraphs = new RelayCommand(param => UpdateitemGraph(param), CanExe);
                }
                return updateitemgraphs;
            }
        }
        private RelayCommand updateitemgraphs;

        public ICommand UpdateGraphCommand
        {
            get
            {
                if (updateGraphCommand == null)
                {
                    updateGraphCommand = new RelayCommand(param => UpdateGraph(param), CanExe);
                }
                return updateGraphCommand;
            }
        }
        private RelayCommand updateGraphCommand;

        public ICommand ExportDataCommand
        {
            get
            {
                if (exportdatacommand == null)
                {
                    exportdatacommand = new RelayCommand(param => ExportDarta(param), CanExe);
                }
                return exportdatacommand;
            }
        }
        private RelayCommand exportdatacommand;


        public bool CanExe(object o)
        {
            return true;
        }

        public void UpdateGraph(object o)
        {
            string ParamName = (string)o;
            Model.ButtonEnable = false;
            string Selected = Model.Cells[Model.SelectedCell];
            Model.Chart.StartTime = Model.StartTime;
            Model.Chart.EndTime = Model.EndTime;
            Model.Updating = true;
            switch(ParamName)
            {
                case "Machine":
                    Mediator.NotifyColleagues("OnUpdateData", new object[] { Selected, Model.StartTime, Model.EndTime });
                    break;

                case "Process":
                    Mediator.NotifyColleagues("OnUpdateDataProcess", new object[] { Selected, Model.StartTime, Model.EndTime });
                    break;

                default:
                    break;
            }
           
            Model.Updating = false;
            //Model.Chart.Update();
            Model.ButtonEnable = false;
            //OnPropertyChanged("Model");
        }

        public void UpdateitemGraph(object o)
        {
            Model.ButtonEnable = false;
            string Selected = Model.Cells[Model.SelectedCell];
            Model.ItemChart.StartTime = Model.StartTime;
            Model.ItemChart.EndTime = Model.EndTime;
            OnPropertyChanged("Model");
            //Model.ItemChart.Update();
            OnPropertyChanged("Model");
            Model.ButtonEnable = true;
           
        }

        public void ExportDarta(object o)
        {
            string ParamName = (string)o;
            switch (ParamName)
            {
                case "Machine":
                    //Mediator.NotifyColleagues("OnUpdateData", new object[] { Selected, Model.StartTime, Model.EndTime });
                    break;

                case "Process":
                   // Mediator.NotifyColleagues("OnUpdateDataProcess", new object[] { Selected, Model.StartTime, Model.EndTime });
                    break;

                default:
                    break;
            }

        }

        private void OnUpdated(object[] o)
        {
            Model.Chart.Update();
            OnPropertyChanged("Model");
        }

    }
}
