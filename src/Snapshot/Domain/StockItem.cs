using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class StockItem : DomainEntity
    {
        public virtual String Name { get; set; }
        public virtual String Description { get; set; }
        public virtual int LowerLimit { get; set; }
        public virtual int UpperLimit { get; set; }
        public virtual String SMSReferenceCode { get; set; }
        public virtual StockGroup StockGroup { get; set; }
        public virtual IList<Outpost> Outposts { get; set; }

        public StockItem()
        {
            this.StockGroup = new StockGroup();
            this.Outposts = new List<Outpost>();
        }
    }
}
