using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using CommonsLib_DAL.Attributes;
using CommonsLib_DAL.Extensions;
using CommonsLib_DAL.Initializers;
using CommonsLib_IOC.Config.Modules;
using Module = Autofac.Module;

namespace CommonsLib_IOC.Config.Initializers
{
    /// <summary>
    /// Initializer class for IoC
    /// </summary>
    public class IoCBootstrapInitializer : IAppInitializer
    {
        private IoCBootstrapInitializer() {}
        public static readonly IoCBootstrapInitializer Self = new IoCBootstrapInitializer();

        /// <summary>
        /// Component Services namespaces list.
        /// </summary>
        public readonly List<string> ComponentNamespaces = new List<string>
        {
            nameof(CommonsLib_DATA),
            nameof(CommonsLib_BLL)
        };

        /// <summary>
        /// Global IoC modules list.
        /// </summary>
        public List<Module> ExternalIoCModules { get; } = new List<Module>
        {
            new LocalDbModule(),
            new LoggerModule()
        };


        /// <inheritdoc/>
        public string InitializerName => "IoC Container Initializer";
        
        /// <inheritdoc/>
        public int Order => 100;
        
        /// <summary>
        /// Initialize IoC Container.
        /// </summary>
        public Task DoInitialize()
        {
            return Task.Run(() =>
            {
                IoCManager.Resolver = new Resolver();
            });
        }
        
        /// <summary>
        /// Create a Container from the app requirements.
        /// </summary>
        /// <returns>Initialized container.</returns>
        private static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            var assemblies = Self.ComponentNamespaces
                .Select(Assembly.Load)
                .ToArray();
                
            // Register Implemented Primary components
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t =>
                    t.HasAttribute<ComponentAttribute>() && 
                    t.GetAttributeIfExists<ComponentAttribute>().Primary
                ).AsImplementedInterfaces()
                .AsSelf()
                .PropertiesAutowired()
                .SingleInstance();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.HasAttribute<ComponentAttribute>())
                .AsImplementedInterfaces()
                .AsSelf()
                .PropertiesAutowired()
                .SingleInstance()
                .PreserveExistingDefaults();   
            
            // Register externally added components.
            Self.ExternalIoCModules.ForEach(module => builder.RegisterModule(module));

            return builder.Build();
        }

        /// <summary>
        /// High level IResolver Implementation.
        /// </summary>
        private class Resolver : IResolver
        {
            private readonly IContainer _container;

            public Resolver()
            {
                _container = Configure();
            }

            /// <inheritdoc/>
            public T ResolveInstance<T>()
            {
                var container = _container;
                T instance;
                using (var scope = container.BeginLifetimeScope())
                {
                    instance = scope.Resolve<T>();
                }
                return instance;
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                _container?.Dispose();
            }
        }

    }
}