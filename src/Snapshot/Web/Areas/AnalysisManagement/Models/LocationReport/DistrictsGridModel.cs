using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.LocationReport
{
    public class DistrictsGridModel
    {
        public string DistrictName { get; set; }
        public int TotalOutposts { get; set; }
        public int GreenOutposts { get; set; }
        public int RedOutposts { get; set; }
    }
}