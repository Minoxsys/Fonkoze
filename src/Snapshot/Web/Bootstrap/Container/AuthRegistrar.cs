using Autofac;
using Core.Services;
using Web.Security;

namespace Web.Bootstrap.Container
{
    public class AuthRegistrar
    {
        public static void Register(ContainerBuilder container)
        {
            container.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>();
            container.RegisterType<AuthenticateUser>().As<IMembershipService>();
            container.RegisterType<SecurePassword>().As<ISecurePassword>();
        }
    }
}