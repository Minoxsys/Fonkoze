using System;
using System.Collections.Generic;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Areas.OutpostManagement.Models.Contact;
using Web.Areas.StockAdministration.Models.Product;
using Web.Areas.StockAdministration.Models.ProductGroup;
using Domain;
using System.Linq;
using System.Web.Mvc;
using Core.Persistence;

namespace Web.Areas.OutpostManagement.Models.StockLevel
{
    public class OutpostStockLevelOverviewModel
    {
        public Guid OutpostId { get; set; }
        public Guid ProdGroupId { get; set; }
        public Guid ProductId { get; set; }
        public string ProdSmsRef { get; set; }
        public int StockLevel { get; set; }
        public int PrevStockLevel { get; set; }
        public string UpdatedMethod { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<Domain.Product> Products { get; set; }

        public String OutpostName { get; set; }
        public List<SelectListItem> ProductGroups { get; set; }
        public List<SelectListItem> ProductList { get; set; }

        public string Error { get; set; }


        public OutpostStockLevelOverviewModel()
        {
            this.Products = new List<Domain.Product>();
        }

        public void Add(Domain.Product product)
        {
            Products.Add(product);
        }

    }
}