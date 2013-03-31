using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Models.UserManager;

namespace Web.Areas.AnalysisManagement.Models.ReportRegionLevel
{
    public class ProductsReferenceOutputModel
    {
        public ReferenceModel[] Products { get; set; }
        public int TotalItems { get; set; }
    }
}