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
        : IDataRepository<TEntity, TId>
        where TEntity : class, new()
    {
        private ILogger _logger = LoggerManager.MainLogger;

        public ILogger Logger
        {
            get => _logger;
            set => _logger = value.ForContext(GetType());
        }
        
        public SQLiteAsyncConnection SqLiteConnection { get; set; }

        protected DataRepository() { }

        /// <inheritdoc/>
        public async Task<string> FetchTableName()
        {
            var tableMapping = await SqLiteConnection.GetMappingAsync<TEntity>();
            var tableName = tableMapping.TableName; 
            Logger.Debug($"Fetching Table Name: {tableName}");
            return tableName;
        }

        /// <inheritdoc/>
        public async Task<string> FetchColumnNameFromAttribute<TAttribute>() 
            where TAttribute : Attribute
        {
            var tableMapping = await SqLiteConnection.GetMappingAsync<TEntity>();
            var propertyNameForAttribute = ObjectAttributesUtils
                .GetPropertyNameForAttribute<TEntity, TAttribute>();

            return tableMapping.FindColumnWithPropertyName(propertyNameForAttribute)?.Name ?? string.Empty;
        }


        /// <inheritdoc/>
        public async Task<List<TEntity>> FindAll()
        {
            return await SqLiteConnection.Table<TEntity>().ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IPage<TEntity>> FindAll(IPageable pageable)
        {
            var total = await SqLiteConnection.Table<TEntity>().CountAsync();
            var content = await SqLiteConnection.Table<TEntity>()
                .Skip(pageable.Offset)
                .Take(pageable.Size)
                .ToListAsync();

            return Page<TEntity>.With(content, pageable, total);
        }


        /// <inheritdoc/>
        public async Task<TEntity> FindById(TId id)
        {
            try
            {
                var item = await SqLiteConnection.GetAsync<TEntity>(id);
                return item;
            }
            catch (InvalidOperationException ex)
            {
                var errorCode = DataErrorCodes.ItemNotFound;
                throw new GrException(errorCode.Message, cause: ex, errorCode: errorCode);   
            }
        }

        /// <inheritdoc/>
        public async Task<List<TEntity>> FindAllById(IEnumerable<TId> ids)
        {
            var tableName = await FetchTableName();
            var idColName = await FetchColumnNameFromAttribute<PrimaryKeyAttribute>();
            var strIds = string.Join(", ", ids.Select(id => $"'{id}'"));

            return await SqLiteConnection.QueryAsync<TEntity>(
                $"SELECT * FROM {tableName} WHERE {idColName} in ({strIds})");
        }

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


        /// <inheritdoc/>
        public Task RunOnTransaction(Action transactionProcess)
        {
            return SqLiteConnection.RunInTransactionAsync(connection => transactionProcess());
        }

        /// <inheritdoc/>
        public async Task<T> RunOnTransaction<T>(Func<T> transactionProcess)
        {
            return await RunTaskOnTransaction(() => Task.FromResult(transactionProcess()));
        }

        /// <inheritdoc/>
        public Task<T> RunTaskOnTransaction<T>(Func<Task<T>> transactionProcess)
        {
            var t = new TaskCompletionSource<T>();
            RunOnTransaction(() =>
            {
                var transactionTask = transactionProcess();
                transactionTask.Wait();
                t.TrySetResult(
                    transactionTask.Result
                );
            });
            return t.Task;
        }
        
    }
}