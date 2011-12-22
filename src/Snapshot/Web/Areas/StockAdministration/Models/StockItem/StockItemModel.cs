using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.StockItem
{
    public class StockItemModel
    {
        public  Guid Id { get; set; }
        public  String Name { get; set; }
        public  String Description { get; set; }
        public  int LowerLimit { get; set; }
        public  int UpperLimit { get; set; }
        public  String SMSReferenceCode { get; set; }
        public  StockGroupModel StockGroup { get; set; }
    }
}