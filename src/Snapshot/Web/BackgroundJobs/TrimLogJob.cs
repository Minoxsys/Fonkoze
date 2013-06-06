using NHibernate;
using System;
using System.Threading.Tasks;
using WebBackgrounder;
using Web.BackgroundJobs.BackgroundJobsServices;
using Autofac.Features.Indexed;

namespace Web.BackgroundJobs
{
    public class TrimLogJob : IJob
    {
        IIndex<JobExecutionType, IJobExecutionService> _executionTypes;

        public TrimLogJob(IIndex<JobExecutionType, IJobExecutionService> executionTypes)
        {
            _executionTypes = executionTypes;
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
            get { return TimeSpan.FromMinutes(30); }
        }

        public Task Execute()
        {
            return new Task(() =>
                {
                    _executionTypes[JobExecutionType.TrimLogType].ExecuteJob();
                });
        }
    }
}