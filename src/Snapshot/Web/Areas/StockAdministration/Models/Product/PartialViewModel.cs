using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models;

namespace Web.Areas.StockAdministration.Models.Product
{
    public class PartialViewModel
    {
        public List<ProductModel> Products { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public Guid ProductGroupSelectedId { get; set; }
        public string ValueOfSearchProduct { get; set; }

        public PartialViewModel()
        {
            this.Products = new List<ProductModel>();
            this.PagingInfo = new PagingInfo();
        }
    }
}