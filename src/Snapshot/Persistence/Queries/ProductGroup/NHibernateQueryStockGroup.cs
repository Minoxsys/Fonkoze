using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using NHibernate.Linq;

namespace Persistence.Queries.StockItems
{
    public class NHibernateQueryStockGroup : IQueryStockGroup
    {
        public IQueryService<ProductGroup> queryStockGroup;

        public NHibernateQueryStockGroup(IQueryService<ProductGroup> query)
        {
            this.queryStockGroup = query;
        }
    }
}
