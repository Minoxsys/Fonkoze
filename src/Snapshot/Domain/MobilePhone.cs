using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;
using Domain;

namespace Domain
{
    public class MobilePhone : DomainEntity
    {
        public virtual string MobileNumber { get; set; }
        public virtual Outpost Outpost { get; set; }
    }
}
