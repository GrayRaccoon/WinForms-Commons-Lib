using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonsLib_BLL.Config.Initializers;
using CommonsLib_DAL.Config;
using CommonsLib_DAL.Config.Initializer;
using CommonsLib_DAL.Initializers;
using CommonsLib_DATA.Config.Initializers;
using CommonsLib_IOC.Config;
using CommonsLib_IOC.Config.Initializers;

namespace CommonsLib_APP.Config
{
    /// <summary>
    /// Application Base Configuration bootstrap.
    /// </summary>
    public class AppBootstrap
    {
        private AppBootstrap() {}
        public static readonly AppBootstrap Self = new AppBootstrap();
        
        /// <summary>
        /// Component Services namespaces list.
        /// </summary>
        public readonly List<IAppInitializer> BootstrapInitializers = new List<IAppInitializer>
        {
            StandardBasePathBootstrapInitializer.Self,
            GlobalConfigurationBootstrapInitializer.Self,
            SimpleFileLoggerBootstrapInitializer.Self,
            SqLiteBootstrapInitializer.Self,
            IoCBootstrapInitializer.Self,
        };

        /// <summary>
        /// Sends an event every time an initializer is about to start.
        /// </summary>
        public event Action<string>? OnInitializerStatusChanged;

        /// <summary>
        /// Initializes all the app configuration.
        /// </summary>
        public async Task Initialize()
        {
            await InitializeBootstrap();
            var logger = LoggerManager.MainLogger.ForContext<AppBootstrap>();
            logger.Information("### Base App Configuration successfully loaded!");
            await InitializeApplication();
            logger.Information("### App Initializers completed!");
        }

        /// <summary>
        /// Initializes app configuration.
        /// </summary>
        private async Task InitializeBootstrap()
        {
            /*
             * GR Bootstrap Initializers Common Order Recommendations:
             *
             * ... - -1    Setup No-dependency Initializer Preparers.
             * 0           Global Configuration
             * 1           Main Specific Configuration
             * 2 - 10      Setup Config dependent Initializer Preparers
             * 11 - 20     Important Configuration
             * 21 - 100    Normal Priority Configuration
             * 101 - ...   Low Priority Initializers.
             */

            //Order Bootstrap Initializers.
            var bootstrapInitializers = BootstrapInitializers
                .Distinct()
                .OrderBy(init => init.Order)
                .ThenBy(init => init.InitializerName)
                .ToList();

            foreach (var bootstrapInitializer in bootstrapInitializers)
            {
                OnInitializerStatusChanged?.Invoke($"Running: {bootstrapInitializer.InitializerName}");
                await bootstrapInitializer.DoInitialize();
            }

            OnInitializerStatusChanged?.Invoke("Bootstrap Done!");
        }

        /// <summary>
        /// Initializes app components.
        /// </summary>
        private async Task InitializeApplication()
        {
            var initializers = IoCManager.Resolver
                .ResolveInstance<IEnumerable<IAppInitializer>>()
                .Distinct()
                .ToList()
                .OrderBy(init => init.Order)
                .ThenBy(init => init.InitializerName);

            foreach (var appInitializer in initializers)
            {
                var initializerName = appInitializer.InitializerName;
                OnInitializerStatusChanged?.Invoke($"Running: {initializerName}");
                await appInitializer.DoInitialize();
            }
            OnInitializerStatusChanged?.Invoke("All done!");
        }

    }
}