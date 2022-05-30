using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ipte.TS1.UI.Analytics;

namespace IPTE_Base_Project.Models
{
    public class AnaliticsModel : BaseModel
    {
        public ObservableCollection<string> Cells { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> Variants { get; set; } = new ObservableCollection<string>();

        public MachineEventsChart Chart { get; set; } = new MachineEventsChart();

        public ItemEventsChart ItemChart { get; set; } = new ItemEventsChart();

        public DateTime StartTime { get; set; } = new DateTime();

        public DateTime EndTime { get; set; } = new DateTime();

        public int SelectedCell { get; set; } = 0;
        
        private bool _updating = true;
        public bool Updating
        {
            get
            {
                return _updating;
            }
            set
            {
                _updating = value;
                OnPropertyChanged();
            }
        }

        private bool _buttonenable = true;
        public bool ButtonEnable
        {
            get
            {
                return _buttonenable;
            }
            set
            {
                _buttonenable = value;
                OnPropertyChanged();
            }
        }

        public AnaliticsModel()
        {
            SetCellList();
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
        }

        private void GetVariantList()
        {

        }

        private void SetCellList()
        {
            Cells.Add("All");
            Cells.Add("100");
            Cells.Add("100B");
            Cells.Add("150");
            Cells.Add("200");
            Cells.Add("300");
            Cells.Add("350");
            Cells.Add("400");
            Cells.Add("500");
            Cells.Add("600");
            Cells.Add("700");
            Cells.Add("750");
            Cells.Add("800");
            Cells.Add("850");
            Cells.Add("1000");
            Cells.Add("1200");
            Cells.Add("2000");
        }
    }

}

             