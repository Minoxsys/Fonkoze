using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.HistoricalProductLevel
{
    public class OutpostGridModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ProductGroup { get; set; }
        public string SMSResponseDate { get; set; }
        public int NumberOfProducts { get; set; }
    }
}