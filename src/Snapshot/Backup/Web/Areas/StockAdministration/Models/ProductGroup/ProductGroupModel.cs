using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.ProductGroup
{
    public class ProductGroupModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ReferenceCode { get; set; }
        public int ProductsNo { get; set; }
        public Guid Id { get; set; }
        public string Error { get; set; }
    }
}