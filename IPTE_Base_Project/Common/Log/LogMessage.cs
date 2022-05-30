using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTE_Base_Project.Common
{
    public class LogMessage
    {
        public string Message { get; set; }

        public LogMessage(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
