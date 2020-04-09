namespace CommonsLib_DAL.Errors
{
    /// <summary>
    /// Error Code Interface.
    /// </summary>
    public interface IErrorCode
    {
        /// <summary>
        /// Error String Code.
        /// </summary>
        string Code { get; }
        
        /// <summary>
        /// Error Display Message.
        /// </summary>
        string Message { get; }
    }

    /// <summary>
    /// Simple IErrorCode Implementation.
    /// </summary>
    public class ErrorCode: IErrorCode
    {
        private ErrorCode(string code, string msg)
        {
            Code = code;
            Message = msg;
        }
        
        /// <inheritdoc/>
        public string Code { get; }

        /// <inheritdoc/>
        public string Message { get; }

        /// <summary>
        /// Factory method to build a new ErrorCode.
        /// </summary>
        /// <param name="code">Error string code.</param>
        /// <param name="message">Error display message.</param>
        /// <returns>Built error code.</returns>
        public static ErrorCode As(string code, string message)
        {
            return new ErrorCode(code, message);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is ErrorCode other)) return false;
            return Code == other.Code && Message == other.Message;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + (Code?.GetHashCode() ?? 0);
                hash = hash * 23 + (Message?.GetHashCode() ?? 0);
                return hash;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Code} - {Message}";
        }
    }
}