using System;
using System.Linq;
using Core.Domain;

namespace Domain
{
    public class RequestRecord : DomainEntity
    {
        public virtual Client Client { get; set; }

        public virtual Guid CampaignId { get; set; }
        public virtual string CampaignName { get; set; } 

        public virtual Guid OutpostId { get; set; }
        public virtual string OutpostName { get; set; } 


        public virtual Guid ProductGroupId { get; set; }
        public virtual string ProductGroupName { get; set; } 

        public virtual int ProductsNo { get; set; }
        public virtual Guid ProductLevelRequestId { get; set; }
    }
}
