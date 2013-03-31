using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Shared
{
    public class ListQuery<TENTITY> where TENTITY : Core.Domain.DomainEntity
    {
        public List<TENTITY> Items { get; set; }
    }
    public class ListQuery
    {
    }
}