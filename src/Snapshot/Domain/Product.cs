using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class Product : DomainEntity
    {
        public virtual String Name { get; set; }
        public virtual String Description { get; set; }
        public virtual int LowerLimit { get; set; }
        public virtual int UpperLimit { get; set; }
        public virtual String SMSReferenceCode { get; set; }
        public virtual ProductGroup ProductGroup { get; set; }
        		
        public Product()
        {
            this.ProductGroup = new ProductGroup();
        }
    }
}
