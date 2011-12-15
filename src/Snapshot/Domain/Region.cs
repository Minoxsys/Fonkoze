using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class Region : DomainEntity
    {
        public virtual string Name { get; set; }
        public virtual string Coordinates { get; set; }
        public virtual Country Country { get; set; }
        public virtual Client Client { get; set; }
    }
}
