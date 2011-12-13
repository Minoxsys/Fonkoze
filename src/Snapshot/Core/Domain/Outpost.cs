using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Core.Domain
{
    public class Outpost : DomainEntity
    {
        public virtual string Name { get; set; }
        public virtual string Type { get; set; }
        public virtual string MobileNumber { get; set; }
        public virtual string Email { get; set; }
    }
}
