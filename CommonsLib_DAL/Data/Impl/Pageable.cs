namespace CommonsLib_DAL.Data.Impl
{
    /// <inheritdoc/>
    public class Pageable: IPageable
    {
        /// <inheritdoc/>
        public int Page { get; private set; }

        /// <inheritdoc/>
        public int Size { get; private set; }

        /// <inheritdoc/>
        public int Offset => Size * Page;

        /// <summary>
        /// Creates a new Pageable from the page specifications.
        /// </summary>
        /// <param name="page">Number of page.</param>
        /// <param name="size">Size per page.</param>
        /// <returns>New Pageable from page specifications</returns>
        public static IPageable Of(int page, int size)
        {
            return new Pageable
            {
                Page = page,
                Size = size
            };
        }

    }
}