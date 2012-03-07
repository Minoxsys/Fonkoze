using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Core.Domain;
using Domain;

namespace Persistence.Overrides
{
    public class OutpostAutoMappingOverride : IAutoMappingOverride<Outpost>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<Outpost> mapping)
        {
            mapping.HasMany(p => p.Contacts);
            
        }
    }
}
