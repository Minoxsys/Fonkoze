using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class OutpostStockLevel : DomainEntity
    {
        public virtual Guid OutpostId { get; set; }
        public virtual Guid ProdGroupId { get; set; }
        public virtual Guid ProductId { get; set; }
        public virtual string ProdSmsRef { get; set; }
        public virtual int StockLevel { get; set; }
        public virtual int PrevStockLevel { get; set; }
        public virtual string UpdatedMethod { get; set; }
        public virtual DateTime UpdatedDate { get; set; }
        public virtual String ProductName { get; set; }
    }
}
