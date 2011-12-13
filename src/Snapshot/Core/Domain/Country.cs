using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Core.Domain
{
    public class Country : DomainEntity
    {
        public virtual string Name { get; set; }
        public virtual string ISOCode { get; set; }
        public virtual string PhonePrefix { get; set; }
    }
}
