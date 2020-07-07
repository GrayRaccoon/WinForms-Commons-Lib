using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CommonsLib_DAL.Config;
using CommonsLib_DAL.Initializers;
using Microsoft.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.ConfigServer;

namespace CommonsLib_BLL.Config.Initializers
{
    /// <summary>
    /// Initializer class for Global Application Configuration.
    /// </summary>
    public sealed class GlobalConfigurationBootstrapInitializer : IAppInitializer
    {
        private GlobalConfigurationBootstrapInitializer()
        { }

        public static readonly GlobalConfigurationBootstrapInitializer Self =
            new GlobalConfigurationBootstrapInitializer();

        /// <summary>
        /// Global Settings namespaces list.
        /// </summary>
        public readonly HashSet<string> SettingsNamespaces = new HashSet<string>();

        /// <inheritdoc/>
        public string InitializerName => "Global Configuration Initializer";

        /// <inheritdoc/>
        public int Order => 0;

        /// <summary>
        /// Initialize The Main configuration.
        /// </summary>
        public async Task DoInitialize()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(BasePathManager.BasePath);

            foreach (var settingsNamespace in SettingsNamespaces)
            {
                var settingsAssembly = Assembly.Load(settingsNamespace);
                var appSettingsStream =
                    settingsAssembly.GetManifestResourceStream($"{settingsNamespace}.appsettings.json");
                if (appSettingsStream == null) continue;
                var tmpSettingsFilePath = BasePathManager.GetFile($"tmp.{settingsNamespace}.appsettings.json");
                if (File.Exists(tmpSettingsFilePath))
                    File.Delete(tmpSettingsFilePath);
                using (var tmpSettingsFile = File.OpenWrite(tmpSettingsFilePath))
                    await appSettingsStream.CopyToAsync(tmpSettingsFile);
                configBuilder.AddJsonFile(tmpSettingsFilePath, optional: false, reloadOnChange: false);
            }

            if (GlobalConfigManager.ExternalAppSettingsEnabled)
            {
                configBuilder
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            }

            if (GlobalConfigManager.CloudConfigSettingsEnabled)
            {
                configBuilder
                    .AddConfigServer();
            }

            if (GlobalConfigManager.PostAppSettingsEnabled)
            {
                // appsettings-post has the most higher priority, over assembly, local and cloud configs.
                configBuilder
                    .AddJsonFile("appsettings-post.json", optional: true, reloadOnChange: true);
            }

            GlobalConfigManager.ConfigRoot = configBuilder.Build();
        }
    }
}