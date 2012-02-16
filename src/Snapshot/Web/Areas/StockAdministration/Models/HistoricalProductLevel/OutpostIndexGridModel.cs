using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.HistoricalProductLevel
{
    public class OutpostIndexGridModel
    {
        public OutpostGridModel[] Historical { get; set; }
        public int TotalItems { get; set; }
    }
}