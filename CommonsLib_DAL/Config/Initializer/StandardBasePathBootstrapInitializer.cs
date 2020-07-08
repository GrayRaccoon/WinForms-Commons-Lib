using System.Threading.Tasks;
using CommonsLib_DAL.Initializers;

namespace CommonsLib_DAL.Config.Initializer
{
    /// <summary>
    /// Standard Base Path Bootstrap initializer.
    /// </summary>
    public sealed class StandardBasePathBootstrapInitializer : IAppInitializer
    {
        private StandardBasePathBootstrapInitializer()
        { }

        public static readonly StandardBasePathBootstrapInitializer Self = new StandardBasePathBootstrapInitializer();

        /// <inheritdoc/>
        public string InitializerName => "Standard Base Path Initializer";

        /// <inheritdoc/>
        public int Order => -9;

        /// <summary>
        /// Add Current App Domain Directory as the App Base Path. 
        /// </summary>
        public Task DoInitialize()
        {
            return Task.Run(() =>
            {
                // Initializes Base Path for the app.
                BasePathManager.BasePath = System.AppDomain.CurrentDomain.BaseDirectory;
            });
        }
    }
}