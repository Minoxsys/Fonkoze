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

    }
}