using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommonsLib_DAL.Config;
using CommonsLib_DAL.Initializers;
using Microsoft.Data.Sqlite;
using SQLite;

namespace CommonsLib_DATA.Config.Initializers
{
    /// <summary>
    /// Initializer class for Local Storage
    /// </summary>
    public class SqLiteBootstrapInitializer: IAppInitializer
    {
        private SqLiteBootstrapInitializer() {}
        public static readonly SqLiteBootstrapInitializer Self = new SqLiteBootstrapInitializer();

        /// <summary>
        /// Global migration namespaces list.
        /// </summary>
        public readonly List<string> MigrationsNamespaces = new List<string>
        {
            nameof(CommonsLib_DATA)
        };

        /// <inheritdoc/>
        public string InitializerName => "Local Storage SQLite Initializer";
        
        /// <inheritdoc/>
        public int Order => 40;

        /// <summary>
        /// Initialize SQLite Database.
        /// </summary>
        public Task DoInitialize()
        {
            return Task.Run(() =>
            {
                // Get assemblies and filters from MigrationsNamespaces
                var embeddedMigrationAssemblies = new List<Assembly>();
                var embeddedMigrationsFilters = new List<string>();

                foreach (var migrationsNamespace in MigrationsNamespaces.Distinct().ToList())
                {
                    embeddedMigrationAssemblies.Add(Assembly.Load(migrationsNamespace));
                    embeddedMigrationsFilters.Add($"{migrationsNamespace}.db");
                }

                // Build connection string.
                var dbFilePath = LocalDbManager.DatabaseFile;
                var msConnString = $"Data Source={dbFilePath};";
                var sqliteConnection = new SqliteConnection(msConnString);

                // Apply Migrations.
                var evolve = new Evolve.Evolve(sqliteConnection, 
                    LoggerManager.MainLogger.ForContext<SqLiteBootstrapInitializer>().Information)
                {
                    EmbeddedResourceAssemblies = embeddedMigrationAssemblies.ToArray(),
                    EmbeddedResourceFilters = embeddedMigrationsFilters.ToArray(),
                    IsEraseDisabled = LocalDbManager.EraseSchemaIfValidationFails
                };
                evolve.Migrate();

                // Initialize LocalDB
                LocalDbManager.DbConnection = new SQLiteAsyncConnection(dbFilePath);
            });
        }

    }

}