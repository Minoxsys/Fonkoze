using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.OutpostManagement.Models.StockLevel
{
    public class OutpostStockLevelInputModel
    {
        [Required]
        public Guid OutpostId { get; set; }
        [Required]
        public Guid ProdGroupId { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        public string ProdSmsRef { get; set; }
        public int StockLevel { get; set; }
        public int PrevStockLevel { get; set; }
        public string UpdatedMethod { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}