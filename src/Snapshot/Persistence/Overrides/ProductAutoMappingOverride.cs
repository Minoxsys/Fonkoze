using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Domain;

namespace Persistence.Overrides
{
    public class ProductAutoMappingOverride : IAutoMappingOverride<Product>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<Product> mapping)
        {
            mapping.References(p => p.ProductGroup).Not.LazyLoad();
            mapping.References(p => p.ProductGroup).Cascade.SaveUpdate();    


        }
    }
}
