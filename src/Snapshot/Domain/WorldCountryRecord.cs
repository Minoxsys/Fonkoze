using System;
using System.Linq;
using Core.Domain;

namespace Domain
{
    public class WorldCountryRecord : DomainEntity
    {
        public virtual string Name { get; set; }
        public virtual string ISOCode { get; set; }
        public virtual string PhonePrefix { get; set; }
    }
}
