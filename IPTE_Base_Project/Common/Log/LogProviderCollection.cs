using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTE_Base_Project.Common.Log
{
    public class LogProviderCollection : ProviderCollection
    {
        new public LogProviderBase this[string name]
        {
            get { return (LogProviderBase)base[name]; }
        }
    }
}
