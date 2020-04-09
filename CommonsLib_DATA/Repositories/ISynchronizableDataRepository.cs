using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonsLib_DATA.Repositories
{
    /// <summary>
    /// Generic Data Repository Interface
    /// </summary>
    public interface ISyncableDataRepository<TEntity, in TId>
        : IDataRepository<TEntity, TId>
        where TEntity : class, new()
    {

        /// <summary>
        /// Gets the last Successful Sync Timestamp for TEntity and specified Backend.
        /// </summary>
        /// <param name="backendId">Backend id.</param>
        /// <returns>Last Successful Sync timestamp.</returns>
        Task<DateTimeOffset> FetchLastSuccessfulSync(string backendId = "default");

        /// <summary>
        /// Finds all available TEntity missing to sync using given last successful sync time.
        /// </summary>
        /// <param name="lastSuccessfulSync">Last Successful Sync time.</param>
        /// <param name="newerIds">Just updated Ids to ignore.</param>
        /// <returns>TEntity instances pending to sync.</returns>
        Task<List<TEntity>> FindAllPendingToSync(DateTimeOffset lastSuccessfulSync,
            params TId[] newerIds);

    }
}