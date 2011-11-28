using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Web.Bootstrap.Container;
using Web.Bootstrap.Routes;
using Persistence;
using Autofac;
using Autofac.Integration.Mvc;

namespace Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication, IContainerAccessor
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}


		protected void Application_Start()
		{
			InitializeContainer();


			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RoutesRegistrar.Register();
		}



		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			container.Resolve<INHibernateUnitOfWork>().Initialize();
		}

		protected void Application_EndRequest(object sender, EventArgs e)
		{
			container.Resolve<INHibernateUnitOfWork>().Close();
		}

		private static IContainer container;

		IContainer IContainerAccessor.Container
		{
			get
			{
				return container;
			}
		}

		/// <summary>
		/// Instantiate the container and add all Controllers that derive from 
		/// UnityController to the container.  Also associate the Controller 
		/// with the UnityContainer ControllerFactory.
		/// </summary>
		protected virtual void InitializeContainer()
		{
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            ContainerRegistrar.Register(builder);
            container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}
	}
}