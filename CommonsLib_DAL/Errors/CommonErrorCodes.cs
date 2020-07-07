namespace CommonsLib_DAL.Errors
{
    /// <summary>
    /// Common Error Codes.
    /// </summary>
    public static class CommonErrorCodes
    {
        public static readonly IErrorCode UnknownError = ErrorCode.As("Common_999",
            "Unknown Error");

        public static readonly IErrorCode InvalidOperation = ErrorCode.As("Common_001",
            "Operation is Invalid");

        public static readonly IErrorCode InvalidObject = ErrorCode.As("Common_002",
            "Object is invalid");
    }
}