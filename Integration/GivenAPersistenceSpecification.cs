using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;
using Core.Persistence;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Testing;
using NHibernate;
using Persistence;
using NUnit.Framework;
using Persistence.Conventions;
using FluentNHibernate.Cfg;
using System.IO;
using System.Data.SqlServerCe;
using NHibernate.Tool.hbm2ddl;

namespace IntegrationTests
{
    public class GivenAPersistenceSpecification<ENTITY>
        where ENTITY : DomainEntity
    {
        
        protected ISession session;
        protected NHibernateSessionFactory _sessionFactory;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            _sessionFactory = SessionFactory.Instance;
            session = _sessionFactory.CreateSession();
        }
             

        public PersistenceSpecification<ENTITY> Specs
        {
            get
            {
                return BuildPersistenceSpec();
            }
        }

        public virtual PersistenceSpecification<ENTITY> BuildPersistenceSpec()
        {
            return new PersistenceSpecification<ENTITY>(session);
        }
    }
}
