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
        public virtual Guid Id { get; set; }
        public virtual string MethodType { get; set; }
        public virtual string ContactDetail { get; set; }
        public virtual string MainMethod { get; set; }
        public virtual Outpost Outpost { get; set; }
        public virtual Client Client { get; set; }
        public virtual Guid Outpost_FK { get; set; }
    }
}
