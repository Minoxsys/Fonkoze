using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.ProductGroup
{
    public class ProductGroupIndexOutputModel
    {
        public ProductGroupModel[] ProductGroups { get; set; }
        public int TotalItems { get; set; }
    }
}