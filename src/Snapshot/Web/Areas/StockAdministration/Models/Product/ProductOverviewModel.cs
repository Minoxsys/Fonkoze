using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.Product
{
    public class ProductOverviewModel
    {
        public List<ProductModel> Products { get; set; }

        public ProductOverviewModel()
        {
            this.Products = new List<ProductModel>();
        }
    }
}