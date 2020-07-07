using System.Threading.Tasks;
using CommonsLib_DAL.Attributes;
using CommonsLib_DAL.Errors;
using CommonsLib_DATA.Errors;
using CommonsLib_DATA.Model.Entities;

namespace CommonsLib_DATA.Repositories.Impl
{
    /// <inheritdoc cref="ILastSuccessfulSyncRepository"/>
    [Component]
    public class LastSuccessfulSyncRepository : DataRepository<LastSuccessfulSyncEntity, int>,
        ILastSuccessfulSyncRepository
    {
        public async Task<LastSuccessfulSyncEntity> FindByTableIdAndBackendId(string tableId,
            string backendId = "default")
        {
            var item = await SqLiteConnection.FindWithQueryAsync<LastSuccessfulSyncEntity>(
                "SELECT lss.* FROM last_successful_sync lss WHERE lss.table_id = ? AND lss.backend_id = ?",
                tableId, backendId
            );
            if (item != default) return item;
            var errorCode = DataErrorCodes.ItemNotFound;
            throw new GrException(errorCode.Message, errorCode: errorCode);
        }
    }
}