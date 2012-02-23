using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class SmsRequest : DomainEntity
    {
        public virtual string Message { get; set; }
        public virtual string Number { get; set; }
        public virtual string ProductGroupReferenceCode { get; set; }
        public virtual Guid OutpostId { get; set; }
        public virtual Guid ProductGroupId { get; set; }
        public virtual Client Client { get; set; }
    }
}
