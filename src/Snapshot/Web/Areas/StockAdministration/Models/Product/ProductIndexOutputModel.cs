using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.Product
{
    public class ProductIndexOutputModel
    {
        public List<ProductModel> products { get; set; }
        public int TotalItems { get; set; }

        public ProductIndexOutputModel()
        {
            this.products = new List<ProductModel>();
        }
    }
}