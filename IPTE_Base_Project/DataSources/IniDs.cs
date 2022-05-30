using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.CommonLib;
using System.IO;

namespace IPTE_Base_Project.DataSources
{
    public static class IniDs
    {
        public static IConfiguration ReadIniFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception($"File not found: {filePath}");
            }
            string fullPath = string.Empty;
            try
            {
                fullPath = Path.GetFullPath(filePath);
                IConfiguration config = new ConfigurationBuilder().AddIniFile(fullPath).Build();
                return config;
            }
            catch (Exception e)
            {
                throw new Exception($"There is a problem reading the file {fullPath}.\nError details: {e.ToString()}");
            }

        }
    }
}
