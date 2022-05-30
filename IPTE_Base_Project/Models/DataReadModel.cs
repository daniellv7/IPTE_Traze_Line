using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IPTE_Base_Project.Common;

namespace IPTE_Base_Project.Models
{
   public class DataReadModel:BaseModel
    {
        public Dictionary<string, Dictionary<string, string>> Query_List { get; set; } = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<int, Dictionary<int, string>> ErrorTitleCompilation { get; set; } = new Dictionary<int, Dictionary<int, string>>();
        public Dictionary<int, Dictionary<int, string>> ErrorMessageCompilation { get; set; } = new Dictionary<int, Dictionary<int, string>>();
        public Dictionary<int, Dictionary<int, string>> ErrorImageCompilation { get; set; } = new Dictionary<int, Dictionary<int, string>>();
        public Dictionary<int, Dictionary<int, string>> GeneralMachineErrorMessage { get; set; } = new Dictionary<int, Dictionary<int, string>>();
        public Dictionary<int, Dictionary<int, string>> ProcessErrorText { get; set; } = new Dictionary<int, Dictionary<int, string>>();
        public Dictionary<int, string> LocalMachineErrorMessage { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, List<string>> ProcessMessageList { get; set; } = new Dictionary<int, List<string>>();
        public Dictionary<int, string> BypassList { get; set; } = new Dictionary<int, string>();

        public MachineSettings Config { get; set; }
    #region Constructor

    public DataReadModel(MachineSettings Settings)
        {
            Config = Settings;
            ReadQueryList(Paths.QueryList);
            if (Settings.Station_Settings.Language == 0)
            {
                ReadItemErrorfile(Paths.ErrorList);
                ReadMachineErrorfile(Paths.MachineErrorList);
                ReadProcessMessages(Paths.ProcessMessage);
            }
            else
            {
                ReadItemErrorfile(Paths.ErrorList_0);
                ReadMachineErrorfile(Paths.MachineErrorList_0);
                ReadProcessMessages(Paths.ProcessMessage_0);
            }
            ReadProcessErrorfile(Paths.ProcessErrorFIle);
            ReadBypassList(Paths.BypassListPath);
            string cell = Settings.Station_Settings.Cell_Number.Remove(0, 4);
            try 
            {
                LocalMachineErrorMessage = ReadLocalMachineErrorlist(Int32.Parse(cell));
            }
            catch { }
            
        }
        #endregion
        #region Functions
        
        private void ReadQueryList(string Path)
        {
            XDocument XmlDoc = new XDocument();

            XmlDoc = XDocument.Load(Path);

            var QueryElements = XmlDoc.Root.Elements();
            foreach (var Query in QueryElements)
            {
                Dictionary<string, string> AttributesList = new Dictionary<string, string>();
                var QueryName = Query.Attribute("Name").Value;
                var Cell = Query.Element("Cell").Value;
                var QueryCommand = Query.Element("Command").Value;
                AttributesList.Add("Cell", Cell);
                AttributesList.Add("Command", QueryCommand);
                Query_List.Add(QueryName, AttributesList);
            }
            
        }
        private void ReadItemErrorfile(string Path)
        {
            Dictionary<string, string> ParameterList = new Dictionary<string, string>();
            XDocument XmlDoc = new XDocument();

            XmlDoc = XDocument.Load(Path);

            var CellElements=XmlDoc.Root.Elements();
            foreach (var Cell in CellElements)
            {
                var CellName=Int32.Parse(Cell.Attribute("Name").Value);
                Dictionary<int, string> MessageErrorList=new Dictionary<int, string>();
                Dictionary<int, string> ImageErrorList = new Dictionary<int, string>();
                Dictionary<int, string> TitleErrorList = new Dictionary<int, string>();
                var MessageList = Cell.Elements();
                foreach (var Message in MessageList)
                {
                    var Value =Int32.Parse( Message.Attribute("Value").Value);
                    var Title = Message.Attribute("Title").Value;
                    TitleErrorList.Add(Value, Title);

                    var Content = Message.Attribute("Content").Value;
                    MessageErrorList.Add(Value, Content);

                    if (Message.Attribute("Images") != null)
                    {
                        var ImageC = Message.Attribute("Images").Value;
                        ImageErrorList.Add(Value, ImageC);
                    }
                }
                if(ImageErrorList.Count!=0)
                    ErrorImageCompilation.Add(CellName, ImageErrorList);
                if (MessageErrorList.Count != 0)
                    ErrorMessageCompilation.Add(CellName, MessageErrorList);
                if (MessageErrorList.Count != 0)
                    ErrorTitleCompilation.Add(CellName, TitleErrorList);
            }
           
        }

        private void ReadMachineErrorfile(string Path)
        {
            Dictionary<string, string> ParameterList = new Dictionary<string, string>();
            XDocument XmlDoc = new XDocument();

            XmlDoc = XDocument.Load(Path);

            var CellElements = XmlDoc.Root.Elements();
            foreach (var Cell in CellElements)
            {
                var CellName = Int32.Parse(Cell.Attribute("Name").Value);
                Dictionary<int, string> MessageErrorList = new Dictionary<int, string>();
                var MessageList = Cell.Elements();
                foreach (var Message in MessageList)
                {
                    var Value = Int32.Parse(Message.Attribute("Value").Value);
                    var Content = Message.Attribute("Content").Value;
                    MessageErrorList.Add(Value, Content);
                }
                GeneralMachineErrorMessage.Add(CellName, MessageErrorList);
            }
        }

        private Dictionary<int, string> ReadLocalMachineErrorlist(int cell)
        {
            GeneralMachineErrorMessage.TryGetValue(cell, out var LocalList);
            return LocalList;
        }


        //pendiente
        private void ReadProcessMessages(string Path)
        {
            Dictionary<string, string> ParameterList = new Dictionary<string, string>();
            XDocument XmlDoc = new XDocument();

            XmlDoc = XDocument.Load(Path);

            var MessageElements = XmlDoc.Root.Elements();
            foreach (var Message in MessageElements)
            {
                List<string> MessageContext = new List<string>();
                var MessageNumber = Int32.Parse(Message.Attribute("Value").Value);
                var MessageText =Message.Attribute("Content").Value;
                var MessageLevel =Message.Attribute("Level").Value;
                MessageContext.Add(MessageText);
                MessageContext.Add(MessageLevel);
                ProcessMessageList.Add(MessageNumber, MessageContext);

            }
        }

        private void ReadProcessErrorfile(string Path)
        {
            Dictionary<string, string> ParameterList = new Dictionary<string, string>();
            XDocument XmlDoc = new XDocument();

            XmlDoc = XDocument.Load(Path);

            var CellElements = XmlDoc.Root.Elements();
            foreach (var Cell in CellElements)
            {
                var CellName = Int32.Parse(Cell.Attribute("Name").Value);
                Dictionary<int, string> MessageErrorList = new Dictionary<int, string>();
                var MessageList = Cell.Elements();
                foreach (var Message in MessageList)
                {
                    var Value = Int32.Parse(Message.Attribute("Value").Value);
                    var Content = Message.Attribute("Content").Value;
                    MessageErrorList.Add(Value, Content);
                }
                ProcessErrorText.Add(CellName, MessageErrorList);
            }
        }

        private void ReadBypassList(string Path)
        {
            Dictionary<string, string> ParameterList = new Dictionary<string, string>();
            XDocument XmlDoc = new XDocument();

            XmlDoc = XDocument.Load(Path);

            var CellElements = XmlDoc.Root.Elements();
            foreach (var Cell in CellElements)
            {
                var Value = Int32.Parse(Cell.Attribute("Value").Value);
                var Content = Cell.Attribute("Content").Value;
                BypassList.Add(Value, Content);
            }
        }
        #endregion
    }
}
 