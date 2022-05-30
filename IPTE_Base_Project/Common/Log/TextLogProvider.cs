using System;
using System.IO;

namespace IPTE_Base_Project.Common.Log
{
    public class TextLogProvider : LogProviderBase
    {

        #region Data Members

        private string _configFilePath = "";
        private string _filePath = "";

        #endregion

        #region Overrided Methods

        public override void SetParameters(System.Collections.Specialized.NameValueCollection config)
        {
            _configFilePath = config[""];
            _filePath = config["fileLocation"];
        }

        public override void WriteLog(LogType logType, string message)
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            //    var app = File.Exists(_filePath);

            using (var sw = new StreamWriter(_filePath, true))
            {
                string s = string.Format("{0}, {1}, {2}", DateTime.Now, logType.ToString(), message);
                sw.WriteLine(s);
            }
        }

        #endregion

    }
}
