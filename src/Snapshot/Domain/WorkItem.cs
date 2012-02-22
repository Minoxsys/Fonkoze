using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class WorkItem : DomainEntity
    {
        public virtual long WorkItemId { get; set; }
        public virtual string JobName { get; set; }
        public virtual string WorkerId { get; set; }
        public virtual DateTime Started { get; set; }
        public virtual DateTime? Completed { get; set; }
        public virtual string ExceptionInfo { get; set; }
    }
}
