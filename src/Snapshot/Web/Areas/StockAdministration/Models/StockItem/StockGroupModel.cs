using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.StockItem
{
    public class StockGroupModel
    {
        public Guid Id { get; set; }
        public  String Name { get; set; }
        public  String Description { get; set; }
    }
}