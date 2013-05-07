using System;
using System.Threading.Tasks;
using NHibernate;
using WebBackgrounder;

namespace Web.BackgroundJobs
{
    public class TrimLogJob : IJob
    {
        private readonly Func<ISession> _sessionThunk;
        private ISession _session;

        public TrimLogJob(Func<ISession> sessionThunk)
        {
            _sessionThunk = sessionThunk;
            _session = _sessionThunk();
        }

        public Task Execute()
        {
            return new Task(() =>
                {
                    _session = _sessionThunk();
                    var query = _session.CreateSQLQuery("exec TrimLogTable @cutoffdays=:days");
                    query.SetInt32("days", 30);
                    query.ExecuteUpdate();
                    _session.Dispose();
                });
        }

        public TimeSpan Interval
        {
            //23 hours and 3 minutes
            get { return TimeSpan.FromMinutes(1383); }
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