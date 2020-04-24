using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using CommonsLib_BLL.Config;
using CommonsLib_DAL.Attributes;
using CommonsLib_DAL.Extensions;
using CommonsLib_DAL.Initializers;
using CommonsLib_IOC.Config.Modules;
using Microsoft.Extensions.Configuration;
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
        public readonly HashSet<string> ComponentNamespaces = new HashSet<string>
        {
            nameof(CommonsLib_DATA),
            nameof(CommonsLib_BLL)
        };

        /// <summary>
        /// Global IoC modules list.
        /// </summary>
        public HashSet<Module> ExternalIoCModules { get; } = new HashSet<Module>
        {
            LocalDbModule.Self,
            LoggerModule.Self,
            GlobalConfigModule.Self
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

            // Register Components Not Primary, Not Optional
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t =>
                    t.HasAttribute<ComponentAttribute>()
                    && !t.GetAttributeIfExists<ComponentAttribute>().Primary
                    && !t.HasAttribute<OptionalOnPropertyAttribute>()
                ).AsImplementedInterfaces()
                .AsSelf()
                .PropertiesAutowired()
                .SingleInstance();
            
            // Register Components Not Primary, Optional
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t =>
                    t.HasAttribute<ComponentAttribute>()
                    && !t.GetAttributeIfExists<ComponentAttribute>().Primary
                    && t.HasAttribute<OptionalOnPropertyAttribute>()
                    && GlobalConfigManager.ConfigRoot.GetValue(
                        t.GetAttributeIfExists<OptionalOnPropertyAttribute>().Value,
                        t.GetAttributeIfExists<OptionalOnPropertyAttribute>().DefaultValue)
                ).AsImplementedInterfaces()
                .AsSelf()
                .PropertiesAutowired()
                .SingleInstance();

            // Register Components Primary, Not Optional
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t =>
                    t.HasAttribute<ComponentAttribute>()
                    && t.GetAttributeIfExists<ComponentAttribute>().Primary
                    && !t.HasAttribute<OptionalOnPropertyAttribute>()
                ).AsImplementedInterfaces()
                .AsSelf()
                .PropertiesAutowired()
                .SingleInstance();

            // Register Components Primary, Optional
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t =>
                    t.HasAttribute<ComponentAttribute>()
                    && t.GetAttributeIfExists<ComponentAttribute>().Primary
                    && t.HasAttribute<OptionalOnPropertyAttribute>()
                    && GlobalConfigManager.ConfigRoot.GetValue(
                        t.GetAttributeIfExists<OptionalOnPropertyAttribute>().Value,
                        t.GetAttributeIfExists<OptionalOnPropertyAttribute>().DefaultValue)
                ).AsImplementedInterfaces()
                .AsSelf()
                .PropertiesAutowired()
                .SingleInstance();

            // Register externally added components.
            Self.ExternalIoCModules.ToList().ForEach(module => builder.RegisterModule(module));

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