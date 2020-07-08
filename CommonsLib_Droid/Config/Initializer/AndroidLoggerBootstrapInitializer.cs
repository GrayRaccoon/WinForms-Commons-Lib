using System.Threading.Tasks;
using CommonsLib_DAL.Config;
using CommonsLib_DAL.Initializers;
using Serilog;

namespace CommonsLib_Droid.Config.Initializer
{
    /// <summary>
    /// Android Logger Bootstrap Initializer
    /// </summary>
    public class AndroidLoggerBootstrapInitializer : IAppInitializer
    {
        private AndroidLoggerBootstrapInitializer() { }
        public static readonly AndroidLoggerBootstrapInitializer Self = new AndroidLoggerBootstrapInitializer();

        /// <inheritdoc/>
        public string InitializerName => "Android Logger Initializer";

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
                    .WriteTo.AndroidLog(outputTemplate: LoggerManager.LogOutputTemplate)
                    .WriteTo.File(LoggerManager.LogFileName,
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: LoggerManager.LogOutputTemplate,
                        shared: true)
                    .CreateLogger();
            });
        }
    }
}