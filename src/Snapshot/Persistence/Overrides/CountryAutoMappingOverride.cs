using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using FluentNHibernate.Automapping.Alterations;

namespace Persistence.Overrides
{
    public class CountryAutoMappingOverride : IAutoMappingOverride<Country>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<Country> mapping)
        {
            mapping.References(p => p.Client).Nullable();
            mapping.HasMany(p => p.Regions);
        }
    }
}
