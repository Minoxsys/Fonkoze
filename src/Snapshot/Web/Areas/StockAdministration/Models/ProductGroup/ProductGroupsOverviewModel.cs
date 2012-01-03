using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Domain;
using Domain;

namespace Web.Areas.StockAdministration.Models.ProductGroup
{
    public class ProductGroupsOverviewModel
    {
        public List<ProductGroupModel> ProductGroups { get; set; }
        public string Error { get; set; }


        public ProductGroupsOverviewModel()
        {
            this.ProductGroups = new List<ProductGroupModel>();
        }
   }
}