using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.Product
{
    public class FromProductGroupModel
    {
        public readonly Guid DEFAULT_UNCATEGORIZED_GUID = Guid.Parse("c1d9b38a-d2c6-4a95-be0e-215f569b782f");

        public Guid ProductGroupId { get; set; }
    }
}