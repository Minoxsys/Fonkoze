using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.District
{
    public class FromRegionModel
    {
        public Guid CountryId { get; set; }
        public Guid RegionId { get; set; }
    }
}