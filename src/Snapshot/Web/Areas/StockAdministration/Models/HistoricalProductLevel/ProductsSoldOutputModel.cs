using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.HistoricalProductLevel
{
    public class ProductsSoldOutputModel
    {
        public ProductSaleModel[] ProductSales { get; set; }
        public int TotalItems { get; set; }
    }
}