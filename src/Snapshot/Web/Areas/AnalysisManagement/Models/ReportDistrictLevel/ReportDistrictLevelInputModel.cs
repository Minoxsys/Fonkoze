using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.ReportDistrictLevel
{
    public class ReportDistrictLevelInputModel
    {
        public Guid CountryId { get; set; }
        public Guid RegionId { get; set; }
        public Guid DistrictId { get; set; }
    }
}