using System.IO;

namespace CommonsLib_DAL.Config
{
    /// <summary>
    /// Application Base Directory Manager.
    /// </summary>
    public static class BasePathManager
    {
        /// <summary>
        /// Global Application Base Path.
        /// This value must be changed from app entry point.
        /// </summary>
        public static string BasePath { get; set; } = ".";

        /// <summary>
        /// Combine Application base path with a file name.
        /// </summary>
        /// <param name="fileName">Required file name in base path.</param>
        /// <returns>Provided filename path from app base path.</returns>
        public static string GetFile(string fileName)
        {
            var baseDir = BasePath ?? string.Empty;
            return Path.Combine(baseDir, fileName);
        }

        /// <summary>
        /// Prefixes the base path to an input file path only if input file path is relative
        /// otherwise it returns the same file path.
        /// </summary>
        /// <param name="fileName">Input file path to prefix the base path if required.</param>
        /// <returns>Absolute path to input file path.</returns>
        public static string PrefixBasePathIfRequired(string fileName)
        {
            var isAbsolute = Path.IsPathRooted(fileName);
            return isAbsolute ? fileName : GetFile(fileName);
        }

    }
}