using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Models.Shared;

namespace Web.Areas.StockAdministration.Models.Product
{
    public class ProductJsonActionResponse : JsonActionResponse
    {
        public bool CloseModal { get; set; }
    }
}