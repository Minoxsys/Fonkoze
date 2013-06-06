using Core.Persistence;
using Domain;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.LocalizationResources;
using Web.Services;
using Web.Services.Helper;
using WebBackgrounder;
using Web.BackgroundJobs.BackgroundJobsServices;
using Autofac.Features.Indexed;

namespace Web.BackgroundJobs
{
    public class SmsMessagesMonitoringJob : IJob
    {
        private const string JobName = "SmsMessagesMonitoringJob";

        IIndex<JobExecutionType, IJobExecutionService> _executionTypes;

        public TimeSpan Interval
        {
            //2 hour 1 minute
            get { return TimeSpan.FromMinutes(121); }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromMinutes(20); }
        }

        public string Name
        {
            get { return JobName; }
        }

        public SmsMessagesMonitoringJob(    IIndex<JobExecutionType, IJobExecutionService> executionTypes)
        {
            _executionTypes = executionTypes;
        }

        public Task Execute()
        {
            return new Task(() =>
                {
                    _executionTypes[JobExecutionType.SmsMessageMonitoringType].ExecuteJob();
                });
        }
    }
}