using System.IO;
using Microsoft.Extensions.Configuration;

namespace CountriesLibrary.Managers
{
    public static class ConfigurationManager
    {
        public static IConfigurationRoot Config;
        static ConfigurationManager()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            Config = builder.Build();
        }
    }
}
