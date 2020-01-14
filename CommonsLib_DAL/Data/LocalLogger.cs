using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Serilog;
using Serilog.Core;

namespace CommonsLib_DAL.Data
{
    /// <summary>
    /// Static class that will contain the Main Logger Instance.
    /// </summary>
    public static class LocalLogger
    {
        private static string _logFileName = "application.log";
        
        /// <summary>
        /// Log file Name, update if required it will reset existing MainLogger.
        /// </summary>
        public static string LogFileName
        {
            get => _logFileName;
            set
            {
                _logFileName = value;
                _mainLogger = null;
            }
        }


        /// <summary>
        /// Application logger instnace.
        /// We will work with a single logger so that all output will end in the same file.
        /// </summary>
        private static Logger _mainLogger = null;

        /// <summary>
        /// This property will retrieve the Application Logger Instance.
        /// </summary>
        public static Logger MainLogger =>
            _mainLogger ?? (_mainLogger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(LogFileName,
                    rollingInterval: RollingInterval.Day,
                    shared: true)
                .CreateLogger());
    }
}
