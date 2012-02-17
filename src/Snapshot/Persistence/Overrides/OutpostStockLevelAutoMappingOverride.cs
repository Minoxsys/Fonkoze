using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using FluentNHibernate.Automapping.Alterations;

namespace Persistence.Overrides
{
    public class OutpostStockLevelAutoMappingOverride : IAutoMappingOverride<OutpostStockLevel>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<OutpostStockLevel> mapping)
        {
            mapping.Not.LazyLoad();
            mapping.References(p => p.Outpost).Fetch.Join();
            mapping.References(p => p.ProductGroup).Fetch.Join();
            mapping.References(p => p.Product).Fetch.Join();
        }
    }
}
