using System;
using System.Linq;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class OutpostOverviewModel
    {
        public Guid CountryId { get; set; }
        public Guid RegionId { get; set; }
        public Guid DistrictId { get; set; }
    }
}