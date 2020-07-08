using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonsLib_DAL.Data;
using CommonsLib_DAL.Errors;

namespace CommonsLib_DATA.Repositories
{
    /// <summary>
    /// Generic Read Only Data Repository
    /// </summary>
    public interface IDataReadRepository<TEntity, in TId>
        where TEntity : class, new()
    {
        /// <summary>
        /// Gets the TEntity table name.
        /// </summary>
        /// <returns>TEntity table name.</returns>
        Task<string> FetchTableName();

        /// <summary>
        /// Gets TEntity column name from given Attribute.
        /// </summary>
        /// <typeparam name="TAttribute">Attribute to use to find column name.</typeparam>
        /// <returns>Found column name</returns>
        Task<string> FetchColumnNameFromAttribute<TAttribute>()
            where TAttribute : Attribute;

        /// <summary>
        /// Finds All Available TEntities.
        /// </summary>
        /// <returns>List of available TEntity in Storage.</returns>
        Task<List<TEntity>> FindAll();

        /// <summary>
        /// Finds Page of Available TEntities.
        /// </summary>
        /// <param name="pageable">Page specification.</param>
        /// <returns>Page of available TEntity in Storage.</returns>
        Task<IPage<TEntity>> FindAll(IPageable pageable);

        /// <summary>
        /// Finds a TEntity by TId.
        /// </summary>
        /// <param name="id">If to find</param>
        /// <returns>Found TEntity in Storage.</returns>
        /// <exception cref="GrException">If no entity is found for given id.</exception>
        Task<TEntity> FindById(TId id);

        /// <summary>
        /// Finds a List of Entities using given ids.
        /// </summary>
        /// <param name="ids">Ids to find</param>
        /// <returns>Found TEntity's in Storage.</returns>
        Task<List<TEntity>> FindAllById(IEnumerable<TId> ids);


        /// <summary>
        /// Performs certain Action inside a DB transaction.
        /// </summary>
        /// <param name="transactionProcess">Task to be executed in a transaction.</param>
        /// <returns>Awaitable Transaction Task</returns>
        Task RunOnTransaction(Action transactionProcess);

        /// <summary>
        /// Performs certain Action inside a DB transaction and returns a result.
        /// </summary>
        /// <param name="transactionProcess">Function to be executed in a transaction.</param>
        /// <typeparam name="T">Function return type.</typeparam>
        /// <returns>Provided Function result.</returns>
        Task<T> RunOnTransaction<T>(Func<T> transactionProcess);

        /// <summary>
        /// Performs certain Async Task inside a DB transaction and returns a result.
        /// </summary>
        /// <param name="transactionProcess">Task to be executed in a transaction.</param>
        /// <typeparam name="T">Task return type.</typeparam>
        /// <returns>Provided Task result.</returns>
        Task<T> RunTaskOnTransaction<T>(Func<Task<T>> transactionProcess);
    }
}