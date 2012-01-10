using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;

namespace Persistence.Queries.Products
{
    public interface IQueryProduct
    {
        IQueryable<Product> GetAll();
    }
}
