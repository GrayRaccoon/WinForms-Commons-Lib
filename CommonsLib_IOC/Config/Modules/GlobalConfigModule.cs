using Autofac;
using CommonsLib_BLL.Config;

namespace CommonsLib_IOC.Config.Modules
{
    /// <summary>
    /// Global Config for IoC Container.
    /// </summary>
    public class GlobalConfigModule: Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            // Register DAL 
            builder.RegisterInstance(GlobalConfigManager.ConfigRoot)
                .AsImplementedInterfaces()
                .AsSelf();
        }
    }
}