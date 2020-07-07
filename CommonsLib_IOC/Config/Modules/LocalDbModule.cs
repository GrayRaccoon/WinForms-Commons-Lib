using Autofac;
using CommonsLib_DATA.Config;

namespace CommonsLib_IOC.Config.Modules
{
    /// <summary>
    /// Local DB Access for IoC Container.
    /// </summary>
    public class LocalDbModule : Module
    {
        private LocalDbModule()
        { }

        public static readonly LocalDbModule Self = new LocalDbModule();

        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            // Register DAL 
            builder.RegisterInstance(LocalDbManager.DbConnection).AsSelf();
        }
    }
}