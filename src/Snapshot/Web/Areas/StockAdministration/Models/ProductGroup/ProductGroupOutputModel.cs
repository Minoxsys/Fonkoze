using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Web.Areas.StockAdministration.Models.ProductGroup
{
    public class ProductGroupOutputModel
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public List<SelectListItem> Productss { get; set; }
        public bool CommingFromProductCreate { get; set; }
    }
}