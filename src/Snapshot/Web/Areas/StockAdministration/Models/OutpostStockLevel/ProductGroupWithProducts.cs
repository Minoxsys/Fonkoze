using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.StockAdministration.Models.Product;

namespace Web.Areas.StockAdministration.Models.OutpostStockLevel
{
    public class ProductGroupWithProducts
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ProductModel> StockItems { get; set; }

        public ProductGroupWithProducts()
        {
            this.StockItems = new List<ProductModel>();
        }
    }
}