using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.OutpostStockLevel
{
    public class OutpostStockLevelInputModel
    {
        public Guid? Id { get; set; }       
        public int StockLevel { get; set; }      
       
    }
}