using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class ProductSale : DomainEntity
    {
        public virtual Outpost Outpost { get; set; }
        public virtual Product Product { get; set; }
        public virtual Int32 Quantity { get; set; }
        public virtual string ClientIdentifier { get; set; }
    }
}
