using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class EmailRequest : DomainEntity
    {
        public virtual DateTime Date { get; set; }
        public virtual Guid OutpostId { get; set; }
        public virtual Guid ProductGroupId { get; set; }
        public virtual Client Client { get; set; }
    }
}
