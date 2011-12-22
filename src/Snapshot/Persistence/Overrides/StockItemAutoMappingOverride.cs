using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Domain;

namespace Persistence.Overrides
{
    public class StockItemAutoMappingOverride : IAutoMappingOverride<StockItem>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<StockItem> mapping)
        {
            mapping.References(p => p.StockGroup).Not.LazyLoad();
            mapping.HasMany(it => it.Outposts).Cascade.SaveUpdate();
           


        }
    }
}
