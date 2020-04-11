using System;

namespace CommonsLib_DAL.Errors
{
    /// <summary>
    /// General Application Exception Class.
    /// </summary>
    public class GrException: SystemException
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
    }

}