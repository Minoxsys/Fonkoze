using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.OutpostStockLevel
{
    public class OutpostList
    {
        
        public List<OutpostWithProductGroups> Outposts { get; set; }

        public OutpostList()
        {
            this.Outposts = new List<OutpostWithProductGroups>();
        }
    }
}