using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.Product
{
    public class ProductGroupModel
    {
        public Guid Id { get; set; }
        public  String Name { get; set; }
        public  String Description { get; set; }
    }
}