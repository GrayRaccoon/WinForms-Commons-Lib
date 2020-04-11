using System.Threading.Tasks;

namespace CommonsLib_DAL.Initializers
{
    /// <summary>
    /// App Initializer
    /// </summary>
    public interface IAppInitializer
    {
        /// <summary>
        /// Initializer order if required.
        /// </summary>
        int Order { get; }
        
        /// <summary>
        /// Gets the name of the current app initializer.
        /// </summary>
        string InitializerName { get; }
        
        /// <summary>
        /// Perform an initialization process.
        /// </summary>
        /// <returns></returns>
        Task DoInitialize();
    }

}