using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class ProductGroup : DomainEntity
    {
        public virtual String Name { get; set; }
        public virtual String Description { get; set; }
        
        
    }
}
