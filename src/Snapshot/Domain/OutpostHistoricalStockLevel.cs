﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class OutpostHistoricalStockLevel : DomainEntity
    {
        public virtual Guid OutpostId { get; set; }
        public virtual Guid ProductGroupId { get; set; }
        public virtual Guid ProductId { get; set; }
        public virtual string ProdSmsRef { get; set; }
        public virtual int StockLevel { get; set; }
        public virtual int PrevStockLevel { get; set; }
        public virtual string UpdateMethod { get; set; }
        public virtual DateTime? UpdateDate { get; set; }

        public virtual Guid ClientId { get; set; }

        public virtual string OutpostName { get; set; }
        public virtual string ProductGroupName { get; set; }

        public virtual string ProductName { get; set; }
    }
}
