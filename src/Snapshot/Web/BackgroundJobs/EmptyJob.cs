using System;
using System.Linq;
using System.Threading.Tasks;
using WebBackgrounder;
using Domain;
using Core.Persistence;

namespace Web.BackgroundJobs
{
    public class EmptyJob : IJob
    {
        const string EMPTY_JOB_NAME = "EmptyJob";
        private readonly Func<IQueryService<WorkItem>> queryWorkItems;
        private readonly Func<IDeleteCommand<WorkItem>> deleteWorkItems;

        public EmptyJob(Func<IQueryService<WorkItem>> queryWorkItems, Func<IDeleteCommand<WorkItem>> deleteWorkItems)
        {
            this.queryWorkItems = queryWorkItems;
            this.deleteWorkItems = deleteWorkItems;

        }
        public System.Threading.Tasks.Task Execute()
        {
            return new Task(() => {

                var cutoffDate = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(15));
                var oldItems = queryWorkItems().Query().Where(w => w.Completed != null && w.Completed < cutoffDate);
                if (oldItems.Any())
                {
                    foreach (var workItem in oldItems.ToList())
                    {
                        deleteWorkItems().Execute(workItem);
                    }
                }

            });
            
        }

        public TimeSpan Interval
        {
            get { return TimeSpan.FromSeconds(10); }
        }

        public string Name
        {
            get { return EMPTY_JOB_NAME; }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromMinutes(2); }
        }
    }
}