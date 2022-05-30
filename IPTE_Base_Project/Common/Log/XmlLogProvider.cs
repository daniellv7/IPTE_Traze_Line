using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IPTE_Base_Project.Common.Log
{
    public class XmlLogProvider : LogProviderBase
    {

        #region Data Members

        private string _filePath = "";

        #endregion

        #region Overrided Methods

        public override void SetParameters(System.Collections.Specialized.NameValueCollection config)
        {
            _filePath = config["fileLocation"];
        }

        public override void WriteLog(LogType logType, string message)
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);



            XmlDocument xmlDoc = new XmlDocument();

            if (File.Exists(_filePath))
                xmlDoc.Load(_filePath);
            else
            {
                XmlElement root = xmlDoc.CreateElement("LogData");
                xmlDoc.AppendChild(root);

            }

            XmlElement subRoot = xmlDoc.CreateElement("Log");

            AddXmlElement(xmlDoc, subRoot, "Date", DateTime.Now.ToString());
            AddXmlElement(xmlDoc, subRoot, "LogType", logType.ToString());
            AddXmlElement(xmlDoc, subRoot, "Message", message);

            xmlDoc.DocumentElement.AppendChild(subRoot);

            xmlDoc.Save(_filePath);
        }

        #endregion

        #region Private Methods

        private static void AddXmlElement(XmlDocument xmlDoc, XmlElement subRoot, string elementName, string elementValue)
        {
            XmlElement appendedElementUsername = xmlDoc.CreateElement(elementName);
            XmlText xmlTextUserName = xmlDoc.CreateTextNode(elementValue);
            appendedElementUsername.AppendChild(xmlTextUserName);

            subRoot.AppendChild(appendedElementUsername);

        }

        #endregion

    }
}
