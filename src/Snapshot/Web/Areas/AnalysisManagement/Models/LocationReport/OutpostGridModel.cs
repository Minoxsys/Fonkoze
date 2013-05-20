using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.LocationReport
{
    public class OutpostGridModel
    {
        public string OutpostName { get; set; }
        public int Total { get; set; }
        public List<ProductGridModel> Products { get; set; }
        public OutpostGridModel()
        {
            this.Products = new List<ProductGridModel>();
        }
    }
}