using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.AnalysisManagement.Models.ReportOutpostLevel;

namespace Web.Areas.AnalysisManagement.Models.ReportRegionLevel
{
    public class RegionStackedBarChartModel
    {
        public string RegionName { get; set; }
        public int ProductsUnderTresholdNo { get; set; }
        public int Total { get; set; }
        public List<ProductStackedBarChartModel> Products { get; set; }

        public RegionStackedBarChartModel()
        {
            this.Products = new List<ProductStackedBarChartModel>();
        }
    }
}