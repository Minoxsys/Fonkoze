using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class RequestReminder : DomainEntity
    {
        public virtual string PeriodType { get; set; }
        public virtual int PeriodValue { get; set; }
        public virtual Schedule Schedule { get; set; }
    }
}
