using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.StockAdministration.Models.HistoricalProductLevel
{
    public class ProductSaleModel
    {
        public string Country { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string OutpostName { get; set; }
        public string ProductName { get; set; }
        public string Date { get; set; }
        public int Quantity { get; set; }
    }
}
