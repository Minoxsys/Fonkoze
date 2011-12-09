using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using Core.Services;
using Web.Security;
using Autofac;

namespace Web.Bootstrap.Container
{
	public class AuthRegistrar
	{
		public static void Register( ContainerBuilder container )
		{
			container.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>();
            container.RegisterType<AuthenticateUser>().As<IMembershipService>();
            container.RegisterType<SecurePassword>().As<ISecurePassword>();
		}
	}
}