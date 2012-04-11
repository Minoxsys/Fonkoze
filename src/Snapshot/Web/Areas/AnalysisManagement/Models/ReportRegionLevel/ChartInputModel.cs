using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.ReportRegionLevel
{
    public class ChartInputModel
    {
        public string RegionName { get; set; }
        public string ProductName { get; set; }
        public int StockLevelSum { get; set; }
    }
}