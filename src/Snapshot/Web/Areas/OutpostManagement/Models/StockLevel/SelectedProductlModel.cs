using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.OutpostManagement.Models.StockLevel
{
    public class SelectedProductlModel
    {
        public Guid ProductId { get; set; }
        public String ProductGroupName { get; set; }
        public String ProductName { get; set; }
        public String ProdSmsRef { get; set; }
        public DateTime Updated { get; set; }
        public bool Selected { get; set; }
    }
}