using System;

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
        public int ProductLimit { get; set; }
        public string DistrictManagerPhoneNumber { get; set; }
    }
}