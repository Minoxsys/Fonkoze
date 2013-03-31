using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Web.Services;
using Persistence.Commands;
using Persistence.Security;
using Core.Services;

namespace Web.Bootstrap.Container
{
    public class ServicesConventionsRegistrar
    {
        public static void Register(ContainerBuilder container)
        {
            var webAssembly = typeof(SmsGatewayService).Assembly;
            ApplyServicesConvention(container, webAssembly);

            var persistenceAssembly = typeof(FunctionRightsService).Assembly;
            ApplyServicesConvention(container, persistenceAssembly);

            var coreAssembly = typeof(MimeTypeResolverService).Assembly;
            ApplyServicesConvention(container, coreAssembly);
        }

        private static void ApplyServicesConvention(ContainerBuilder container, System.Reflection.Assembly fromAssembly)
        {
            container.RegisterAssemblyTypes(fromAssembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces();
        }
    }
}