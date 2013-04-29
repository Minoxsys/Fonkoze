using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.ReportOutpostLevel
{
    public class OutpostStackedBarChartModel
    {
        public string OutpostName { get; set; }
        public int ProductsUnderTresholdNo { get; set; }
        public int Total { get; set; }
        public List<ProductStackedBarChartModel> Products  { get; set; }
       
        public OutpostStackedBarChartModel()
        {
            this.Products = new List<ProductStackedBarChartModel>();
        }
    }
}