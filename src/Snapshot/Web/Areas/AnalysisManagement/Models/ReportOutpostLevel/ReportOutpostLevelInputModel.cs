using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.ReportOutpostLevel
{
    public class ReportOutpostLevelInputModel
    {
        public Guid CountryId { get; set; }
        public Guid RegionId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid OutpostId { get; set; }
        public bool OnlyUnderTreshold { get; set; }
    }
}