using System;
using System.Windows.Forms;
using CommonsLib_DAL.Errors;
using Serilog;

namespace CommonsLib_FLL.Utils
{
    /// <summary>
    /// UI Utilities for errors.
    /// </summary>
    public static class ErrorUtils
    {

        /// <summary>
        /// Displays an error message from an exception.
        /// </summary>
        /// <param name="logger">Logger to use</param>
        /// <param name="errorMessage">Display error message.</param>
        /// <param name="ex">Exception caught</param>
        public static void DisplayError(ILogger logger, string errorMessage, Exception ex)
        {
            var grException = GrException.From(ex, errorMessage);
            logger.Error(grException, 
                $"{grException.ErrorCode} - {grException.Message} - {errorMessage}");
            MessageBox.Show(errorMessage);
        }
        
    }

}