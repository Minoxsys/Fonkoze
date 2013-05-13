using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.HistoricalProductLevel
{
    public class ProductSaleChartModel
    {
        public string Day { get; set; }
        public int Quantity { get; set; }
        public string Date { get; set; }
    }
}