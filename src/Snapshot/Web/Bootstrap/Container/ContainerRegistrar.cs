using Autofac;
using Autofac.Features.ResolveAnything;
using System.Linq;
using System.Web.Mvc;
using Infrastructure.Logging;
using Web.Controllers;
using Web.ReceiveSmsUseCase.Services.MessageParsers;
using Web.ReceiveSmsUseCase.Services.MessageParsers.Fonkoze;
using Web.BackgroundJobs.BackgroundJobsServices;
using Web.BackgroundJobs;

namespace Web.Bootstrap.Container
{
    public class ContainerRegistrar
    {

        public static void Register(ContainerBuilder container)
        {
            AutoWireControllerProperties(container);

            AuthRegistrar.Register(container);

            PersistenceRegistrar.Register(container);

            ServicesConventionsRegistrar.Register(container);

            container.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource(type => type.Assembly.FullName.StartsWith("Web")));

            container.RegisterType<FonkozeSmsTextParserService>().As<ISmsTextParserService>();
            container.RegisterType<ElmahLogger>().As<ILogger>();

            container.RegisterType<EmptyJobExecutionService>().Keyed<IJobExecutionService>(JobExecutionType.EmptyJobType);
            container.RegisterType<CampaignJobExecutionService>().Keyed<IJobExecutionService>(JobExecutionType.CampaignJobType);
            container.RegisterType<OutpostInactivityJobExecutionService>().Keyed<IJobExecutionService>(JobExecutionType.OutpostInactivityType);
            container.RegisterType<SmsMessageMonitoringJobExecutionService>().Keyed<IJobExecutionService>(JobExecutionType.SmsMessageMonitoringType);
            container.RegisterType<StockLevelsMonitoringJobExecutionService>().Keyed<IJobExecutionService>(JobExecutionType.StockLevelMonitoringType);
            container.RegisterType<TrimLogJobExecutionService>().Keyed<IJobExecutionService>(JobExecutionType.TrimLogType);
        }

        private static void AutoWireControllerProperties(ContainerBuilder container)
        {
            var types = typeof(HomeController).Assembly.GetTypes();

            types.ToList().ForEach(it =>
                {
                    if (it.BaseType == typeof(Controller))
                    {
                        container.RegisterType(it).PropertiesAutowired();
                    }

                });
        }
    }
}