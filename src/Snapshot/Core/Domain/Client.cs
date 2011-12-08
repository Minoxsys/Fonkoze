using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Domain
{
    public class Client : DomainEntity
    {
        public virtual string Name { get; set; }
    }
}
