using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class Country : DomainEntity
    {
        public virtual string Name { get; set; }
        public virtual string ISOCode { get; set; }
        public virtual string PhonePrefix { get; set; }
        public virtual Client Client { get; set; }
        public virtual IList<Region> Regions { get; set; }
    }
}
