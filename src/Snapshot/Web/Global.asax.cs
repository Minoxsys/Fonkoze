using Autofac;
using Autofac.Integration.Mvc;
using Persistence;
using System.Web.Mvc;
using Web.BackgroundJobs;
using Web.Bootstrap.Container;
using Web.Bootstrap.Routes;
using Web.CustomModelBinders;
using Web.CustomViewEngine;
using Web.Security;
using WebBackgrounder;

namespace Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication, IContainerAccessor
    {
        private static JobManager _jobManager;
        private static IContainer _container;

        IContainer IContainerAccessor.Container
        {
            get { return _container; }

        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        protected void Application_Start()
        {
            ReplaceDefaultViewEngine();

            InitializeContainer();

            _jobManager = CreateJobWorkersManager();
            _jobManager.Start();

            AreaRegistration.RegisterAllAreas();

            ModelBinders.Binders.Add(typeof (UserAndClientIdentity), _container.Resolve<UserAndClientIdentityModelBinder>());

            RegisterGlobalFilters(GlobalFilters.Filters);
            RoutesRegistrar.Register();
        }

        private void ReplaceDefaultViewEngine()
        {
            ViewEngines.Engines.Clear();

            var engine = new CustomPathsRazorViewEngine();
            engine.AddViewLocationFormat("~/WarehouseMgmtUseCase/Views/{1}/{0}.cshtml");
            engine.AddViewLocationFormat("~/WarehouseMgmtUseCase/Views/{1}/{0}.vbhtml");

            // Add a shared location too, as the lines above are controller specific
            engine.AddPartialViewLocationFormat("~/WarehouseMgmtUseCase/Views/Shared/{0}.cshtml");
            engine.AddPartialViewLocationFormat("~/WarehouseMgmtUseCase/Views/Shared/{0}.vbhtml");

            ViewEngines.Engines.Add(engine);
        }

        protected void Application_Stop()
        {
            _jobManager.Stop();
        }

        /// <summary>
        /// Instantiate the container and add all Controllers that derive from 
        /// UnityController to the container.  Also associate the Controller 
        /// with the UnityContainer ControllerFactory.
        /// </summary>
        protected static void InitializeContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof (MvcApplication).Assembly);
            builder.RegisterFilterProvider();
            ContainerRegistrar.Register(builder);
            _container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
        }

        private static JobManager CreateJobWorkersManager()
        {
            var jobs = new IJob[]
                {
                    _container.Resolve<EmptyJob>(),
                    _container.Resolve<StockLevelsMonitoringJob>(),
                    _container.Resolve<CampaignExecutionJob>(),
                    _container.Resolve<OutpostInactivityJob>(),
                    _container.Resolve<SmsMessagesMonitoringJob>(),
                    new TrimLogJob(() => _container.Resolve<INHibernateSessionFactory>().CreateSession()) 
                    //new SampleJob(TimeSpan.FromSeconds(35), TimeSpan.FromSeconds(60)),
                    /* new ExceptionJob(TimeSpan.FromSeconds(15)), */
                    // new WorkItemCleanupJob(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5), new WorkItemsContext())
                };

            var coordinator =
                new WebFarmJobCoordinator(new NHibernateWorkItemRepository(() => _container.Resolve<INHibernateSessionFactory>().CreateSession()));
            var manager = new JobManager(jobs, coordinator) {RestartSchedulerOnFailure = true};
            manager.Fail(ex => Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex)));

            return manager;
        }
    }
}