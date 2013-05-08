using Domain;
using Infrastructure.Logging;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Linq;
using System.Transactions;
using WebBackgrounder;

namespace Persistence
{
    public class NHibernateWorkItemRepository : IWorkItemRepository
    {
        private readonly Func<ISession> _sessionThunk;
        private ISession _session;
        private readonly Func<ILogger> _logger;

        public NHibernateWorkItemRepository(Func<ISession> sessionThunk, Func<ILogger> logger)
        {
            _logger = logger;
            _sessionThunk = sessionThunk;
            _session = _sessionThunk();
        }

        public long CreateWorkItem(string workerId, IJob job)
        {
            try
            {
                var workItem = new WorkItem
                    {
                        JobName = job.Name,
                        WorkerId = workerId,
                        Started = DateTime.UtcNow,
                        Completed = null
                    };
                _session.Save(workItem);
                _session.Flush();

                return workItem.WorkItemId;
            }
            catch (Exception ex)
            {
                _logger().LogError(ex, "Create work item failed");
                throw;
            }
        }

        public IWorkItem GetLastWorkItem(IJob job)
        {
            try
            {
                var workItemRecord = (from w in _session.Query<WorkItem>()
                                      where w.JobName == job.Name
                                      orderby w.Started descending
                                      select w).FirstOrDefault();

                if (workItemRecord == null) return null;

                return new WorkItemAdapter(workItemRecord);
            }
            catch (Exception ex)
            {
                _logger().LogError(ex, "GetLastWorkItem failed");
                throw;
            }
        }

        public void RunInTransaction(Action query)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    query();
                    transaction.Complete();
                }

                _session.Dispose();
                _session = _sessionThunk();
            }
            catch (Exception ex)
            {
                _logger().LogError(ex, "RunInTransaction failed");
                throw;
            }
        }

        public void SetWorkItemCompleted(long workItemId)
        {
            try
            {
                var workItem = GetWorkItem(workItemId);
                workItem.Completed = DateTime.UtcNow;
                _session.Save(workItem);
                _session.Flush();
            }
            catch (Exception ex)
            {
                _logger().LogError(ex, "SetWorkItemCompleted failed");
                throw;
            }
        }

        public void SetWorkItemFailed(long workItemId, Exception exception)
        {
            try
            {
                var workItem = GetWorkItem(workItemId);
                workItem.Completed = DateTime.UtcNow;
                workItem.ExceptionInfo = exception.Message + Environment.NewLine + exception.StackTrace;

                _session.Save(workItem);
                _session.Flush();
            }
            catch (Exception ex)
            {
                _logger().LogError(ex, "SetWorkItemFailed failed");
                throw;
            }
        }

        private WorkItem GetWorkItem(long workerId)
        {
            try
            {
                return _session.Query<WorkItem>().Single(w => w.WorkItemId == workerId);
            }
            catch (Exception ex)
            {
                _logger().LogError(ex, "GetWorkItem failed");
                throw;
            }
        }

        public void Dispose()
        {
            _session.Dispose();
        }

        public class WorkItemAdapter : IWorkItem
        {
            private readonly WorkItem _item;

            public WorkItemAdapter(WorkItem item)
            {
                _item = item;
            }

            public DateTime? Completed
            {
                get { return _item.Completed; }
                set { _item.Completed = value; }
            }

            public long Id
            {
                get { return _item.WorkItemId; }
                set { }
            }

            public DateTime Started
            {
                get { return _item.Started; }
                set { }
            }
        }
    }
}
