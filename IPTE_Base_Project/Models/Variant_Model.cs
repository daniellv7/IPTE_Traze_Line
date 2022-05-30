using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Collections.Specialized;

namespace IPTE_Base_Project.Models
{
    public class Variant_Model : BaseModel
    {
        public Dictionary<string, object> VariantS = new Dictionary<string, object>();
        private int _copyto;
        public int Copyto
        {
            get { return _copyto; }
            set
            {
                _copyto = value;
                OnPropertyChanged();
            }
        }

        public Variant_Model()
        {
           
          
        }
        
    }
}
