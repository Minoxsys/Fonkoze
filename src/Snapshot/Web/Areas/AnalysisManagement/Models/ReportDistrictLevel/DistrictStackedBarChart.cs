using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.AnalysisManagement.Models.ReportOutpostLevel;

namespace Web.Areas.AnalysisManagement.Models.ReportDistrictLevel
{
    public class DistrictStackedBarChartModel
    {
        public string DistrictName { get; set; }
        public int ProductsUnderTresholdNo { get; set; }
        public int Total { get; set; }
        public List<ProductStackedBarChartModel> Products { get; set; }

        public DistrictStackedBarChartModel()
        {
            this.Products = new List<ProductStackedBarChartModel>();
        }
    }
}