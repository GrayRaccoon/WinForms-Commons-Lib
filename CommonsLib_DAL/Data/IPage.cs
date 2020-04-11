using System;
using System.Collections.Generic;

namespace CommonsLib_DAL.Data
{
    /// <summary>
    /// Data Domain to represent a Page of items.
    /// </summary>
    /// <typeparam name="T">Page items type.</typeparam>
    public interface IPage<T>
    {
        /// <summary>
        /// Page info as Pageable.
        /// </summary>
        IPageable PageInfo { get; }

        /// <summary>
        /// Page Content.
        /// </summary>
        List<T> Content { get; }

        /// <summary>
        /// Total of Items.
        /// </summary>
        int Total { get; }

        /// <summary>
        /// Maps Page content type to another page with same details but new mapped type.
        /// </summary>
        /// <param name="mapper">Function to convert Page items.</param>
        /// <typeparam name="TMapped">New Page expected type.</typeparam>
        /// <returns>New page with new mapped type.</returns>
        IPage<TMapped> Map<TMapped>(Func<T, TMapped> mapper);

    }
}