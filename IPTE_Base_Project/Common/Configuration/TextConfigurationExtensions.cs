using System;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;

namespace IPTE_Base_Project.Common.Configuration
{
    public static class TextConfigurationExtensions
    {
        public static IConfigurationBuilder AddTextFile(this IConfigurationBuilder builder, string path)
        {
            return AddTextFile(builder, provider: null, path: path, optional: false, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddTextFile(this IConfigurationBuilder builder, string path, bool optional)
        {
            return AddTextFile(builder, provider: null, path: path, optional: optional, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddTextFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            return AddTextFile(builder, provider: null, path: path, optional: optional, reloadOnChange: reloadOnChange);
        }

        public static IConfigurationBuilder AddTextFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange)
        {
            if (provider == null && Path.IsPathRooted(path))
            {
                provider = new PhysicalFileProvider(Path.GetDirectoryName(path));
                path = Path.GetFileName(path);
            }
            var source = new TextConfigurationSource
            {
                FileProvider = provider,
                Path = path,
                Optional = optional,
                ReloadOnChange = reloadOnChange
            };
            builder.Add(source);
            return builder;
        }
    }
}
