using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPTE_Base_Project.Common.Utils.MediatorPattern;
using IPTE_Base_Project.Common;
using IPTE_Base_Project.Models;
using com.itac.mes.imsapi.client.dotnet;
using com.itac.mes.imsapi.domain.container;
using IPTE_Base_Project.Common.Utils.Plc;
using IPTE_Base_Project.DataSources.DeviceConfig;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using IPTE_Base_Project.Models.Devices;
using Ipte.TS1.UI.Controls;
using IPTE_Base_Project.Variants;
using System.Xml.Linq;


namespace IPTE_Base_Project.ViewModels.Common
{
    class ITAC_ControlViewModel
    {
        #region Members
        private ITACModel ItacModel;

        public ITACModel.Station CurrentStation { get; set; }
        // Generates the instance of IMSApiDotNet
        private IMSApiDotNet _IMSApiDotNet = IMSApiDotNet.loadLibrary();
        //Session context struct for regLogInFunction
        private IMSApiSessionContextStruct SessionContex = null;
        string StationCode;
        int LogInResult;
        bool ITAC_Enable;


        private Dictionary<UInt32, object[]> Querys = new Dictionary<UInt32, object[]>();
        private Dictionary<string, Dictionary<string, string>> Query_List;
        UInt32 ID;

        #endregion
        #region Constructor
        public ITAC_ControlViewModel(DataReadModel DataModel , Itac_Settings Settings,ITACModel itacmodel)
        {

            ID = 0;
            ItacModel = itacmodel;
            Query_List = DataModel.Query_List;
            Mediator.Register("Itac_CAll", Itac_Stack);
            Mediator.Register("Itac_CAll_Interlock", Itac_Stack);
            Mediator.Register("Itac_CAll_Booking", Itac_Stack);
            Mediator.Register("WindowClose", CloseConnection);
            Mediator.Register("ResetReference", Reset_Itac);
            
            //Itac Parametrization
            IMSApiDotNet.setProperty("itac.artes.clusternodes",Settings.Cluster_Node);
            IMSApiDotNet.setProperty("itac.appid",Settings.AppID);
            IMSApiDotNet.setProperty("itac.propdir",Settings.PropDir);
            ITAC_Enable = Settings.Enable;
            StationCode = Settings.ItacStCode;

        }
        #endregion

        #region Metods

        public void Init()
        {
            if (ITAC_Enable)
                Connect_Itac();
        }

       
        private void Connect_Itac ()
        {

            Logging.log.Info("Itac Connection Start HS ");
            _IMSApiDotNet.imsapiInit();
            LogInResult = _IMSApiDotNet.regLogin(new IMSApiSessionValidationStruct()
            {
                stationNumber = StationCode,
                client = "01",
                registrationType = "S",
                systemIdentifier = StationCode,
            }, out SessionContex);

            if (LogInResult.Equals(IMSApiDotNetConstants.RES_OK))
            {
                ItacModel.Status = true;
                GetStationSettings(StationCode);
                Logging.log.Debug("Connecting to ITAC: " + LogInResult + " =>" + GetErrorText(LogInResult));
                Mediator.NotifyColleagues("ItacStatus", new object[] { ItacModel.Status });
               
            }
            else
            {
                ItacModel.Status = false;
                Logging.log.Debug("Error Connecting to ITAC: "+ LogInResult + " =>" + GetErrorText(LogInResult));
                Mediator.NotifyColleagues("ReferenceError", new object[] { "ITAC", "Connecting to ITAC:Fail", "Connection to Itac not Received Error: " + LogInResult + " =>" + GetErrorText(LogInResult) });
                Mediator.NotifyColleagues("ItacStatus", new object[] { ItacModel.Status });
            }
        }

        private void Reset_Itac(object[] param)
        {
            Logging.log.Info("Itac Connection Start HS ");
            _IMSApiDotNet.imsapiInit();
            LogInResult = _IMSApiDotNet.regLogin(new IMSApiSessionValidationStruct()
            {
                stationNumber = StationCode,
                client = "01",
                registrationType = "S",
                systemIdentifier = StationCode,
            }, out SessionContex);

            if (LogInResult.Equals(IMSApiDotNetConstants.RES_OK))
            {
                ItacModel.Status = true;
                GetStationSettings(StationCode);
                Logging.log.Debug("Connecting to ITAC: " + LogInResult + " =>" + GetErrorText(LogInResult));
                Mediator.NotifyColleagues("ItacStatus", new object[] { ItacModel.Status });

            }
            else
            {
                ItacModel.Status = false;
                Logging.log.Debug("Error Connecting to ITAC: " + LogInResult + " =>" + GetErrorText(LogInResult));
                Mediator.NotifyColleagues("ReferenceError", new object[] { "ITAC", "Connecting to ITAC:Fail", "Connection to Itac not Received Error: " + LogInResult + " =>" + GetErrorText(LogInResult) });
                Mediator.NotifyColleagues("ItacStatus", new object[] { ItacModel.Status });
            }
        }
        public void CloseConnection(object[] param)
        {
            _IMSApiDotNet.regLogout(SessionContex);
            ItacModel.Status = false;
            Logging.log.Debug("ITAC Disconnect");

        }

        private void Itac_Stack(object[] param)
        {
            
            ID++;

        }

        #region Functions
        public ITACModel.Station GetStationSettings(string stationName)
        {
            string[] Values = null;
            var CurrentStation = new ITACModel.Station();

            int resultValue = _IMSApiDotNet.trGetStationSetting(SessionContex, stationName,
                new string[]
                        {
                            "PART_NUMBER",
                            "PROCESS_VERSION",
                            "PROCESS_LAYER",
                            "WORKORDER_NUMBER",
                            "BOM_VERSION"
                        }, out Values);

            //Send the status message
            Logging.log.Debug(GetErrorText(resultValue));
           // StatusReturned = RaiseOutputMessage(resultValue, "[SessionContextHandler] -> trGetStationSetting");

            //If result was OK, copy the values in the array (previously defined on the function call) to the current values
            if (resultValue.Equals(IMSApiDotNetConstants.RES_OK))
            {
                CurrentStation = new ITACModel.Station()
                {
                    Name = stationName,
                    PartNumber = Values[0],
                    ProcessVersion = Values[1],
                    Layer = string.IsNullOrEmpty(Values[2].ToString()) ? 0 : Convert.ToInt32(Values[2]),
                    WorkOrder = Values[3],
                    Bom = Convert.ToInt32(Values[4])
                };

                this.CurrentStation = new ITACModel.Station()
                {
                    Name = stationName,
                    PartNumber = Values[0],
                    ProcessVersion = Values[1],
                    Layer = string.IsNullOrEmpty(Values[2].ToString()) ? 0 : Convert.ToInt32(Values[2]),
                    WorkOrder = Values[3],
                    Bom = Convert.ToInt32(Values[4])
                };
            }
            //Otherwise copy blank error values
            else
            {
                CurrentStation = new ITACModel.Station()
                {
                    Name = "-1",
                    PartNumber = "-1",
                    ProcessVersion = "-1",
                    Layer = -1,
                    WorkOrder = "-1",
                    Bom = -1
                };
            }

            return CurrentStation;
        }

        public ITACModel.SerialState Interlock(string serialNumber, string Station, ref ITACModel.ErrorMessage StatusReturned, out int SerialNumberState, out List<ITACModel.Spec> RecipeData)
        {
            //Declare local usage variables 

            ITACModel.Station CurrentStation = new ITACModel.Station();
            ITACModel.Serial CurrentSerialNumber = new ITACModel.Serial();
            // List<iTacObjects.Spec> RecipeData = new List<iTacObjects.Spec>();

            //Following the API flow

            //Get Serial Number Info
            CurrentSerialNumber = GetSerialNumber(serialNumber, Station, ref StatusReturned);

            if (!StatusReturned.Success)
            {
                //StatusReturned = RaiseOutputMessage(StatusReturned.Code, "[Interlocking]-> trGetSerialNumberInfo");
                SerialNumberState = -1;

                RecipeData = null;
                return ITACModel.SerialState.UNKNOWN;
            }


            //Get Station Settings and save them to the local variable

            CurrentStation = GetStationSettings(Station, ref StatusReturned);

            //If all Object properties are equals to -1 it means that the station is wrong, if just some objects are -1 something is worng in the code
            if (CurrentStation.PartNumber == "-1" || CurrentStation.Name == "-1" || CurrentStation.Layer == -1 || CurrentStation.Bom == -1 || CurrentStation.ProcessVersion == "-1" || CurrentStation.WorkOrder == "-1")
            {
                // StatusReturned = RaiseOutputMessage(StatusReturned.Code, "[Interlocking]-> trGetStationSettings");
                SerialNumberState = -1;
                RecipeData = null;
                return ITACModel.SerialState.UNKNOWN;
            }


            //Check serial number state. If 0, then Interlock is OK and the PCB can be entered 
            var srnState = CheckSerialNumberstate(CurrentSerialNumber, CurrentStation, ref StatusReturned);

            if (StatusReturned.Success == true && srnState == ITACModel.SerialState.PASS)
            {

                //Get recipe data
                //Send GetRecipeData fucntion to verify that the serial number has a recipe assigned
                RecipeData = GetRecipeData(CurrentStation, ref StatusReturned);


                //Check if the recipe got information
                if (StatusReturned.Success == false)
                {
                    SerialNumberState = -1;
                    return ITACModel.SerialState.UNKNOWN;
                }
                else if (StatusReturned.Success == true && (RecipeData == null || RecipeData.Count == 0))
                {
                    Logging.log.Debug("Error, no se pudo obtener la receta de iTAC, receta obtenida vacia");
                    // RaiseOutputMessage(100000, "[GetRecipeData] -> mdaGetRecipeData", "Error, no se pudo obtener la receta de iTAC, receta obtenida vacia");
                    SerialNumberState = -1;
                    return ITACModel.SerialState.UNKNOWN;
                }

                else if (StatusReturned.Success == true && RecipeData.Count != 0)
                {
                    SerialNumberState = 0;
                    return ITACModel.SerialState.PASS;
                }

                //If the code could reach this lines, send status unknown
                SerialNumberState = -1;
                return ITACModel.SerialState.UNKNOWN;
            }
            else
            {
                Logging.log.Debug("[AppendAttributes] -> attribAppendAttributeValues" + GetErrorText(StatusReturned.Code));
                //StatusReturned = RaiseOutputMessage(StatusReturned.Code, "[Interlocking]-> trCheckSerialNumberState");
                SerialNumberState = -1;
                RecipeData = null;
                return ITACModel.SerialState.UNKNOWN;
            }
        }

        public List<ITACModel.Spec> GetRecipeDataFromItac(string serialNumber, string Station, ref ITACModel.ErrorMessage StatusReturned)
        {

            ITACModel.Station CurrentStation = new ITACModel.Station();
            ITACModel.Serial CurrentSerialNumber = new ITACModel.Serial();
            List<ITACModel.Spec> RecipeData = new List<ITACModel.Spec>();

            //Following the API flow

            //Get Serial Number Info
            CurrentSerialNumber = GetSerialNumber(serialNumber, Station, ref StatusReturned);

            if (!StatusReturned.Success)
            {
                //error on serial number
                return null;
            }


            //Get Station Settings and save them to the local variable
            CurrentStation = GetStationSettings(Station, ref StatusReturned);

            //If all Object properties are equals to -1 it means that the station is wrong, if just some objects are -1 something is worng in the code
            if (CurrentStation.PartNumber == "-1" || CurrentStation.Name == "-1" || CurrentStation.Layer == -1 || CurrentStation.Bom == -1 || CurrentStation.ProcessVersion == "-1" || CurrentStation.WorkOrder == "-1")
            {

                //error on station
                return null;
            }


            //Get recipe data
            //Send GetRecipeData fucntion to verify that the serial number has a recipe assigned
            RecipeData = GetRecipeData(CurrentStation, ref StatusReturned);
            //Check if the recipe got information
            if (StatusReturned.Success == false)
            {
                return null;
            }
            else if (StatusReturned.Success == true && (RecipeData == null || RecipeData.Count == 0))
            {
                Logging.log.Debug("Error, no se pudo obtener la receta de iTAC, receta obtenida vacia" + GetErrorText(100000));
                //RaiseOutputMessage(100000, "[GetRecipeData] -> mdaGetRecipeData", "Error, no se pudo obtener la receta de iTAC, receta obtenida vacia");
                return null;
            }

            else if (StatusReturned.Success == true && RecipeData.Count != 0)
            {
                return RecipeData;
            }


            else
                return null;
        }

        //Booking for upload all data to iTac
        public bool BookData(string serialNumber, string Station, ITACModel.SerialState SerialState, float cycleTime, List<ITACModel.Measure> measureList, int IsPanel, ref ITACModel.ErrorMessage StatusReturned)
        {

            //Declare local usage variables 

            ITACModel.Station CurrentStation = new ITACModel.Station();
            ITACModel.Serial CurrentSerialNumber = new ITACModel.Serial();

            //Following the API flow, check: SerialnumberInfo, StationSettings, HistoryData and SerialNumberState in order to allow ther PCB to work 
            //Get Serial Number Info
            CurrentSerialNumber = GetSerialNumber(serialNumber, Station, ref StatusReturned);

            if (!StatusReturned.Success)
            {
                Logging.log.Debug("[BookData]-> trGetSerialNumberInfo" + GetErrorText(StatusReturned.Code));
                //StatusReturned = RaiseOutputMessage(StatusReturned.Code, "[BookData]-> trGetSerialNumberInfo");
                return false;
            }

            //Get Station Settings and save them to the local variable
            CurrentStation = GetStationSettings(Station, ref StatusReturned);

            //If all Object properties are equals to -1 it means that the station is wrong, if just some objects are -1 something is worng in the code
            if (CurrentStation.PartNumber == "-1" || CurrentStation.Name == "-1" || CurrentStation.Layer == -1 || CurrentStation.Bom == -1 || CurrentStation.ProcessVersion == "-1" || CurrentStation.WorkOrder == "-1")
            {
                Logging.log.Debug("[BookData]-> trGetStationSettings" + GetErrorText(StatusReturned.Code));
                //StatusReturned = RaiseOutputMessage(StatusReturned.Code, "[BookData]-> trGetStationSettings");
                return false;
            }


            bool UploadResult = UploadResultData(CurrentSerialNumber, CurrentStation, SerialState, cycleTime, measureList, IsPanel, ref StatusReturned);

            if (UploadResult == true)
                return true;
            else
            {
                Logging.log.Debug("[BookData]-> trUploadResultData" + GetErrorText(StatusReturned.Code));
                //StatusReturned = RaiseOutputMessage(StatusReturned.Code, "[BookData]-> trUploadResultData");
                return false;
            }
        }

        public bool AppendAndBookData(string serialNumber, string Station, ITACModel.SerialState SerialState, float cycleTime, string Attribute, List<ITACModel.Measure> measureList, int IsPanel, ref ITACModel.ErrorMessage StatusReturned)
        {
            //Declare local usage variables 

            ITACModel.Station CurrentStation = new ITACModel.Station();
            ITACModel.Serial CurrentSerialNumber = new ITACModel.Serial();

            //Following the API flow, check: SerialnumberInfo, StationSettings, HistoryData and SerialNumberState in order to allow ther PCB to work 
            //Get Serial Number Info
            CurrentSerialNumber = GetSerialNumber(serialNumber, Station, ref StatusReturned);

            if (!StatusReturned.Success)
            {
                Logging.log.Debug("[BookData]-> trGetSerialNumberInfo" + GetErrorText(StatusReturned.Code));
                //StatusReturned = RaiseOutputMessage(StatusReturned.Code, "[BookData]-> trGetSerialNumberInfo");
                return false;
            }

            //Get Station Settings and save them to the local variable
            CurrentStation = GetStationSettings(Station, ref StatusReturned);

            //If all Object properties are equals to -1 it means that the station is wrong, if just some objects are -1 something is worng in the code
            if (CurrentStation.PartNumber == "-1" || CurrentStation.Name == "-1" || CurrentStation.Layer == -1 || CurrentStation.Bom == -1 || CurrentStation.ProcessVersion == "-1" || CurrentStation.WorkOrder == "-1")
            {
                Logging.log.Debug("[Interlocking]-> trGetStationSettings" + GetErrorText(StatusReturned.Code));
                //StatusReturned = RaiseOutputMessage(StatusReturned.Code, "[Interlocking]-> trGetStationSettings");
                return false;
            }


            //Apend attributes to iTac
            bool AppendResult = AppendAttributes(CurrentSerialNumber, CurrentStation, Attribute, ref StatusReturned);
            if (StatusReturned.Success == true && AppendResult == true)
            {
                //Upload data to iTac
                bool UploadResult = UploadResultData(CurrentSerialNumber, CurrentStation, SerialState, cycleTime, measureList, IsPanel, ref StatusReturned);

                if (StatusReturned.Success == true && UploadResult == true)
                {
                    return true;
                }
                else
                {
                    //StatusReturned = RaiseOutputMessage(StatusReturned.Code, "[AppendAndBookData]-> UploadResultData");
                    return false;
                }
            }
            else
            {
                //StatusReturned = RaiseOutputMessage(StatusReturned.Code, "[AppendAndBookData]-> AppendAttributes");
                return false;
            }


        }
        #endregion
        #region IMSApi Functions

        public ITACModel.Station GetStationSettings(string stationName, ref ITACModel.ErrorMessage StatusReturned)
        {
            string[] Values = null;
            var CurrentStation = new ITACModel.Station();

            int resultValue = _IMSApiDotNet.trGetStationSetting(SessionContex, stationName,
                new string[]
                        {
                            "PART_NUMBER",
                            "PROCESS_VERSION",
                            "PROCESS_LAYER",
                            "WORKORDER_NUMBER",
                            "BOM_VERSION"
                        }, out Values);

            //Send the status message
            Logging.log.Debug("[SessionContextHandler] -> trGetStationSetting" + GetErrorText(resultValue));
            //StatusReturned = RaiseOutputMessage(resultValue, "[SessionContextHandler] -> trGetStationSetting");

            //If result was OK, copy the values in the array (previously defined on the function call) to the current values
            if (resultValue.Equals(IMSApiDotNetConstants.RES_OK))
            {
                CurrentStation = new ITACModel.Station()
                {
                    Name = stationName,
                    PartNumber = Values[0],
                    ProcessVersion = Values[1],
                    Layer = string.IsNullOrEmpty(Values[2].ToString()) ? 0 : Convert.ToInt32(Values[2]),
                    WorkOrder = Values[3],
                    Bom = Convert.ToInt32(Values[4])
                };

                this.CurrentStation = new ITACModel.Station()
                {
                    Name = stationName,
                    PartNumber = Values[0],
                    ProcessVersion = Values[1],
                    Layer = string.IsNullOrEmpty(Values[2].ToString()) ? 0 : Convert.ToInt32(Values[2]),
                    WorkOrder = Values[3],
                    Bom = Convert.ToInt32(Values[4])
                };
            }
            //Otherwise copy blank error values
            else
            {
                CurrentStation = new ITACModel.Station()
                {
                    Name = "-1",
                    PartNumber = "-1",
                    ProcessVersion = "-1",
                    Layer = -1,
                    WorkOrder = "-1",
                    Bom = -1
                };
            }

            return CurrentStation;

        }

        private ITACModel.Serial GetSerialNumber(string serialNumber, string Station, ref ITACModel.ErrorMessage StatusReturned)
        {
            //Check station settings
            //Following the Generic Tester API Flow V0.1 Released on 8/22/2018 is just needed to get Serial number and Serial number pos form iTac

            var CurrentSerialNumber = new ITACModel.Serial();

            int GetSerialNoResult = 0;
            string[] values;

            GetSerialNoResult = _IMSApiDotNet.trGetSerialNumberInfo(SessionContex, Station, serialNumber, "-1",
                new string[] { "SERIAL_NUMBER", "SERIAL_NUMBER_POS" }, out values);

            //Send the status message

            Logging.log.Debug("[GetSerialNumberInfo] -> trGetSerialNumberInfo" + GetErrorText(GetSerialNoResult));
            //StatusReturned = RaiseOutputMessage(GetSerialNoResult, "[GetSerialNumberInfo] -> trGetSerialNumberInfo");

            //If result was OK, copy the values in the array (previously defined on the function call) to the current values
            if (GetSerialNoResult.Equals(IMSApiDotNetConstants.RES_OK))
            {
                //var SerialNumber = new iTacObjects.Serial()
                CurrentSerialNumber = new ITACModel.Serial()
                {
                    ID = serialNumber,
                    PanelSN = values[0],
                    Pos = Convert.ToInt32(values[1]),
                };
                return CurrentSerialNumber;
                // return SerialNumber;
            }

            //Otherwise copy blank error values
            else
            {
                //var SerialNumber = new iTacObjects.Serial()
                CurrentSerialNumber = new ITACModel.Serial()
                {
                    ID = "-1",
                    PanelSN = "-1",
                    Pos = -1,
                };
                return CurrentSerialNumber;
                // return SerialNumber;
            }


        }

        private ITACModel.SerialState CheckSerialNumberstate(ITACModel.Serial serialNumber, ITACModel.Station Station, ref ITACModel.ErrorMessage StatusReturned)
        {
            string[] ValidationResult;

            // Execute serial number check
            int resultValue = _IMSApiDotNet.trCheckSerialNumberState(SessionContex, Station.Name, Station.Layer,
                0, serialNumber.ID, serialNumber.Pos.ToString(), new String[] { "SERIAL_NUMBER_STATE", "ERROR_CODE" }, out ValidationResult);

            if (ValidationResult.Length > 0)
            {
                Logging.log.Debug("[trCheckSerialNumberState] -> trCheckSerialNumberState" + Convert.ToInt32(ValidationResult[1]));
                //StatusReturned = RaiseOutputMessage(Convert.ToInt32(ValidationResult[1]), "[trCheckSerialNumberState] -> trCheckSerialNumberState");
                return (ITACModel.SerialState)Convert.ToInt32(ValidationResult[0]);

            }
            else
            {
                Logging.log.Debug("[trCheckSerialNumberState] -> trCheckSerialNumberState" + GetErrorText(-110));
                // StatusReturned = RaiseOutputMessage(-110, "[trCheckSerialNumberState] -> trCheckSerialNumberState");
                return ITACModel.SerialState.UNKNOWN;
            }
        }

        private List<ITACModel.Spec> GetRecipeData(ITACModel.Station Station, ref ITACModel.ErrorMessage StatusReturned)
        {
            string[] res;

            int resultValue = _IMSApiDotNet.mdaGetRecipeData(SessionContex, Station.Name, -1, "-1", "-1", "-1", 0, "-1", "-1", 1,
                new KeyValue[]
                {
                    new KeyValue("PART_NUMBER", Station.PartNumber),
                    /*new KeyValue("STATION_NUMBER", Station.Name),
                    new KeyValue("BOM_VERSION", Station.Bom.ToString()),
                    new KeyValue("PROCESS_VERSION", Station.ProcessVersion)*/ },

                    new string[] { "MEASURE_NAME", "MIN_VALUE", "NOMINAL", "MAX_VALUE", "UNIT" },
                    out res);

            //StatusReturned = RaiseOutputMessage(resultValue, "[GetReceipeInfo] -> mdaGetRecipeData");

            Logging.log.Debug("[GetReceipeInfo] -> mdaGetRecipeData" + GetErrorText(resultValue));

            if (resultValue.Equals(IMSApiDotNet.RES_OK))
            {
                var RecipeObjects = new List<ITACModel.Spec>();
                for (int i = 0; i < res.GetLength(0); i++)
                {
                    RecipeObjects.Add(new ITACModel.Spec()
                    {
                        Name = res[i],
                        MinValue = res[i + 1],
                        Nominal = res[i + 2],
                        MaxValue = res[i + 3],
                        Unit = res[i + 4]
                    });
                    i = i + 4;
                }
                return RecipeObjects;
            }
            else
                return null;
        }

        private ITACModel.HistoryData GetSerialNumberHistory(ITACModel.Serial serialNumber, ITACModel.Station Station, ref ITACModel.ErrorMessage StatusReturned)
        {
            string[] a = null, BookResultKeys = null;
            string s = string.Empty;
            int i = 0; long l = 0;

            ITACModel.HistoryData CurrentSnHistoryData = new ITACModel.HistoryData();

            //As we already have session context, station name and serial number, im using the ones i have and jump directly to the GetHistoryData function, in case of failures, make it as the example
            //Call Get Serial Number History Fucntion
            int resultValue = _IMSApiDotNet.trGetSerialNumberHistoryData(
                SessionContex,
                Station.Name,
                serialNumber.ID,
                serialNumber.Pos.ToString(),
                2/*Station.Layer*/, //Process layer. Valuea as indicated on API Flow
                1, //Disolving Serial Number. Value as indicated on API Flow
                0, //Disolving Level. Value as indicated on API Flow
                new string[] { "BOOK_DATE", "BOOK_STATE", "SEQUENCE_NUMBER", "SERIAL_NUMBER", "SERIAL_NUMBER_POS", "STATION_NUMBER", "WORKORDER_NUMBER" }, out BookResultKeys, //Booking Result keys
                new string[0], out a, // failureDataResultKeys
                new string[0], out a, // failureSlipDataResultKeys
                new string[0], out a, // measureDataResultKeys
                out s, // workOrderNumber
                out s, // partNumber
                out s, //customerPartNumber
                out s, //partDesc
                out s, //quantity
                out l, //lastReportDate
                out s, //lotNumber
                out i); //isLocked

            Logging.log.Debug("[GetSerialNumberHistory] -> trGetSerialNumberHistoryData" + GetErrorText(resultValue));
            //StatusReturned = RaiseOutputMessage(resultValue, "[GetSerialNumberHistory] -> trGetSerialNumberHistoryData");

            if (resultValue.Equals(IMSApiDotNet.RES_OK) && BookResultKeys.Length == 0)
            {
                CurrentSnHistoryData.BookDate = "3";
                CurrentSnHistoryData.BookState = "3";
                CurrentSnHistoryData.SeqNumber = "3";
                CurrentSnHistoryData.SerialNumber = "3";
                CurrentSnHistoryData.SNPosition = "3";
                CurrentSnHistoryData.StationNumber = "3";
                CurrentSnHistoryData.WorkOrderNumber = "3";

                return CurrentSnHistoryData;
            }

            if (resultValue.Equals(IMSApiDotNet.RES_OK) && BookResultKeys.Length != 0)
            {
                //In order to get the las book state, index to the end of the array instead of the start of the array 
                CurrentSnHistoryData.BookDate = BookResultKeys[BookResultKeys.Length - 7];
                CurrentSnHistoryData.BookState = BookResultKeys[BookResultKeys.Length - 6];
                CurrentSnHistoryData.SeqNumber = BookResultKeys[BookResultKeys.Length - 5];
                CurrentSnHistoryData.SerialNumber = BookResultKeys[BookResultKeys.Length - 4];
                CurrentSnHistoryData.SNPosition = BookResultKeys[BookResultKeys.Length - 3];
                CurrentSnHistoryData.StationNumber = BookResultKeys[BookResultKeys.Length - 2];
                CurrentSnHistoryData.WorkOrderNumber = BookResultKeys[BookResultKeys.Length - 1];

                return CurrentSnHistoryData;
            }
            else
            {
                CurrentSnHistoryData.BookDate = "-1";
                CurrentSnHistoryData.BookState = "-1";
                CurrentSnHistoryData.SeqNumber = "-1";
                CurrentSnHistoryData.SerialNumber = "-1";
                CurrentSnHistoryData.SNPosition = "-1";
                CurrentSnHistoryData.StationNumber = "-1";
                CurrentSnHistoryData.WorkOrderNumber = "-1";

                return CurrentSnHistoryData;
            }

        }

        //Append attributes to the serial number
        private bool AppendAttributes(ITACModel.Serial SerialNumber, ITACModel.Station Station, string BoxBarCode, ref ITACModel.ErrorMessage StatusReturned)
        {
            string[] attributeUploadKeys = new[] { "ATTRIBUTE_CODE", "ATTRIBUTE_VALUE", "ERROR_CODE" };
            string[] res;
            List<ITACModel.Attributes> outList = new List<ITACModel.Attributes>();

            int resultValue = _IMSApiDotNet.attribAppendAttributeValues(SessionContex, Station.Name, 0, SerialNumber.ID, SerialNumber.Pos.ToString(), -1, 1,
                attributeUploadKeys, new[] { "BATTERY_SN", BoxBarCode, "0" }, out res);

            Logging.log.Debug("[AppendAttributes] -> attribAppendAttributeValues" + GetErrorText(resultValue));
            //StatusReturned = RaiseOutputMessage(resultValue, "[AppendAttributes] -> attribAppendAttributeValues");

            if (resultValue.Equals(IMSApiDotNetConstants.RES_OK))
                return true;
            else
                return false;
        }
       
        //Get current attributes from PCB
        private bool GetAtrributes(ITACModel.Station Station, string BatterySN, ref ITACModel.ErrorMessage StatusReturned)
        {
            string[] res;

            int resultValue = _IMSApiDotNet.attribGetObjectsForAttributeValues(SessionContex, Station.Name, 0, "BATTERY_SN", BatterySN, 1,
                new KeyValue[]
                {
                    new KeyValue("DATE_FROM", "-1")

                },
                new string[] { "SERIAL_NUMBER" },
                out res
                );
            Logging.log.Debug("[AppendAttributes] -> attribGetObjectsForAttributeValues" + GetErrorText(resultValue));
            //StatusReturned = RaiseOutputMessage(resultValue, "[AppendAttributes] -> attribGetObjectsForAttributeValues");

            //CHECK IF results array = 0, if so, battery can works, otherwise send error, "Battery allready assigned to another PCB"
            if (!resultValue.Equals(IMSApiDotNetConstants.RES_OK) || res.Length == 0)
                return true;
            else
                return false;
        }

        //Get next serial number to be printed from iTac
        private string NextSerialNumber(ITACModel.Station Station, ref ITACModel.ErrorMessage StatusReturned)
        {
            var res = new SerialNumberData[0];
            int resultValue = _IMSApiDotNet.trGetNextSerialNumber(SessionContex, Station.Name, Station.WorkOrder, "-1", 1, out res);

            Logging.log.Debug("[NextSerialNumber] -> trGetNextSerialNumber" + GetErrorText(resultValue));
          //  StatusReturned = RaiseOutputMessage(resultValue, "[NextSerialNumber] -> trGetNextSerialNumber");

            if (resultValue.Equals(IMSApiDotNet.RES_OK))
                return res[0].serialNumber;
            else
                return string.Empty;
        }

        //Change the serial number in Itac for the one just printed
        public bool Switch(string newSerialNumber, string oldserialNumber, string Station, ref ITACModel.ErrorMessage StatusReturned)
        {
            //Declare local usage variables 

            ITACModel.Station CurrentStation = new ITACModel.Station();
            ITACModel.Serial CurrentOldSerialNumber = new ITACModel.Serial();

            //Following the API flow, check: SerialnumberInfo, StationSettings, HistoryData and SerialNumberState in order to allow ther PCB to work 
            //Get Serial Number Info
            CurrentOldSerialNumber = GetSerialNumber(oldserialNumber, Station, ref StatusReturned);

            if (!StatusReturned.Success)
            {
                Logging.log.Debug("[BookData]-> trGetSerialNumberInfo" + GetErrorText(StatusReturned.Code));
                //StatusReturned = RaiseOutputMessage(StatusReturned.Code, "[BookData]-> trGetSerialNumberInfo");
                return false;
            }

            //Get Station Settings and save them to the local variable
            CurrentStation = GetStationSettings(Station, ref StatusReturned);

            //If all Object properties are equals to -1 it means that the station is wrong, if just some objects are -1 something is worng in the code
            if (CurrentStation.PartNumber == "-1" || CurrentStation.Name == "-1" || CurrentStation.Layer == -1 || CurrentStation.Bom == -1 || CurrentStation.ProcessVersion == "-1" || CurrentStation.WorkOrder == "-1")
            {
                Logging.log.Debug("[Interlocking]-> trGetStationSettings" + GetErrorText(StatusReturned.Code));
                //StatusReturned = RaiseOutputMessage(StatusReturned.Code, "[Interlocking]-> trGetStationSettings");
                return false;
            }

            var serialToSwitch = new SwitchSerialNumberData[] { new SwitchSerialNumberData(1, newSerialNumber, CurrentOldSerialNumber.Pos.ToString(), CurrentOldSerialNumber.ID, 0) };
            int resultValue = _IMSApiDotNet.trSwitchSerialNumber(SessionContex, CurrentStation.Name, CurrentOldSerialNumber.ID, CurrentOldSerialNumber.Pos.ToString(), ref serialToSwitch);
            Logging.log.Debug("[Switch] -> trSwitchSerialNumber" + GetErrorText(resultValue));
           // StatusReturned = RaiseOutputMessage(resultValue, "[Switch] -> trSwitchSerialNumber");
            return resultValue.Equals(IMSApiDotNet.RES_OK);
        }

        //Upload Failure And results Data
        private bool UploadResultData(ITACModel.Serial serialNumber, ITACModel.Station CurrentStation, ITACModel.SerialState SerialState, float cycleTime, List<ITACModel.Measure> measureList, int IsPanel, ref ITACModel.ErrorMessage StatusReturned)
        {

            //Convert list to array
            List<string> measures = new List<string>();
            measureList.ForEach(e =>
            {
                measures.Add(e.ErrorCode);
                measures.Add(e.FailCode);
                measures.Add(e.Name);
                measures.Add(e.Value);
            });
            var arrayMeasures = measures.ToArray();

            // var arrayMeasures = new string [0];

            //Send results to iTac
            //Aqui revisar si solo se envian los metricos tal cual Audi o se tiene que enviar todo lo referente a los failures como lo inidca el API Flow
            int resultValue = _IMSApiDotNet.trUploadFailureAndResultData(
                SessionContex,                      // sessionContext
                CurrentStation.Name,                // stationNumber
                CurrentStation.Layer,               // processLayer
                serialNumber.ID,                       // serialNumberRef
                serialNumber.Pos.ToString(), // serialNumberRefPos
                (int)SerialState,                   // serialNumberState
                IsPanel,                            // duplicateSerialNumber
                cycleTime,                          // cycleTime
                -1,                                 // bookDate
                new string[] { "ERROR_CODE", "MEASURE_FAIL_CODE", "MEASURE_NAME", "MEASURE_VALUE" }, arrayMeasures,  // measureKeys
                out arrayMeasures,                  // measureValues
                new string[0],                      // measureResultValues
                new string[0], out arrayMeasures,   // failureKeys, failureValues
                new string[0],                      // failureResultValues
                new string[0], out arrayMeasures);  // failureSlipKeys, failureSlipValues

            Logging.log.Debug("[UploadResultData] -> trUploadFailureAndResultData" + GetErrorText(resultValue));
            //StatusReturned = RaiseOutputMessage(resultValue, "[UploadResultData] -> trUploadFailureAndResultData");


            if (resultValue.Equals(IMSApiDotNetConstants.RES_OK))
            {
                return resultValue.Equals(IMSApiDotNet.RES_OK);
            }
            else
                return false;
        }

        private string GetErrorText(int Code)
        {
            string errorText = string.Empty;
            _IMSApiDotNet.imsapiGetErrorText(SessionContex, Code, out errorText);
            return errorText;
        }

        #endregion

        #endregion


    }


}
