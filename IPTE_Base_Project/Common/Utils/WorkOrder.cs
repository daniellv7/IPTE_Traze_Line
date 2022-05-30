using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTE_Base_Project.Common.Utils
{
    public class WorkOrder
    {
        // Immutable struct pattern
        public string InfoTxt1 { get; }
        public string InfoTxt2 { get; }
        public string PartNumber { get; }
        public string WorkOrderNumber { get; }

        public WorkOrder(string workOrderNumber, string partNumber, string infoTxt1, string infoTxt2)
        {
            InfoTxt1 = infoTxt1;
            InfoTxt2 = infoTxt2;
            PartNumber = partNumber;
            WorkOrderNumber = workOrderNumber;
        }
    }
}
