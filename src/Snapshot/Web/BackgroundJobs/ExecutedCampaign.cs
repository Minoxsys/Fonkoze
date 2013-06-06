using Core.Persistence;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Services;
using WebBackgrounder;
using Web.BackgroundJobs.BackgroundJobsServices;
using Autofac.Features.Indexed;

namespace Web.BackgroundJobs
{
    public class CampaignExecutionJob : IJob
    {
        private const string JobName = "CampaignExecutionJob";

        IIndex<JobExecutionType, IJobExecutionService> _executionTypes;

        public CampaignExecutionJob(IIndex<JobExecutionType, IJobExecutionService> executionTypes)
        {
            _executionTypes = executionTypes;
        }

        public TimeSpan Interval
        {
            //3 hours 43 minutes
            get { return TimeSpan.FromMinutes(223); }
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
                    _executionTypes[JobExecutionType.CampaignJobType].ExecuteJob();
                });
        }
    }
}