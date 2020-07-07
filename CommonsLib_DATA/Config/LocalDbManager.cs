using SQLite;

namespace CommonsLib_DATA.Config
{
    /// <summary>
    /// Abstract Local Db Manager Class.
    /// </summary>
    public static class LocalDbManager
    {
        /// <summary>
        /// Database file where to store db 
        /// </summary>
        public static string DatabaseFile { get; set; } = "app_data.db3";

        /// <summary>
        /// Whether or not to delete existing database schema if validation fails.
        /// </summary>
        public static bool EraseSchemaIfValidationFails { get; set; } = false;

        /// <summary>
        /// Global SQLiteAsyncConnection Instance.
        /// </summary>
        public static SQLiteAsyncConnection DbConnection { get; set; }
    }
}