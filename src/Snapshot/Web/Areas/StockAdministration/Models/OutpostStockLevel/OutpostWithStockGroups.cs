using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.OutpostStockLevel
{
    public class OutpostWithProductGroups
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ProductGroupWithProducts> StockGroups { get; set; }

        public OutpostWithProductGroups()
        {
            this.StockGroups = new List<ProductGroupWithProducts>();
        }
    }
}