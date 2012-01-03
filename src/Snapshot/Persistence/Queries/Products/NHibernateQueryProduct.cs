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
        public IQueryService<Domain.Product> queryProduct;

        public NHibernateQueryProduct(IQueryService<Domain.Product> query)
        {
            this.queryProduct = query;
        }
    }
}
