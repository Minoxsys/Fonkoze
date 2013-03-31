using System;
using System.Collections.Generic;

namespace Web.Areas.OutpostManagement.Models.StockLevel
{
    public class OutpostStockLevelInputModel
    {
        public Guid OutpostId { get; set; }
        public Guid ProductGroupId { get; set; }
        public String ProductGroupName { get; set; }
        public SelectedProductlModel[] Products { get; set; }
        public OutpostStockLevelModel[] StockLevels { get; set; }

        public OutpostStockLevelInputModel()
        {
                //Products = new List<OutpostStockLevelModel>();
                //StockLevels = new List<OutpostStockLevelModel>();
        }
    }
}