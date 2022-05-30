using Microsoft.Extensions.Configuration;

namespace IPTE_Base_Project.Common.Configuration
{
    class TextConfigurationSource : FileConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            FileProvider = FileProvider ?? builder.GetFileProvider();
            return new TextConfigurationProvider(this);
        }
    }
}
