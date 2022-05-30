using System;
using System.Reflection;

namespace IPTE_Base_Project.Models
{
    public class ITACModel : BaseModel
    {
        #region Data members and accessors

        public bool Status { get; set; }
        public Station station { get; set; }
        public Serial serial { get; set; }

        public HistoryData historydata { get; set; }

        public Attributes attributes { get; set; }

        public Measure measure { get; set; }

        public ErrorMessage errormessage { get; set; }

        public Spec spec { get; set; }



        //Constants

        #endregion

        public ITACModel()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            station = new Station();
            serial = new Serial();
            historydata = new HistoryData();
            attributes = new Attributes();
            measure = new Measure();
            errormessage = new ErrorMessage();
            spec = new Spec();
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }

        #region Load/Save/Close/Compare

        public class Station
        {
            /// Station Name      
            public string Name { get; set; }

            /// Current Part Number set on station
            public string PartNumber { get; set; }

            /// Process version
            public string ProcessVersion { get; set; }

            /// Current BOM set on station
            public int Bom { get; set; }

            /// Current Part # Layer set on station
            public int Layer { get; set; }

            /// Current WO # set on station
            public string WorkOrder { get; set; }
        }

        public class Serial
        {
            /// Current ID
            public string ID { get; set; }

            /// Panel Serial Number
            public string PanelSN { get; set; }

            /// Current BOM Version based on the Part #
            public int Bom { get; set; }

            /// Current Part # based on serial #
            public string PartNumber { get; set; }

            /// Current Pos # based on serial #
            public int Pos { get; set; }

            /// Current WO # based on serial #
            public string WorkOrder { get; set; }

        }

        public class HistoryData
        {

            //Book Date
            public string BookDate { get; set; }

            //Book state
            public string BookState { get; set; }

            //Sequence Number
            public string SeqNumber { get; set; }

            //Serial number
            public string SerialNumber { get; set; }

            //Serial number Position
            public string SNPosition { get; set; }

            // Station number
            public string StationNumber { get; set; }

            //WorkOrder number
            public string WorkOrderNumber { get; set; }

        }

        public class Attributes
        {
            /// <summary>
            /// Code
            /// </summary>
            public string Code { get; set; }
            /// <summary>
            /// Value
            /// </summary>
            public string Value { get; set; }
            /// <summary>
            /// Error
            /// </summary>
            public string Error { get; set; }
        }

        public class Measure
        {
            /// <summary>
            /// Error Code -> Parent Error
            /// </summary>
            public string ErrorCode { get; set; }
            /// <summary>
            /// Fail Code -> Detailed Error
            /// </summary>
            public string FailCode { get; set; }
            /// <summary>
            /// Name for Measure
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Value for Measure
            /// </summary>
            public string Value { get; set; }
        }

        public class ErrorMessage
        {

            /// IMSAPIDotNet Technical Error code
            public int Code { get; set; }

            /// iTAC Controller Method Name
            public string @Methods { get; set; }

            /// Technical Error Code Description
            public string Description { get; set; }

            /// Current Station
            public string WorkStation { get; set; }

            /// Check if current method executed well or not
            public bool Success { get; set; }

            /// Current time stamp
            public DateTime @TimeStamp { get; set; }

            /// Which level class was released, where Level 0 = Root class, Level 1 = Helper
            public int Level { get; set; }
        }

        public class Spec
        {
            /// Name -> Name of the spec
            public string Name { get; set; }

            /// Nominal -> Nominal Value
            public string Nominal { get; set; }

            /// Min. Val -> Minimum Value
            public string MinValue { get; set; }

            /// Max. Val -> Maximum Value
            public string MaxValue { get; set; }

            /// Unit -> Measure Unit
            public string Unit { get; set; }
        }

        public enum SerialState
        {
            /// OK
            PASS = 0,

            /// Fail
            FAIL = 1,

            /// Scrap
            SCRAP = 2,

            ///New
            NEW = 3,

            /// Unknown
            UNKNOWN = -1,
        }

        public enum AttributeTypes
        {
            SerialNumber = 0,
            WorkOrder = 1,
            Station = 7,
            PartNumber = 10,
            Bom = 13,
            Equipment = 15
        }

        #endregion
    }
}
