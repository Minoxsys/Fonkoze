﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.StockAdministration.Models.OutpostStockLevel
{
    public class OutpostStockLevelInputModel
    {
        public Guid Id { get; set; }
        public Guid OutpostId { get; set; }
        public Guid ProdGroupId { get; set; }
        public Guid ProductId { get; set; }
        public string ProdSmsRef { get; set; }
        public int StockLevel { get; set; }
        public int PrevStockLevel { get; set; }
        public string UpdateMethod { get; set; }
        public bool EditAreCommingFromFilterByAllOutposts { get; set; }
    }
}