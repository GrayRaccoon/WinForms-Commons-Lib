using System;

namespace CommonsLib_DAL.Errors
{
    /// <summary>
    /// General Application Exception Class.
    /// </summary>
    public class GrException : SystemException
    {
        public IErrorCode ErrorCode { get; }

        public GrException(
            string? message = null,
            IErrorCode? errorCode = null,
            Exception? cause = null
        )
            : base(message, cause)
        {
            ErrorCode = errorCode ?? CommonErrorCodes.UnknownError;
        }

        /// <summary>
        /// Wraps an exception into a GrException if required.
        /// </summary>
        /// <param name="ex">exception to wrap</param>
        /// <param name="errorMessage">Custom error message</param>
        /// <returns>Generated GrException.</returns>
        public static GrException From(Exception ex, string? errorMessage = null)
        {
            GrException grException;
            if (ex is GrException exception)
                grException = exception;
            else if (ex.InnerException is GrException innerException)
                grException = innerException;
            else
                grException = new GrException(errorMessage ?? ex.Message, cause: ex);
            return grException;
        }
    }
}