using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommonsLib_DAL.Errors;
using CommonsLib_DAL.Extensions;
using CommonsLib_DAL.Utils;
using CommonsLib_DATA.Attributes;
using SQLite;

namespace CommonsLib_DATA.Repositories.Impl
{
    /// <inheritdoc/>
    public abstract class DataRepository<TEntity, TId>
        : DataReadRepository<TEntity, TId>, IDataRepository<TEntity, TId>
        where TEntity : class, new()
    {
        protected DataRepository() : base()
        { }

        /// <inheritdoc/>
        public async Task<TEntity> Save(TEntity entity, bool updateTimestamp = true)
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

        /// <inheritdoc/>
        public async Task DeleteById(TId id, bool softDelete = true)
        {
            if (softDelete)
                await SoftDeleteEntity(id);
            else
                await HardDeleteEntity(id);
        }

        /// <inheritdoc/>
        public async Task DeleteAllById(IEnumerable<TId> ids, bool softDelete = true)
        {
            await RunOnTransaction(() =>
            {
                foreach (var id in ids)
                    DeleteById(id, softDelete).Wait();
            });
        }

        /// <inheritdoc/>
        public async Task DeleteAll(bool softDelete = true)
        {
            await RunOnTransaction(() =>
            {
                var entitiesTask = FindAll();
                entitiesTask.Wait();
                var entities = entitiesTask.Result;
                foreach (var id in entities.Select(entity => entity.GetValueForAttribute<TId, PrimaryKeyAttribute>()))
                    DeleteById(id, softDelete).Wait();
                
            });
        }


        /// <summary>
        /// Performs hard delete for given id.
        /// </summary>
        /// <param name="id">id to hard delete</param>
        /// <returns>Process task.</returns>
        protected async Task HardDeleteEntity(TId id)
        {
            var tableName = await FetchTableName();
            var idColName = await FetchColumnNameFromAttribute<PrimaryKeyAttribute>();

            await SqLiteConnection.ExecuteAsync($"DELETE FROM {tableName} WHERE {idColName} = ?", id);
        }

        /// <summary>
        /// Performs soft delete for given id.
        /// </summary>
        /// <param name="id">id to soft delete</param>
        /// <returns>Process task.</returns>
        protected async Task SoftDeleteEntity(TId id)
        {
            TEntity entity;
            try
            {
                entity = await FindById(id);
            }
            catch (GrException grException)
            {
                Logger.Warning("Soft delete failed due to non existing entity.", grException);
                return;
            }

            var softDeleteAttr =
                ObjectAttributesUtils.GetAttributeForAnyFieldIn<IsDeletedFlagColumnAttribute>(entity.GetType());
            if (softDeleteAttr == null)
            {
                Logger.Warning("Not able to perform soft-delete due to non existing soft delete column.");
                return;
            }
            entity.SetValueForAttribute<IsDeletedFlagColumnAttribute>(softDeleteAttr.IsDeletedValue);
            await Save(entity);
        }
        
    }
}