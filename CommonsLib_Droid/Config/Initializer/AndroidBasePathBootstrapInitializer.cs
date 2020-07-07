using System;
using System.Threading.Tasks;
using CommonsLib_DAL.Config;
using CommonsLib_DAL.Initializers;

namespace CommonsLib_Droid.Config.Initializer
{
    /// <summary>
    /// Android Base Path Bootstrap Initializer.
    /// </summary>
    public class AndroidBasePathBootstrapInitializer : IAppInitializer
    {
        private AndroidBasePathBootstrapInitializer() { }
        public static readonly AndroidBasePathBootstrapInitializer Self = new AndroidBasePathBootstrapInitializer();

        /// <inheritdoc/>
        public string InitializerName => "Android Base Path Initializer";

        /// <inheritdoc/>
        public int Order => -9;

        /// <summary>
        ///     Add Current App Domain Directory as the App Base Path.
        /// </summary>
        public Task DoInitialize()
        {
            return Task.Run(() =>
            {
                // Initializes Base Path for the app.
                BasePathManager.BasePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            });
        }
    }
}