using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Alerts
{
    public class AlertOutputModel
    {
        public Guid OutpostId { get; set; }
        public string OutpostName { get; set; }
        public string Contact { get; set; }
        public Guid ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public Guid OutpostStockLevelId { get; set; }
        public int StockLevel { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string RefCode { get; set; }
        public Guid ClientId { get; set; }
    }
}