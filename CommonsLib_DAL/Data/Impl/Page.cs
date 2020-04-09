using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonsLib_DAL.Data.Impl
{
    /// <inheritdoc/>
    public class Page<T> : IPage<T>
    {
        /// <inheritdoc/>
        public IPageable PageInfo { get; private set; } = Pageable.Of(0,0);

        /// <inheritdoc/>
        public List<T> Content { get; private set; } = new List<T>(Enumerable.Empty<T>());

        /// <inheritdoc/>
        public int Total { get; private set; }

        /// <inheritdoc/>
        public IPage<TMapped> Map<TMapped>(Func<T, TMapped> mapper)
        {
            return Page<TMapped>.With(
                Content.Select(mapper).ToList(),
                PageInfo, Total);
        }


        /// <summary>
        /// Creates a new page from page details.
        /// </summary>
        /// <param name="content">Page content.</param>
        /// <param name="pageable">Page Info.</param>
        /// <param name="total">Total of Items.</param>
        /// <returns>New page from page details.</returns>
        public static IPage<T> With(IEnumerable<T> content, IPageable pageable, int total)
        {
            return new Page<T>
            {
                Content = new List<T>(content),
                PageInfo = pageable,
                Total = total
            };
        }

    }
}