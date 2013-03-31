using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.HistoricalProductLevel
{
    public class ProductGroupLevelModel
    {
        public Guid Id { get; set; }
        public Guid OutpostId { get; set; }
        public string OutpostName { get; set; }
        public Guid ProductGroupId { get; set; }
        public string ProductGroupName { get; set;}
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string SMSReferenceCode { get; set; }
        public int ProductStockLevel { get; set; }
        public string LastUpdated { get; set; }
        public string UpdateMethod { get; set; }
        public string Description { get; set; }

    }
}