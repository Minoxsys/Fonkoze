using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.StockAdministration.Models.Product;

namespace Web.Areas.StockAdministration.Models.OutpostStockLevel
{
    public class ProductGroupOverviewModel
    {

        public Guid CountryId { get; set; }
        public Guid RegionId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid OutpostId { get; set; }

        public Guid ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public string OutpostName { get; set; }
        public bool AreCommingFromFilterByAll { get; set; }
        public List<ProductModel> Products { get; set; }

        public ProductGroupOverviewModel()
        {
            this.Products = new List<ProductModel>();
        }
    }
}