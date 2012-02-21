using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using FluentNHibernate.Automapping.Alterations;

namespace Persistence.Overrides
{
    public class ScheduleAutoMappingOverride : IAutoMappingOverride<Schedule>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<Schedule> mapping)
        {
            mapping.HasMany(r => r.Reminders).Cascade.All().Inverse();
            mapping.Map(r => r.Name).Unique();
        }
    }
}
