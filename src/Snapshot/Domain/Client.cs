using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class Client : DomainEntity
    {
        public virtual string Name { get; set; }
        
    }
}
