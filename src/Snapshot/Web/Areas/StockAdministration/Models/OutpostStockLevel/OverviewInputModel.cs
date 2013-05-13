using System;

namespace Web.Areas.StockAdministration.Models.OutpostStockLevel
{
    public class OverviewInputModel
    {
        public Guid OutpostId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid RegionId { get; set; }
        public Guid CountryId { get; set; }
        public String ProductGroupExpandedOnEdit { get; set; }
    }
}