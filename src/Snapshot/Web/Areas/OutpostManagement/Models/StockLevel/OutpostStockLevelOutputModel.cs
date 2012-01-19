using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Web.Areas.OutpostManagement.Models.StockLevel
{
    public class OutpostStockLevelOutputModel
    {
        public Guid OutpostId { get; set; }
        public String OutpostName { get; set; }
        public Guid ProductGroupId { get; set; }
        public String ProductGroupName { get; set; }
        public List<SelectedProductlModel> Products { get; set; }
        public List<OutpostStockLevelModel> StockLevels { get; set; }
        public List<SelectListItem> ProductGroups { get; set; }

        public string Error { get; set; }

    }
}