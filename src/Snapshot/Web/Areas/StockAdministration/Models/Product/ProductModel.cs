using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.StockAdministration.Models.ProductGroup;

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
        public  ProductGroupModel ProductGroup { get; set; }
    }
}