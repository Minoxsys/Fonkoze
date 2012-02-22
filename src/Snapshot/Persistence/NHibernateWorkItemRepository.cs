using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBackgrounder;
using NHibernate;
using NHibernate.Linq;
using Domain;
using System.Transactions;

namespace Persistence
{
    public class NHibernateWorkItemRepository :IWorkItemRepository
    {
        private Func<ISession> _sessionThunk;
        private ISession _session;

        public NHibernateWorkItemRepository(Func<ISession> sessionThunk)
        {
            this._sessionThunk = sessionThunk;
            this._session = _sessionThunk();

        }
        public long CreateWorkItem(string workerId, IJob job)
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

        public IWorkItem GetLastWorkItem(IJob job)
        {
            var workItemRecord = (from w in _session.Query<WorkItem>()
                    where w.JobName == job.Name
                    orderby w.Started descending
                    select w).FirstOrDefault();

            if (workItemRecord == null) return null;

            return new WorkItemAdapter(workItemRecord);
        }

        public void RunInTransaction(Action query)
        {
            using (var transaction = new TransactionScope())
            {
                query();
                transaction.Complete();
            }

            _session.Dispose();
            _session = _sessionThunk();

        }

        public void SetWorkItemCompleted(long workItemId)
        {
            var workItem = GetWorkItem(workItemId);
            workItem.Completed = DateTime.UtcNow;
            _session.Save(workItem);
            _session.Flush();
        }

        public void SetWorkItemFailed(long workItemId, Exception exception)
        {
            var workItem = GetWorkItem(workItemId);
            workItem.Completed = DateTime.UtcNow;
            workItem.ExceptionInfo = exception.Message + Environment.NewLine + exception.StackTrace;

            _session.Save(workItem);
            _session.Flush();
        }

        private WorkItem GetWorkItem(long workerId)
        {
            return _session.Query<WorkItem>().Single(w => w.WorkItemId == workerId);
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
                get
                {
                    return _item.Completed;
                }
                set
                {
                    _item.Completed = value;
                }
            }

            public long Id
            {
                get
                {
                    return _item.WorkItemId;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public DateTime Started
            {
                get
                {
                    return _item.Started;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
