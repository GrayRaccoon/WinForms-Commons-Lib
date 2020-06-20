using Serilog;

namespace CommonsLib_DAL.Config
{
    /// <summary>
    /// Static class that will contain the Main Logger Instance.
    /// </summary>
    public static class LoggerManager
    {

        /// <summary>
        /// Log file Name.
        /// </summary>
        public static string LogFileName { get; set; } = "logs/application.log";

        /// <summary>
        /// Logger Output Template.
        /// </summary>
        public static string LogOutputTemplate { get; set; } = 
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

        /// <summary>
        /// Application logger instance.
        /// We will work with a single logger so that all output will end in the same file.
        /// </summary>
        public static ILogger MainLogger { get; set; } = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.File(LogFileName,
                rollingInterval: RollingInterval.Day,
                outputTemplate: LogOutputTemplate,
                shared: true)
            .CreateLogger();
        
    }
}
