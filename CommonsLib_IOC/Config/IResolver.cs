using System;

namespace CommonsLib_IOC.Config
{
    /// <summary>
    /// IoC Container Resolver.
    /// </summary>
    public interface IResolver: IDisposable
    {

        /// <summary>
        /// Resolves an Instance of Certain Type.
        /// </summary>
        /// <typeparam name="T">Expected instance Type.</typeparam>
        /// <returns>Found instance for provided type.</returns>
        T ResolveInstance<T>();
    }
}