using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FluentNHibernate.Automapping.Alterations;
using Domain;

namespace Persistence.Overrides
{
    public class RegionAutoMappingOverride: IAutoMappingOverride<Region>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<Region> mapping)
        {
            mapping.References(p => p.Country).Not.LazyLoad();
            mapping.HasOne(p => p.Client).Cascade.SaveUpdate();
                   
           
        }
    }
}
