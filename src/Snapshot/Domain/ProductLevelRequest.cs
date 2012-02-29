using System;
using System.Linq;
using Core.Domain;

namespace Domain
{
    public class ProductLevelRequest:DomainEntity
    {
        public virtual Campaign Campaign { get; set; }
        public virtual Schedule Schedule { get; set; }
        public virtual ProductGroup ProductGroup { get; set; }
        public virtual Client Client { get; set; }
        public virtual byte[] Products { get; set; }

    }
}
