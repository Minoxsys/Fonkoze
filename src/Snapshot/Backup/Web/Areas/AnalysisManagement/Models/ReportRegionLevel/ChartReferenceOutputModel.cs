using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.ReportRegionLevel
{
    public class ChartReferenceOutputModel
    {
        public ChartInputModel[] Products { get; set; }
        public int TotalItems { get; set; }
    }
}