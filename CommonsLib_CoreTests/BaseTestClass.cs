using System;
using System.IO;
using System.Threading.Tasks;
using CommonsLib_APP.Config;
using CommonsLib_BLL.Config;
using CommonsLib_BLL.Config.Initializers;
using CommonsLib_DAL.Config;
using CommonsLib_DAL.Initializers;
using CommonsLib_DATA.Config;
using CommonsLib_DATA.Config.Initializers;
using CommonsLib_IOC.Config;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SQLite;

namespace CommonsLib_CoreTests
{
    [TestFixture]
    public class BaseTestClass
    {
        protected const string DbFileName = "unitTests.db3";

        [SetUp]
        public void Setup()
        {
            Teardown(); // Make sure data tear down has run

            AppBootstrap.Self.BootstrapInitializers.Add(PreConfigInitializer.Self);
            AppBootstrap.Self.BootstrapInitializers.Add(PostConfigInitializer.Self);
            
            static void StatusLogger(string status)
            { Console.WriteLine($"BL Status: {status}"); }

            AppBootstrap.Self.OnInitializerStatusChanged -= StatusLogger;
            AppBootstrap.Self.OnInitializerStatusChanged += StatusLogger;

            // Initialize App
            var initializerTask = AppBootstrap.Self.Initialize();
            
            initializerTask.Wait();
        }

        [TearDown]
        public void Teardown()
        {
            LocalDbManager.DbConnection?.CloseAsync().Wait();
            IoCManager.Resolver?.Dispose();

            var expectedDbFilePath = BasePathManager.GetFile(DbFileName);
            if (File.Exists(expectedDbFilePath))
                File.Delete(expectedDbFilePath);
        }

        [Test, Order(-1)]
        public void ContextLoads()
        {
            Assert.IsNotNull(LoggerManager.MainLogger);
            Assert.IsNotNull(IoCManager.Resolver);

            var rootConfig = IoCManager.Resolver.ResolveInstance<IConfigurationRoot>();
            Assert.IsNotNull(rootConfig);

            var dbConn = IoCManager.Resolver.ResolveInstance<SQLiteAsyncConnection>();
            Assert.IsNotNull(dbConn);

            var expectedDbFilePath = BasePathManager.GetFile(DbFileName);
            Assert.IsTrue(File.Exists(expectedDbFilePath));
        }
    }

    internal class PreConfigInitializer : IAppInitializer
    {
        private PreConfigInitializer() {}
        public static readonly PreConfigInitializer Self = new PreConfigInitializer();
        
        /// <inheritdoc/>
        public int Order => -6;

        /// <inheritdoc/>
        public string InitializerName => "Unit Tests Pre-Config Initializer.";
        
        /// <summary>
        /// Add unit test initializer stuff.
        /// </summary>
        public Task DoInitialize()
        {
            return Task.Run(() =>
            {
                GlobalConfigManager.ExternalAppSettingsEnabled = true;
                GlobalConfigManager.PostAppSettingsEnabled = true;

                GlobalConfigurationBootstrapInitializer.Self.SettingsNamespaces.UnionWith(new []
                {
                    nameof(CommonsLib_CoreTests)
                });
                SqLiteBootstrapInitializer.Self.MigrationsNamespaces.UnionWith(new []
                {
                    nameof(CommonsLib_CoreTests)
                });
            });
        }
    }
    
    internal class PostConfigInitializer : IAppInitializer
    {
        private PostConfigInitializer() {}
        public static readonly PostConfigInitializer Self = new PostConfigInitializer();
        
        /// <inheritdoc/>
        public int Order => 2;

        /// <inheritdoc/>
        public string InitializerName => "Unit Tests Post-Config Initializer.";
        
        /// <summary>
        /// Add unit test initializer stuff.
        /// </summary>
        public Task DoInitialize()
        {
            return Task.Run(() =>
            {
                var rootConfig = GlobalConfigManager.ConfigRoot;
                LocalDbManager.EraseSchemaIfValidationFails =
                    bool.Parse(rootConfig["main-config:Bootstrap:EraseSchemaIfValidationFails"]);
                LocalDbManager.DatabaseFile = BasePathManager
                    .PrefixBasePathIfRequired(rootConfig["main-config:Bootstrap:DatabaseFile"]);

                LoggerManager.LogFileName = BasePathManager
                    .PrefixBasePathIfRequired(rootConfig["main-config:Bootstrap:LogFile"]);
            });
        }
    }
}