using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using FluentNHibernate.Automapping.Alterations;

namespace Persistence.Overrides
{
    public class RequestScheduleAutoMappingOverride : IAutoMappingOverride<RequestSchedule>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<RequestSchedule> mapping)
        {
            mapping.HasMany(r => r.Reminders).Cascade.All();
            mapping.Map(r => r.ScheduleName).Unique();
        }
    }
}
