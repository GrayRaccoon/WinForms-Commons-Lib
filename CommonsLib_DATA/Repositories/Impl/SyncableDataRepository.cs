using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonsLib_DATA.Attributes;
using SQLite;

namespace CommonsLib_DATA.Repositories.Impl
{
    /// <inheritdoc cref="ISyncableDataRepository{TEntity,TId}" />
    public abstract class SyncableDataRepository<TEntity, TId>
        : DataRepository<TEntity, TId>, ISyncableDataRepository<TEntity, TId>
        where TEntity : class, new()
    {

        public ILastSuccessfulSyncRepository LastSuccessfulSyncRepository { get; set; }

        protected SyncableDataRepository() : base() { }

        /// <inheritdoc/>
        public async Task<DateTimeOffset> FetchLastSuccessfulSync(string backendId = "default")
        {
            var tableName = await FetchTableName();
            var lastSuccessfulSync = await LastSuccessfulSyncRepository
                .FindByTableIdAndBackendId(tableName, backendId);
            
            return lastSuccessfulSync.SyncTimestamp;
        }

        public async Task<List<TEntity>> FindAllPendingToSync(DateTimeOffset lastSuccessfulSync, params TId[] newerIds)
        {
            newerIds ??= new TId[0];
            var tableName = await FetchTableName();
            var idColumn = await FetchColumnNameFromAttribute<PrimaryKeyAttribute>();
            var createdAtColumn = await FetchColumnNameFromAttribute<CreatedAtColumnAttribute>();
            var updatedAtColumn = await FetchColumnNameFromAttribute<UpdatedAtColumnAttribute>();
            var strNewerIds = string.Join(", ", newerIds.Select(id => $"'{id}'"));

            return await SqLiteConnection.QueryAsync<TEntity>(
                $"SELECT * FROM {tableName} WHERE {createdAtColumn} >= ? " +
                $"OR ( {updatedAtColumn} >= ? AND {idColumn} not in ({strNewerIds}) )",
                lastSuccessfulSync, lastSuccessfulSync);
        }
        
    }
}