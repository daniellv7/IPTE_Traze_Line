using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTE_Base_Project.DataSources
{
    public static class XmlDs
    {
        public static IConfiguration ReadXmlFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception($"File not found: {filePath}");
            }
            string fullPath = string.Empty;
            try
            {
                fullPath = Path.GetFullPath(filePath);
                IConfiguration config = new ConfigurationBuilder().AddXmlFile(fullPath).Build();
                return config;
            }
            catch (Exception e)
            {
                throw new Exception($"There is a problem reading the file {fullPath}.\nError details: {e.ToString()}");
            }

        }
    }
}
