using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Persistence;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models;

namespace Web.Areas.StockAdministration.Models.Product
{
    public class ProductOverviewModel
    {
        public List<ProductModel> Products { get; set; }
        public List<SelectListItem> ProductGroups { get; set; }
        public PartialViewModel PartialViewModel { get; set; }

        public IQueryService<Domain.ProductGroup> QueryProductGroup { get; set; }

        public ProductOverviewModel(IQueryService<Domain.ProductGroup> queryProductGroup)
        {
            this.Products = new List<ProductModel>();
            this.ProductGroups = new List<SelectListItem>();
            this.QueryProductGroup = queryProductGroup;

            var productGroups = QueryProductGroup.Query();

            foreach (Domain.ProductGroup productGroup in productGroups)
            {
                this.ProductGroups.Add(new SelectListItem { Text = productGroup.Name, Value = productGroup.Id.ToString() });
            }
        }
    }
}