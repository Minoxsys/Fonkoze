using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.ReportDistrictLevel
{
    public class ProductsForChartOutputModel
    {
        public ProductsChartModel[] Products { get; set; }
        public int TotalItems { get; set; }
    }
}