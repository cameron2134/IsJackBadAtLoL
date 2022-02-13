using Microsoft.Extensions.Configuration;
using System;

namespace MatchDataRequester
{
    public static class FunctionConfigHelper
    {
        public static bool IsInitialised = false;
        public static IConfigurationRoot AppConfigSettings;

        // Default name of the local settings file for App Functions.
        private const string LocalSettingsFile = "local.settings.json";

        /// <summary>
        /// Initialises/loads the config file and allows the settings to be loaded by the application. Uses
        /// the local JSON settings file if running locally, otherwise if running from Azure, then the
        /// Azure config settings are used.
        /// </summary>
        public static void InitialiseConfigBuilder()
        {
            if (IsInitialised) return;

            var localRoot = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot");
            var azureRoot = $"{Environment.GetEnvironmentVariable("HOME")}/site/wwwroot";

            // Determine which environment we should use to obtain the config settings.
            var actualRoot = localRoot ?? azureRoot;

            AppConfigSettings = new ConfigurationBuilder()
                .SetBasePath(actualRoot)
                .AddJsonFile(LocalSettingsFile, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            IsInitialised = true;
        }

        /// <summary>
        /// Gets a setting from the connection string section of the config file.
        /// </summary>
        /// <param name="connectionStringName">The name of the setting to retrieve.</param>
        /// <returns>The desired setting from the connection string section.</returns>
        public static string GetConnectionString(string connectionStringName)
        {
            return AppConfigSettings.GetConnectionString(connectionStringName);
        }

        /// <summary>
        /// Gets a setting from the default section of the config file.
        /// </summary>
        /// <param name="settingName">The name of the setting to retrieve.</param>
        /// <returns>The desired setting value.</returns>
        public static string GetSetting(string settingName)
        {
            return AppConfigSettings[settingName];
        }

        /// <summary>
        /// Gets an entire section of settings from the config file.
        /// </summary>
        /// <param name="sectionName">The name of the section to retrieve.</param>
        /// <returns>The desired setting section.</returns>
        public static IConfiguration GetSettingSection(string sectionName)
        {
            return AppConfigSettings.GetSection(sectionName);
        }
    }
}