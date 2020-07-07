namespace CommonsLib_DAL.Data
{
    /// <summary>
    /// Data Domain to represent a page request.
    /// </summary>
    public interface IPageable
    {
        /// <summary>
        /// Page Number.
        /// </summary>
        int Page { get; }

        /// <summary>
        /// Page Size.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Page Offset.
        /// </summary>
        int Offset { get; }
    }
}