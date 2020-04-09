using System.Threading.Tasks;
using CommonsLib_DATA.Model.Entities;

namespace CommonsLib_DATA.Repositories
{
    /// <summary>
    /// Repository to access Last Successful Sync States in Local Storage.
    /// </summary>
    public interface ILastSuccessfulSyncRepository : IDataRepository<LastSuccessfulSyncEntity, int>
    {

        /// <summary>
        /// Finds a LastSuccessfulSyncEntity for a provided table name and backend id.
        /// </summary>
        /// <param name="tableId">Table name or id.</param>
        /// <param name="backendId">Backend sync server id.</param>
        /// <returns>Found Last Successful Sync Entity.</returns>
        Task<LastSuccessfulSyncEntity> FindByTableIdAndBackendId(string tableId, string backendId = "default");

    }
}