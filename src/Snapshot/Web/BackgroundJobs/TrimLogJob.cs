
using Infrastructure.Logging;
using NHibernate;
using System;
using System.Threading.Tasks;
using WebBackgrounder;

namespace Web.BackgroundJobs
{
    public class TrimLogJob : IJob
    {
        private readonly Func<ISession> _sessionThunk;
        private ISession _session;
        private readonly Func<ILogger> _logger;

        public TrimLogJob(Func<ISession> sessionThunk, Func<ILogger> logger)
        {
            _logger = logger;
            _sessionThunk = sessionThunk;
            _session = _sessionThunk();
        }

        public Task Execute()
        {
            return new Task(() =>
                {
                    try
                    {
                        _session = _sessionThunk();
                        var query = _session.CreateSQLQuery("exec TrimLogTable @cutoffdays=:days");
                        query.SetInt32("days", 30);
                        query.ExecuteUpdate();
                        _session.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger().LogError(ex, "Trim job has failed");
                        throw;
                    }
                });
        }

        public TimeSpan Interval
        {
            //23 hours and 3 minutes
            get { return TimeSpan.FromMinutes(2); }
        }

        public string Name
        {
            get { return "TrimLogJob"; }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromMinutes(1); }
        }
    }
}