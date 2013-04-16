using Autofac;
using Core.Persistence;
using FluentNHibernate.Automapping;
using NHibernate;
using Persistence;
using Persistence.Commands;
using Persistence.Conventions;
using Persistence.Queries;
using Persistence.Queries.Countries;

namespace Web.Bootstrap.Container
{
    public class PersistenceRegistrar
    {
        public static void Register(ContainerBuilder container)
        {
            container.RegisterType<DomainEntityMappingConvention>().As<IAutomappingConfiguration>();


            container.RegisterGeneric(typeof (NHibernateSaveOrUpdateCommand<>)).As(typeof (ISaveOrUpdateCommand<>));

            container.RegisterGeneric(typeof (NHibernateDeleteCommand<>)).As(typeof (IDeleteCommand<>));

            container.RegisterType<NHibernateUnitOfWork>().As<INHibernateUnitOfWork>().SingleInstance();

            container.Register(c => new NHibernateSessionFactory(c.Resolve<IAutomappingConfiguration>())).As<INHibernateSessionFactory>().SingleInstance();
            container.Register(c => c.Resolve<INHibernateSessionFactory>().CreateSession()).As<ISession>().InstancePerLifetimeScope();

            container.RegisterGeneric(typeof (NHibernateQueryService<>)).As(typeof (IQueryService<>));

            container.RegisterAssemblyTypes(typeof (NHibernateQueryRegion).Assembly)
                     .Where(t => t.Name.StartsWith("NHibernateQuery"))
                     .AsImplementedInterfaces();
        }
    }
}