using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.ReportDistrictLevel
{
    public class ProductsChartModel
    {
        public string ProductName {get; set;}
        public string StockLevel { get; set; }
        public int LowerLimit { get; set; }
    }
}