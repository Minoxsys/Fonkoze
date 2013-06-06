using Core.Persistence;
using Domain;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.LocalizationResources;
using Web.Services;
using Web.Services.Configuration;
using Web.Services.SendEmail;
using WebBackgrounder;
using Web.BackgroundJobs.BackgroundJobsServices;
using Autofac.Features.Indexed;

namespace Web.BackgroundJobs
{
    public class OutpostInactivityJob : IJob
    {
        private const string JobName = "OutpostInactivityJob";

        IIndex<JobExecutionType, IJobExecutionService> _executionTypes;

        public OutpostInactivityJob(IIndex<JobExecutionType, IJobExecutionService> executionTypes)
        {
            _executionTypes = executionTypes;
        }

        public TimeSpan Interval
        {
            //23 hours 15 minutes
            get { return TimeSpan.FromMinutes(1395); }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromMinutes(30); }
        }

        public string Name
        {
            get { return JobName; }
        }
        public Task Execute()
        {
            return new Task(() =>
            {
                _executionTypes[JobExecutionType.OutpostInactivityType].ExecuteJob();
            });
        }
    }
}