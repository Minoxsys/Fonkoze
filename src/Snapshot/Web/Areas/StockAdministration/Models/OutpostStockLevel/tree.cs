using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.OutpostStockLevel
{
    public class tree
    {
        public String text { get; set; }
        public String duration { get; set; }
        public String user { get; set; }
        public bool expanded { get; set; }
        public bool leaf { get; set; }
        public tree[] children { get; set; }
        

    }
}