using System;
using System.Linq;
using Domain;
using FluentNHibernate.Automapping.Alterations;

namespace Persistence.Overrides
{
    public class WorkItemAutoMappingOverride : IAutoMappingOverride<WorkItem>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<WorkItem> mapping)
        {
            mapping.Map(p => p.WorkItemId).Generated.Always();
        }
    }
}
