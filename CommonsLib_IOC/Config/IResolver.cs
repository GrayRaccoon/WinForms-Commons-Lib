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

        /// <summary>
        /// Resolves all available instances for properties of given service. 
        /// </summary>
        /// <param name="service"></param>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Instance with autowired properties</returns>
        TService InjectProperties<TService>(TService service);
    }
}