using System;
using System.Threading.Tasks;
namespace Web.BackgroundJobs.BackgroundJobsServices
{
    public interface IJobExecutionService
    {
        void ExecuteJob();
    }
}
