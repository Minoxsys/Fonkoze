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
using WebBackgrounder;
using Autofac.Core;

namespace Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication, IContainerAccessor
    {
        private static JobManager jobManager;

        private static IContainer container;

        IContainer IContainerAccessor.Container
        {
            get
            {
                return container;
            }
        }


        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        protected void Application_Start()
        {
            InitializeContainer();

            jobManager = CreateJobWorkersManager();
            jobManager.Start();

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RoutesRegistrar.Register();
        }

        protected void Application_Stop()
        {
            jobManager.Stop();
        }


        /// <summary>
        /// Instantiate the container and add all Controllers that derive from 
        /// UnityController to the container.  Also associate the Controller 
        /// with the UnityContainer ControllerFactory.
        /// </summary>
        protected static void InitializeContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            ContainerRegistrar.Register(builder);
            container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static JobManager CreateJobWorkersManager()
        {
            var jobs = new IJob[]
            {
                container.Resolve<Web.BackgroundJobs.EmptyJob>(),
                container.Resolve<Web.BackgroundJobs.AddAlertsJob>(),
                container.Resolve<Web.BackgroundJobs.CampaignExecutionJob>()
                
                //new SampleJob(TimeSpan.FromSeconds(35), TimeSpan.FromSeconds(60)),
                /* new ExceptionJob(TimeSpan.FromSeconds(15)), */
                //new WorkItemCleanupJob(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5), new WorkItemsContext())
            };

            var coordinator = new WebFarmJobCoordinator(new NHibernateWorkItemRepository(() => container.Resolve<INHibernateSessionFactory>().CreateSession()));
            var manager = new JobManager(jobs, coordinator);

            //manager.Fail(ex => Elmah.ErrorLog.GetDefault(null).Log(new Error(ex)));
            return manager;
        }
    }
}