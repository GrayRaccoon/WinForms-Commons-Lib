using Autofac;
using CommonsLib_DAL.Config;

namespace CommonsLib_IOC.Config.Modules
{
    /// <summary>
    /// Root Logger access for IoC Container.
    /// </summary>
    public class LoggerModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register Main Logger
            builder.RegisterInstance(LoggerManager.MainLogger)
                .As<Serilog.ILogger>()
                .AsSelf();
        }
    }
}