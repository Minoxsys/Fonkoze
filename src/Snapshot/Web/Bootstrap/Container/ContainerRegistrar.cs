using Autofac;
using Autofac.Core;
using Autofac.Features.ResolveAnything;
using System.Linq;
using System.Web.Mvc;
using Web.BackgroundJobs;
using Web.Controllers;
using Web.Services.Configuration;
using Web.Services.SendEmail;

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

            //container.RegisterType<AddAlertsJob>()
            //         .AsSelf()
            //         .OnActivated(e => { e.Instance.PreconfiguredEmailService = (PreconfiguredEmailService) e.Context.ResolveNamed<IEmailSendingService>("config"); });
        }

        private static void AutoWireControllerProperties(ContainerBuilder container)
        {
            var types = typeof (HomeController).Assembly.GetTypes();

            types.ToList().ForEach(it =>
                {
                    if (it.BaseType == typeof (Controller))
                    {
                        container.RegisterType(it).PropertiesAutowired();
                    }

                });
        }
    }
}