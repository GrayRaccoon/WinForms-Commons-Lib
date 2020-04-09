using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonsLib_DAL.Data;
using CommonsLib_DAL.Errors;

namespace CommonsLib_DATA.Repositories
{
    /// <summary>
    /// Generic Data Repository Interface
    /// </summary>
    public interface IDataRepository<TEntity, in TId> 
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
        /// <returns>Found TEntity in Storage.</returns>
        /// <exception cref="GrException">If no entity is found for given id.</exception>
        Task<TEntity> FindById(TId id);

        /// <summary>
        /// Finds a List of Entities using given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<TEntity>> FindAllById(IEnumerable<TId> ids);

        /// <summary>
        /// Saves a given TEntity.
        /// </summary>
        /// <param name="entity">Entity to save.</param>
        /// <param name="updateTimestamp">Whether or not to update the columns InsertedAt and UpdatedAt.</param>
        /// <returns>Saved Entity</returns>
        Task<TEntity> Save(TEntity entity, bool updateTimestamp = true);

        /// <summary>
        /// Saves all entities in the provided list of TEntity.
        /// </summary>
        /// <param name="entities">Entities to save.</param>
        /// <param name="updateTimestamp">Whether or not to update the columns InsertedAt and UpdatedAt.</param>
        /// <returns>Saved Entities</returns>
        Task<List<TEntity>> SaveAll(IEnumerable<TEntity> entities, bool updateTimestamp = false);


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