using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.ReportRegionLevel
{
    public class FilterInputModel
    {
        public Guid RegionId { get; set; }
        public Guid CountryId { get; set; }

    }
}