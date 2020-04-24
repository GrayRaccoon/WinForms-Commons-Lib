using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonsLib_DAL.Config;
using CommonsLib_DAL.Data;
using CommonsLib_DAL.Data.Impl;
using CommonsLib_DAL.Errors;
using CommonsLib_DAL.Extensions;
using CommonsLib_DAL.Utils;
using CommonsLib_DATA.Attributes;
using CommonsLib_DATA.Errors;
using Serilog;
using SQLite;

namespace CommonsLib_DATA.Repositories.Impl
{
    /// <inheritdoc/>
    public abstract class DataRepository<TEntity, TId>
        : DataReadRepository<TEntity, TId>, IDataRepository<TEntity, TId>
        where TEntity : class, new()
    {
        protected DataRepository(): base() { }

        /// <inheritdoc/>
        public async Task<TEntity> Save(TEntity entity, bool updateTimestamp)
        {
            var now = DateTimeOffset.Now;

            var entityId = entity.GetValueForAttribute<TId, PrimaryKeyAttribute>();
            var hasId = entityId != null && !entityId.Equals(default(TId));

            if (updateTimestamp)
            {
                if (!hasId)
                {
                    entity.SetValueForAttribute<CreatedAtColumnAttribute>(now);
                }
                entity.SetValueForAttribute<UpdatedAtColumnAttribute>(now);   
            }

            var isUpdate = hasId;
            if (hasId)
            {
                var tableName = await FetchTableName();
                var idColName = await FetchColumnNameFromAttribute<PrimaryKeyAttribute>();
                
                var oldRecord = await SqLiteConnection.FindWithQueryAsync<TEntity>(
                    $"SELECT * FROM {tableName} WHERE {idColName} = ?", entityId);

                if (oldRecord == default) isUpdate = false;
            }

            var saveTask = isUpdate ? SqLiteConnection.UpdateAsync(entity) : SqLiteConnection.InsertAsync(entity);
            await saveTask;
            return entity;
        }

        /// <inheritdoc/>
        public async Task<List<TEntity>> SaveAll(IEnumerable<TEntity> entities, bool updateTimestamp = false)
        {
            return await RunTaskOnTransaction(async () =>
            {
                var savedEntities = new List<TEntity>();

                foreach (var entity in entities)
                {
                    var savedEntity = await Save(entity, updateTimestamp);
                    savedEntities.Add(savedEntity);
                }

                return savedEntities;
            });
        }

    }
}