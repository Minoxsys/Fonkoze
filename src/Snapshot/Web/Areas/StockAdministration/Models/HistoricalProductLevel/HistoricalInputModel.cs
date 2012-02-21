using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.HistoricalProductLevel
{
    public class HistoricalInputModel
    {
        public Guid Id { get; set; }
        public int StockLevel { get; set; }
    }
}