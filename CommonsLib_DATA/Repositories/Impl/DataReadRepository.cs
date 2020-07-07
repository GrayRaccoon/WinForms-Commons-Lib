using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonsLib_DAL.Config;
using CommonsLib_DAL.Data;
using CommonsLib_DAL.Data.Impl;
using CommonsLib_DAL.Errors;
using CommonsLib_DAL.Utils;
using CommonsLib_DATA.Errors;
using Serilog;
using SQLite;

namespace CommonsLib_DATA.Repositories.Impl
{
    /// <inheritdoc/>
    public class DataReadRepository<TEntity, TId>
        : IDataReadRepository<TEntity, TId>
        where TEntity : class, new()
    {
        private ILogger _logger = LoggerManager.MainLogger;

        public ILogger Logger
        {
            get => _logger;
            set => _logger = value.ForContext(GetType());
        }

        public SQLiteAsyncConnection SqLiteConnection { get; set; }

        protected DataReadRepository()
        { }

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
                var result = transactionTask.Result;
                Task.Run(() =>
                {
                    Task.Delay(1).Wait();
                    t.TrySetResult(result);
                });
            });
            return t.Task;
        }
    }
}