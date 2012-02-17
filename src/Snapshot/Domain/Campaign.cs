using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class Campaign : DomainEntity
    {
        public virtual String Name { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual DateTime? CreationDate { get; set; }
        public virtual bool Open { get; set; }
        public virtual String Options { get; set; }
        public virtual Client Client { get; set; }
    }
}
