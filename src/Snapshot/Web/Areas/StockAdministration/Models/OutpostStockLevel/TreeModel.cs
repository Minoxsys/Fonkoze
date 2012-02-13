using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.OutpostStockLevel
{
    public class TreeModel
    {
        public String OutpostName { get; set; }
        public String ProductGroupName { get; set; }
        public String ProductName { get; set; }
        public String SMSCode { get; set; }
        public String LastUpdate { get; set; }
        public String UpdateMethod { get; set; }
        public String PreviuousLevel { get; set; }
        public TreeModel children { get; set; }
    }
}