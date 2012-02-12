using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cache;
using FluentNHibernate.Automapping;
using Core.Domain;
using Persistence.Conventions;
using System.IO;
using NHibernate.Tool.hbm2ddl;
using Domain;


namespace Persistence
{
    public class NHibernateSessionFactory : INHibernateSessionFactory
    {
        private readonly ISessionFactory sessionFactory;
        private readonly IAutomappingConfiguration mappingConfiguration;

        private readonly Action<FluentConfiguration> databaseCreation;

        public NHibernateSessionFactory(IAutomappingConfiguration mappingConfiguration)
            : this(mappingConfiguration, ConnectWithSqlServer2008)
        {

        }

        public NHibernateSessionFactory(IAutomappingConfiguration mappingConfiguration, Action<FluentConfiguration> databaseCreation)
        {
            this.mappingConfiguration = mappingConfiguration;
            this.databaseCreation = databaseCreation;
            sessionFactory = CreateSessionFactory();
        }

        private static void ConnectWithSqlServer2008(FluentConfiguration config)
        {
            config.Database(

                MsSqlConfiguration.MsSql2008
                    .ConnectionString(c => c.FromConnectionStringWithKey("DbConnection"))
					.AdoNetBatchSize(250)
                    .ShowSql()
                    )
                    .Cache(c =>
                    c.UseQueryCache().ProviderClass<HashtableCacheProvider>());

        }
        
        private static void ConnectWithMySql(FluentConfiguration config)
        {
            config.Database(
               MySQLConfiguration.
               Standard.
               ConnectionString(c => c.FromConnectionStringWithKey("MySqlDbConnection"))
               //.ShowSql()
                );

            //config.ExposeConfiguration(cfg => new SchemaExport(cfg).Create(true, true));

        }



        public ISession CreateSession()
        {
            return sessionFactory.OpenSession();
        }

        private ISessionFactory CreateSessionFactory()
        {

            var config = Fluently.Configure();

            databaseCreation(config);

            config.Mappings(m =>

                m.AutoMappings.Add(
                    AutoMap
                    .AssemblyOf<Role>(mappingConfiguration)
                    .AddEntityAssembly(	typeof(Client).Assembly)
                    .UseOverridesFromAssembly(this.GetType().Assembly)
                    .IgnoreBase<DomainEntity>()
                    .Conventions.Add<TableNamingConvention>()
                    .Conventions.Add<ErmForeignKeyConvention>()
                    .Conventions.Add<ManyToManyConvention>()

                )
            );

            var factory = config.BuildSessionFactory();
            return factory;

        }


    }
}
