using Core.Persistence;
using Domain;
using Domain.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.LocalizationResources;
using Web.Models.Alerts;
using Web.Services;
using Web.Services.SendEmail;
using WebBackgrounder;
using Web.BackgroundJobs.BackgroundJobsServices;
using Autofac.Features.Indexed;

namespace Web.BackgroundJobs
{
    public class StockLevelsMonitoringJob : IJob
    {
        private const string JobName = "StockLevelsMonitoringJob";

        IIndex<JobExecutionType, IJobExecutionService> _executionTypes;

        public TimeSpan Interval
        {
            //1 hour 32 minutes
            get { return TimeSpan.FromMinutes(92); }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromMinutes(30); }
        }

        public string Name
        {
            get { return JobName; }
        }

        public StockLevelsMonitoringJob(   IIndex<JobExecutionType, IJobExecutionService> executionTypes)
        {
            _executionTypes = executionTypes;
        }

        public Task Execute()
        {
            return new Task(() =>
                {
                    _executionTypes[JobExecutionType.StockLevelMonitoringType].ExecuteJob();
                });
        }
    }
}