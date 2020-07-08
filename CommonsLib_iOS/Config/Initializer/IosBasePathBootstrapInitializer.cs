using System;
using System.IO;
using System.Threading.Tasks;
using CommonsLib_DAL.Config;
using CommonsLib_DAL.Initializers;
using Xamarin.Essentials;

namespace CommonsLib_iOS.Config.Initializer
{
    /// <summary>
    /// iOS Base Path Bootstrap Initializer.
    /// </summary>
    public class IosBasePathBootstrapInitializer : IAppInitializer
    {
        private IosBasePathBootstrapInitializer() { }
        public static readonly IosBasePathBootstrapInitializer Self = new IosBasePathBootstrapInitializer();

        /// <inheritdoc/>
        public string InitializerName => "iOS Base Path Initializer";

        /// <inheritdoc/>
        public int Order => -9;

        /// <summary>
        ///     Add Current App Domain Directory as the App Base Path.
        /// </summary>
        public Task DoInitialize()
        {
            return Task.Run(() =>
            {
                var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var basePath = Path.Combine(docFolder, "..", "Library", AppInfo.PackageName);
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                // Initializes Base Path for the app.
                BasePathManager.BasePath = basePath;
            });
        }
    }
}