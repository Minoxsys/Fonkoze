using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class Client : DomainEntity
    {
        public static Guid DEFAULT_ID = new Guid("BEEC53CE-A73C-4F03-A354-C617F68BC813");
        public virtual string Name { get; set; }
        
    }
}
