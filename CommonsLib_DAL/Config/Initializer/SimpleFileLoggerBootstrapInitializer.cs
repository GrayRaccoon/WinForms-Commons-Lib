using System.Threading.Tasks;
using CommonsLib_DAL.Initializers;
using Serilog;

namespace CommonsLib_DAL.Config.Initializer
{
    /// <summary>
    /// Initializer class for Logger
    /// </summary>
    public class SimpleFileLoggerBootstrapInitializer: IAppInitializer
    {
        private SimpleFileLoggerBootstrapInitializer() {}
        public static readonly SimpleFileLoggerBootstrapInitializer Self = new SimpleFileLoggerBootstrapInitializer();


        /// <inheritdoc/>
        public string InitializerName => "Simple File Logger Initializer";
        
        /// <inheritdoc/>
        public int Order => 5;

        /// <summary>
        /// Initialize Main Logger.
        /// </summary>
        public Task DoInitialize()
        {
            return Task.Run(() =>
            {
                LoggerManager.MainLogger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Console(outputTemplate: LoggerManager.LogOutputTemplate)
                    .WriteTo.File(LoggerManager.LogFileName,
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: LoggerManager.LogOutputTemplate,
                        shared: true)
                    .CreateLogger();
            });
        }

    }
}