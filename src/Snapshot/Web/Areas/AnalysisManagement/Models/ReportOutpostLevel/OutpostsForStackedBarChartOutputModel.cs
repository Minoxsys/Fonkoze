using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.ReportOutpostLevel
{
    public class OutpostsForStackedBarChartOutputModel
    {        
        public OutpostStackedBarChartModel [] Outposts { get; set; }
        public int TotalItems { get; set; }
       
    }
}