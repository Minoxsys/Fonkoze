using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Persistence;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models;
using Web.Areas.StockAdministration.Models.Product;

namespace Web.Areas.OutpostManagement.Models.Product
{
    public class ListProductsModel
    {
        public List<ProductModel> Products { get; set; }
        public List<SelectListItem> ProductGroups { get; set; }

        public string Error { get; set; }

        public ListProductsModel(IQueryService<Domain.ProductGroup> queryProductGroup)
        {
            this.Products = new List<ProductModel>();
        }

        public void Add(ProductModel productModel)
        {
            Products.Add(productModel);
        }

    }
}