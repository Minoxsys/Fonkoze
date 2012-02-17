using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class RequestSchedule : DomainEntity
    {
        public virtual string ScheduleName { get; set; }
        public virtual string FrequencyType { get; set; }
        public virtual int FrequencyValue { get; set; }
        public virtual int StartOn { get; set; }
        public virtual List<RequestReminder> Reminders { get; set; }
        public virtual string ScheduleBasis { get; set; }
    }
}
