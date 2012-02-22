using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class Schedule : DomainEntity
    {
        public virtual string Name { get; set; }
        public virtual string FrequencyType { get; set; }
        public virtual int FrequencyValue { get; set; }
        public virtual int StartOn { get; set; }
        public virtual IList<RequestReminder> Reminders { get; set; }
        public virtual string ScheduleBasis { get; set; }
        public virtual Client Client { get; set; }

        public virtual void AddReminder(RequestReminder reminder)
        {
            Reminders.Add(reminder);
            reminder.Schedule = this;
        }
    }
}
