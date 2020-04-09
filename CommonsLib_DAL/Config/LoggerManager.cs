using Serilog;

namespace CommonsLib_DAL.Config
{
    /// <summary>
    /// Static class that will contain the Main Logger Instance.
    /// </summary>
    public static class LoggerManager
    {

        /// <summary>
        /// Log file Name, update if required it will reset existing MainLogger.
        /// </summary>
        public static string LogFileName { get; set; } = "log/application.log";

        /// <summary>
        /// Application logger instance.
        /// We will work with a single logger so that all output will end in the same file.
        /// </summary>
        public static ILogger MainLogger { get; set; } = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(LogFileName,
                rollingInterval: RollingInterval.Day,
                shared: true)
            .CreateLogger();
        
    }
}
