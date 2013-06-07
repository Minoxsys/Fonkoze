using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.HistoricalProductLevel
{
    public class ProductSalesFilterModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? RegionId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? OutpostId { get; set; }
        public Guid? ProductId { get; set; }
        public string Client { get; set; }

    }
}