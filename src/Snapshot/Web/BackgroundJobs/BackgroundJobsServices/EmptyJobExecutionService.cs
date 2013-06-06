using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Core.Persistence;
using Domain;

namespace Web.BackgroundJobs.BackgroundJobsServices
{
    public class EmptyJobExecutionService : IJobExecutionService
    {
        private readonly IQueryService<WorkItem> _queryWorkItems;
        private readonly IDeleteCommand<WorkItem> _deleteWorkItems;

        public EmptyJobExecutionService(IQueryService<WorkItem> queryWorkItems, IDeleteCommand<WorkItem> deleteWorkItems)
        {
            _queryWorkItems = queryWorkItems;
            _deleteWorkItems = deleteWorkItems;
        }

        public void ExecuteJob()
        {
            var cutoffDate = DateTime.UtcNow.Subtract(TimeSpan.FromHours(4));
            var oldItems = _queryWorkItems.Query().Where(w => w.Completed != null && w.Completed < cutoffDate).ToList();
            if (!oldItems.Any())
                return;

            foreach (var workItem in oldItems)
            {
                _deleteWorkItems.Execute(workItem);
            }
        }
    }
}