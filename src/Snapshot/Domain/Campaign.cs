using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;
using Core.Services;

namespace Domain
{
    public class Campaign : DomainEntity
    {
        public virtual String Name { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual DateTime? CreationDate { get; set; }
        public virtual bool Opened { get; set; }
        public virtual byte[] Options { get; set; }
        public virtual Client Client { get; set; }

        public virtual void StoreOptions<T>(T model)
        {
            this.Options = BinaryJsonStore<T>.From(model);
        }

        public virtual T RestoreOptions<T>()
        {
            return BinaryJsonStore<T>.From(this.Options);
        }
    }
}
