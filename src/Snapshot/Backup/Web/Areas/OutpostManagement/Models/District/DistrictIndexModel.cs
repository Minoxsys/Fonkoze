using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.District
{
    public class DistrictIndexModel
    {
        public Guid? RegionId { get; set; }
        public Guid? CountryId { get; set; }

        public int? Page { get; set; }
        public int? Start { get; set; }
        public int? Limit { get; set; }

        public string sort { get; set; }
        public string dir { get; set; }

        public String SearchName { get; set; }

    }
}