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
        public String OutpostName { get; set; }
        public Guid OutpostId { get; set; }
        public Guid ProductGroupId { get; set; }
        public IList<SelectListItem> ProductGroups { get; set; }
        public List<OutpostStockLevelModel> StockLevels { get; set; }
        public List<OutpostStockLevelModel> Products { get; set; }

        public string Error { get; set; }

 
        //public void Add(Domain.Product product)
        //{
        //    Products.Add(product);
        //}
    }
}