using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace CommonsLib_BLL.Config
{
    /// <summary>
    /// Simple Configuration Manager Class.
    /// </summary>
    public static class GlobalConfigManager
    {
        /// <summary>
        /// Whether or not external base directory app settings file load is enabled. 
        /// </summary>
        public static bool ExternalAppSettingsEnabled { get; set; } = true;

        /// <summary>
        /// Whether or not external base directory post app settings file load is enabled.
        /// </summary>
        public static bool PostAppSettingsEnabled { get; set; } = true;

        /// <summary>
        /// Whether or not cloud config app settings is enabled.
        /// </summary>
        public static bool CloudConfigSettingsEnabled { get; set; } = true;

        /// <summary>
        /// Global configuration root instance.
        /// </summary>
        public static IConfigurationRoot ConfigRoot { get; set; } = new ConfigurationRoot(new List<IConfigurationProvider>());
    }
}