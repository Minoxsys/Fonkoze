using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class OutpostStockLevel : DomainEntity
    {
        public static string MANUAL_UPDATE = "Manual";

        public virtual int StockLevel { get; set; }

        public virtual int PrevStockLevel { get; set; }

        public virtual string UpdateMethod { get; set; }

        public virtual Client Client { get; set; }

        public virtual Product Product { get; set; }
        public virtual ProductGroup ProductGroup { get; set; }
        public virtual Outpost Outpost { get; set; }
    }
}
