using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.StockLevel
{
    public class OutpostStockLevelModel
    {
        public Guid OutpostId { get; set; }
        public Guid ProdGroupId { get; set; }
        public Guid ProductId { get; set; }
        public string ProdSmsRef { get; set; }
        public int StockLevel { get; set; }
        public int PrevStockLevel { get; set; }
        public string UpdatedMethod { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}