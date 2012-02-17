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
        public virtual ScheduleFrequency Frequency { get; set; }
        public virtual List<RequestReminder> Reminders { get; set; }
        public virtual string ScheduleBasis { get; set; }
        public virtual Guid CampaignId { get; set; }
    }
}
