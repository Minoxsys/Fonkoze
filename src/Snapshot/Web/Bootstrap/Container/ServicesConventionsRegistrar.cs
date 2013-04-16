using Autofac;
using Core.Services;
using Persistence.Security;
using Web.Services;

namespace Web.Bootstrap.Container
{
    public class ServicesConventionsRegistrar
    {
        public static void Register(ContainerBuilder container)
        {
            var webAssembly = typeof (SmsGatewayService).Assembly;
            ApplyServicesConvention(container, webAssembly);
            ApplyFactoryConvention(container, webAssembly);

            var persistenceAssembly = typeof (FunctionRightsService).Assembly;
            ApplyServicesConvention(container, persistenceAssembly);
            ApplyFactoryConvention(container, persistenceAssembly);

            var coreAssembly = typeof (MimeTypeResolverService).Assembly;
            ApplyServicesConvention(container, coreAssembly);
            ApplyFactoryConvention(container, coreAssembly);
        }

        private static void ApplyServicesConvention(ContainerBuilder container, System.Reflection.Assembly fromAssembly)
        {
            container.RegisterAssemblyTypes(fromAssembly)
                     .Where(t => t.Name.EndsWith("Service"))
                     .AsImplementedInterfaces();
        }

        private static void ApplyFactoryConvention(ContainerBuilder container, System.Reflection.Assembly fromAssembly)
        {
            container.RegisterAssemblyTypes(fromAssembly)
                     .Where(t => t.Name.EndsWith("Factory"))
                     .AsImplementedInterfaces();
        }
    }
}