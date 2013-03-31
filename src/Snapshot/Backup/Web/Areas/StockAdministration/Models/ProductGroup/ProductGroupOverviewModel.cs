using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Domain;
using Domain;

namespace Web.Areas.StockAdministration.Models.ProductGroup
{
    public class ProductGroupOverviewModel
    {
        public List<ProductGroupModel> ProductGroups { get; set; }
        public int ProductsNo { get; set; }
        public string Error { get; set; }


        public ProductGroupOverviewModel()
        {
            this.ProductGroups = new List<ProductGroupModel>();
        }

    }
}