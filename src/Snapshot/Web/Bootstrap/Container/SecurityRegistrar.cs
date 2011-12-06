using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Security;
using Persistence.Security;
using Autofac;

namespace Web.Bootstrap.Container
{
	public class SecurityRegistrar
	{
		public static void Register( ContainerBuilder container )
		{
			container.RegisterType<FunctionRightsService>().As<IPermissionsService>() ;
		}
	}
}