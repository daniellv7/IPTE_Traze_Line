using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTE_Base_Project.Common.Utils
{
    /// <summary>
    /// Data container for data sent in the mediator pattern
    /// In this case is mainly used to exchange data between PlcViewModel and SequenceViewModel
    /// </summary>
    public class MessageData
    {
        public int NScrews { get; set; }

        public float CycleTime { get; set; }

        public string[] Torques { get; set; }

        public string[] Angles { get; set; }

        public string[] screwTorqueBooleanResult { get; set; }

        public string[] screwAngleBooleanResult { get; set; }

    }
}
