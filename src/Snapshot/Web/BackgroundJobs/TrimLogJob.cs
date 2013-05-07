using System;
using System.Threading.Tasks;
using NHibernate;
using WebBackgrounder;

namespace Web.BackgroundJobs
{
    public class TrimLogJob : IJob
    {
        private readonly Func<ISession> _sessionThunk;
        private readonly ISession _session;

        public TrimLogJob(Func<ISession> sessionThunk)
        {
            _sessionThunk = sessionThunk;
            _session = _sessionThunk();
        }

        public Task Execute()
        {
            return new Task(() =>
                {
                    var query = _session.CreateSQLQuery("exec TrimLogTable @cutoffdays=:days");
                    query.SetInt32("days", 30);
                    query.ExecuteUpdate();
                });
        }

        public TimeSpan Interval
        {
            //23 hours and 53 minutes
            get { return TimeSpan.FromMinutes(2); }
        }

        public string Name
        {
            get { return "TrimLogJob"; }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromMinutes(15); }
        }
    }
}