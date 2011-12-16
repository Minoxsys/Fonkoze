using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class District : DomainEntity
    {
        public virtual string Name { get; set; }
        public virtual Region Region { get; set; }
        public virtual Client Client { get; set; }
    }
}
