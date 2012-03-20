using System;
using System.Linq;
using Core.Domain;

namespace Domain
{
    public class ProductLevelRequestDetail : DomainEntity
    {
        public virtual Guid ProductLevelRequestId { get; set; }
        public virtual string ProductGroupName { get; set; }
        public virtual string OutpostName { get; set; }

        public virtual string Method { get; set; }
        public virtual string RequestMessage { get; set; }

    }
}
