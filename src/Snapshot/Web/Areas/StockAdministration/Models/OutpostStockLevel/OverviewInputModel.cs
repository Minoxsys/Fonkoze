using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.OutpostStockLevel
{
    public class OverviewInputModel
    {
        public Guid OutpostId { get; set; }
        public Guid DistrictId { get; set; }
        public String ProductGroupExpandedOnEdit { get; set; }
               
    }
}