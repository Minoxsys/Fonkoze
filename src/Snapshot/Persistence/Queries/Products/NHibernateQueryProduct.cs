using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using NHibernate.Linq;

namespace Persistence.Queries.Products
{
    public class NHibernateQueryProduct : IQueryProduct
    {
        public IQueryService<Product> queryProduct;

        public NHibernateQueryProduct(IQueryService<Product> query)
        {
            this.queryProduct = query;
        }
        public IQueryable<Product> GetAll()
        {
            return queryProduct.Query().Fetch(it => it.ProductGroup);
        }
    }
}
