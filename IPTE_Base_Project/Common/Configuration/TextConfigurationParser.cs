using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTE_Base_Project.Common.Configuration
{
    class TextConfigurationParser
    {
        private IDictionary<string, string> _data = new Dictionary<string, string>();
        private char separator = '=';

        public IDictionary<string, string> Parse(Stream input)
        {
            _data.Clear();

            try
            {
                using (StreamReader sr = new StreamReader(input))
                {
                    //process file
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] split = line.Split(new char[] { separator });

                        if (split.Length < 2)
                        {
                            //discard
                            continue;
                        }
                        else if (split.Length == 2)
                        {
                            //add key-value pair
                            _data.Add(split[0].Trim(), split[1].Trim());
                        }
                    }
                }
            }
            catch 
            {
                
            }

            return _data;
        }
    }
}
