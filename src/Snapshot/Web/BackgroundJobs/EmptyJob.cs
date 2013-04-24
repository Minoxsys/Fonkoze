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
        private const string EmptyJobName = "EmptyJob";
        private readonly Func<IQueryService<WorkItem>> _queryWorkItems;
        private readonly Func<IDeleteCommand<WorkItem>> _deleteWorkItems;

        public EmptyJob(Func<IQueryService<WorkItem>> queryWorkItems, Func<IDeleteCommand<WorkItem>> deleteWorkItems)
        {
            _queryWorkItems = queryWorkItems;
            _deleteWorkItems = deleteWorkItems;
        }

        public Task Execute()
        {
            return new Task(() =>
                {
                    var cutoffDate = DateTime.UtcNow.Subtract(TimeSpan.FromHours(4));
                    var oldItems = _queryWorkItems().Query().Where(w => w.Completed != null && w.Completed < cutoffDate).ToList();
                    if (oldItems.Any())
                    {
                        foreach (var workItem in oldItems)
                        {
                            _deleteWorkItems().Execute(workItem);
                        }
                    }
                });
        }

        public TimeSpan Interval
        {
            //23 hours and 53 minutes
            get { return TimeSpan.FromMinutes(1433); }
        }

        public string Name
        {
            get { return EmptyJobName; }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromMinutes(10); }
        }
    }
}