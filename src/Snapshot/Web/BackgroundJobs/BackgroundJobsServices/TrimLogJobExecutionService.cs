using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using Persistence;

namespace Web.BackgroundJobs.BackgroundJobsServices
{
    public class TrimLogJobExecutionService: IJobExecutionService
    {
        private INHibernateSessionFactory _sessionFactory;
        private ISession _session;

        public TrimLogJobExecutionService(INHibernateSessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;

            _session = sessionFactory.CreateSession();
         }

        public void ExecuteJob()
        {
            var query = _session.CreateSQLQuery("exec TrimLogTable @cutoffdays=:days");
            query.SetInt32("days", 30);
            query.ExecuteUpdate();
            _session.Dispose();
        }
    }
}