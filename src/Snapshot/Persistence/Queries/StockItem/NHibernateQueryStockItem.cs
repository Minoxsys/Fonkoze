using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using NHibernate.Linq;

namespace Persistence.Queries.StockItems
{
    public class NHibernateQueryStockItem : IQueryStockItem
    {
        public IQueryService<StockItem> queryStockItem;

        public NHibernateQueryStockItem(IQueryService<StockItem> query)
        {
            this.queryStockItem = query;
        }
        public IQueryable<StockItem> GetAll()
        {
            return queryStockItem.Query().Fetch(it => it.StockGroup);
        }
    }
}
