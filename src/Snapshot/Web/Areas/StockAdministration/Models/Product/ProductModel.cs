using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.Product
{
    public class ProductModel
    {
        public  Guid Id { get; set; }
        public  String Name { get; set; }
        public  String Description { get; set; }
        public  int LowerLimit { get; set; }
        public  int UpperLimit { get; set; }
        public  String SMSReferenceCode { get; set; }
        public  String ProductGroupName { get; set; }
        public  String LastUpdateAt { get; set; }
        public  string UpdateMethod { get; set; }
        public  int PreviousStockLevel { get; set; }
        public  int StockLevel { get; set; }
        public Guid OutpostStockLevelId { get; set; }
    }
}