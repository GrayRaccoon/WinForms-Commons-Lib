using CommonsLib_DAL.Errors;

namespace CommonsLib_DATA.Errors
{
    /// <summary>
    /// Error codes related to data layer.
    /// </summary>
    public static class DataErrorCodes
    {
        public static readonly IErrorCode ItemNotFound = ErrorCode.As("Data_001",
            "Item Not Found");

    }
}