using System;
using SQLite;

namespace CommonsLib_DATA.Model.Entities
{
    /// <summary>
    /// Entity where to store Entities Last Successful Sync information.
    /// </summary>
    [Table("last_successful_sync")]
    public class LastSuccessfulSyncEntity
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("backend_id"), NotNull] public string BackendId { get; set; } = string.Empty;

        [Column("table_id"), NotNull] public string TableId { get; set; } = string.Empty;

        [Column("sync_timestamp"), NotNull] public DateTimeOffset SyncTimestamp { get; set; }
    }
}