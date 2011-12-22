using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.StockItem
{
    public class StockItemOverviewModel
    {
        public List<StockItemModel> StockItems { get; set; }

        public StockItemOverviewModel()
        {
            this.StockItems = new List<StockItemModel>();
        }
    }
}