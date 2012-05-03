using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Persistence;
using Persistence.Conventions;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.IO;

namespace IntegrationTests
{
    public static class SessionFactory
    {
        static NHibernateSessionFactory _sessionFactory;

        private static string _databaseFilename = "database.sdf";
        private static string _connectionstring = string.Format(@"DataSource=""{0}"";", _databaseFilename);

        public static NHibernateSessionFactory Instance
        {
            get
            {
                if (_sessionFactory == null)
                    _sessionFactory = new NHibernateSessionFactory(new DomainEntityMappingConvention(), ConnectWithSqlLite);

                return _sessionFactory;
            }
        }

     
        private static void ConnectWithMySql(FluentConfiguration config)
        {
            config.Database(
               MySQLConfiguration.
               Standard.
               ConnectionString(c => c.FromConnectionStringWithKey("MySqlDbConnection"))
                //.ShowSql()
                );

            config.ExposeConfiguration(cfg => new SchemaExport(cfg).Create(true, true));

        }

        private static void ConnectWithSqlLite(FluentConfiguration config)
        {
            config.Database(
                SQLiteConfiguration.Standard.UsingFile("database.db")
                .ShowSql()
                );
            config.ExposeConfiguration(cfg => new SchemaExport(cfg).Create(true, true));
        }
    }
}
