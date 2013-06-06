using Core.Persistence;
using Domain;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebBackgrounder;
using Web.BackgroundJobs.BackgroundJobsServices;
using Autofac.Features.Indexed;

namespace Web.BackgroundJobs
{
    public class EmptyJob : IJob
    {
        private const string EmptyJobName = "EmptyJob";

        IIndex<JobExecutionType, IJobExecutionService> _executionTypes;

        public EmptyJob(IIndex<JobExecutionType, IJobExecutionService> executionTypes)
        {
            _executionTypes = executionTypes;
        }

        public TimeSpan Interval
        {
            //20 hours
            get { return TimeSpan.FromMinutes(1200); }
        }

        public string Name
        {
            get { return EmptyJobName; }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromMinutes(15); }
        }

        public Task Execute()
        {
            return new Task(() =>
            {
                _executionTypes[JobExecutionType.EmptyJobType].ExecuteJob();
            });
        }
    }
}