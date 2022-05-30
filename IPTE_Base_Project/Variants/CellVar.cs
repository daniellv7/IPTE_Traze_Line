using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IPTE_Base_Project.Models;


namespace IPTE_Base_Project.Variants
{
   public  class CellVar
    {
        int _cell;
        public int Cell
        {
            get
            {
                return _cell;
                
            }

            set
            {
                _cell = value;
              //  OnPropertyChanged("Cell");
            }
        }

        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get
            {
               
                return _timestamp;
            }
            set
            {

                _timestamp = value;
                //OnPropertyChanged("Timestamp");
            }
        }

        private bool _alive;    
        public bool Alive
        {
            get
            {
              
                return _alive;
            }
            set
            {
                _alive = value;
              //  OnPropertyChanged();
            }
        }

        int _machinestate;
        public int MachineState
        {
            get
            {
                return _machinestate;

            }

            set
            {
                _machinestate = value;
                //  OnPropertyChanged("Cell");
            }
        }

        private bool _itacconnection;
        public bool ITACConnection { get; set; }

        public CellVar()
        {
            
        }

        
    }
}
