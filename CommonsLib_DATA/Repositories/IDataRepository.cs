using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonsLib_DATA.Repositories
{
    /// <summary>
    /// Generic Data Repository Interface
    /// </summary>
    public interface IDataRepository<TEntity, in TId>
        : IDataReadRepository<TEntity, TId>
        where TEntity : class, new()
    {
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
        /// Deletes entity using provided id. 
        /// </summary>
        /// <param name="id">Id to delete</param>
        /// <param name="softDelete">Whether or not soft delete entity instead of hard delete.</param>
        /// <returns>Process task.</returns>
        Task DeleteById(TId id, bool softDelete = true);

        /// <summary>
        /// Deletes entities using provided ids.
        /// </summary>
        /// <param name="ids">Id's to delete</param>
        /// <param name="softDelete">Whether or not soft delete entity instead of hard delete.</param>
        /// <returns>Process task.</returns>
        Task DeleteAllById(IEnumerable<TId> ids, bool softDelete = true);

        /// <summary>
        /// Deletes all the entities in the storage.
        /// </summary>
        /// <param name="softDelete">Whether or not soft delete entity instead of hard delete.</param>
        /// <returns>Process task.</returns>
        Task DeleteAll(bool softDelete = true);

    }
}