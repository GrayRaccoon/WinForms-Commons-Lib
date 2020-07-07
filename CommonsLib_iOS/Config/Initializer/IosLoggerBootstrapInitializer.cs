using System.Threading.Tasks;
using CommonsLib_DAL.Config;
using CommonsLib_DAL.Initializers;
using Serilog;

namespace CommonsLib_iOS.Config.Initializer
{
    /// <summary>
    /// iOS Logger Bootstrap Initializer
    /// </summary>
    public class IosLoggerBootstrapInitializer : IAppInitializer
    {
        public static readonly IosLoggerBootstrapInitializer Self = new IosLoggerBootstrapInitializer();

        private IosLoggerBootstrapInitializer()
        {
        }

        /// <inheritdoc/>
        public string InitializerName => "iOS Logger Initializer";

        /// <inheritdoc/>
        public int Order => 5;

        /// <summary>
        ///     Initialize Main Logger.
        /// </summary>
        public Task DoInitialize()
        {
            return Task.Run(() =>
            {
                LoggerManager.MainLogger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.NSLog(outputTemplate: LoggerManager.LogOutputTemplate)
                    .WriteTo.File(LoggerManager.LogFileName,
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: LoggerManager.LogOutputTemplate,
                        shared: true)
                    .CreateLogger();
            });
        }
    }
}