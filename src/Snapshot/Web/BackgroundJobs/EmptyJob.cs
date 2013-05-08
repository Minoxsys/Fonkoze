using Core.Persistence;
using Domain;
using Infrastructure.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebBackgrounder;

namespace Web.BackgroundJobs
{
    public class EmptyJob : IJob
    {
        private const string EmptyJobName = "EmptyJob";
        private readonly Func<IQueryService<WorkItem>> _queryWorkItems;
        private readonly Func<IDeleteCommand<WorkItem>> _deleteWorkItems;
        private readonly Func<ILogger> _logger;

        public EmptyJob(Func<IQueryService<WorkItem>> queryWorkItems, Func<IDeleteCommand<WorkItem>> deleteWorkItems, Func<ILogger> logger)
        {
            _logger = logger;
            _queryWorkItems = queryWorkItems;
            _deleteWorkItems = deleteWorkItems;
        }

        public Task Execute()
        {
            return new Task(() =>
                {
                    try
                    {
                        var cutoffDate = DateTime.UtcNow.Subtract(TimeSpan.FromHours(4));
                        var oldItems = _queryWorkItems().Query().Where(w => w.Completed != null && w.Completed < cutoffDate).ToList();
                        if (!oldItems.Any()) 
                            return;
                        
                        foreach (var workItem in oldItems)
                        {
                            _deleteWorkItems().Execute(workItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger().LogError(ex, "EmptyJpob has failed");
                        throw;
                    }
                });
        }

        public TimeSpan Interval
        {
            //23 hours and 53 minutes
            get { return TimeSpan.FromMinutes(30); }
        }

        public string Name
        {
            get { return EmptyJobName; }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromMinutes(15); }
        }
    }
}