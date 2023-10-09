﻿using Microsoft.Extensions.Configuration;
using System.IO;

namespace MMA.WebApi.Helpers
{
    public class ConfigHelper
    {
        private static ConfigHelper _appSettings;

        public string appSettingValue { get; set; }

        public static string AppSetting(string section, string key)
        {
            _appSettings = GetCurrentSettings(section, key);
            return _appSettings.appSettingValue;
        }

        public ConfigHelper(IConfiguration config, string Key)
        {
            this.appSettingValue = config.GetValue<string>(Key);
        }

        public static ConfigHelper GetCurrentSettings(string section, string key)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            var settings = new ConfigHelper(configuration.GetSection(section), key);

            return settings;
        }
    }
}
