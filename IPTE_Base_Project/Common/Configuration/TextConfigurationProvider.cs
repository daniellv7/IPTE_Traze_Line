using Microsoft.Extensions.Configuration;
using System.IO;

namespace IPTE_Base_Project.Common.Configuration
{
    class TextConfigurationProvider : FileConfigurationProvider
    {
        public TextConfigurationProvider(TextConfigurationSource source) : base(source) { }

        public override void Load(Stream stream)
        {
            var parser = new TextConfigurationParser();
            Data = parser.Parse(stream);
        }
    }
}
