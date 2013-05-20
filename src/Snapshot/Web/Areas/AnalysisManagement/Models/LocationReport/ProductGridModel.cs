using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Web.Areas.AnalysisManagement.Models.LocationReport
{
    public class ProductGridModel
    {
        public string ProductName { get; set; }
        public int StockLevel { get; set; }
        public int LowerLimit { get; set; }

    }
}
