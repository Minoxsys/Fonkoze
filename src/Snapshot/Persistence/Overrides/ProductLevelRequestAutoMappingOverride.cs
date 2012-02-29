using System;
using System.Linq;
using Domain;
using FluentNHibernate.Automapping.Alterations;

namespace Persistence.Overrides
{
    public class ProductLevelRequestAutoMappingOverride : IAutoMappingOverride<ProductLevelRequest>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<ProductLevelRequest> mapping)
        {
            mapping.References(p => p.Campaign).Fetch.Join();
            mapping.References(p => p.ProductGroup).Fetch.Join();
            mapping.References(p => p.Schedule).Fetch.Join();
        }
    }
}
