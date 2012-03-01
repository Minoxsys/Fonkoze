using System;
using System.Linq;
using Core.Domain;
using Core.Services;

namespace Domain
{
    public class ProductLevelRequest:DomainEntity
    {
        public virtual Campaign Campaign { get; set; }
        public virtual Schedule Schedule { get; set; }
        public virtual ProductGroup ProductGroup { get; set; }
        public virtual Client Client { get; set; }
        public virtual byte[] Products { get; set; }

        public virtual bool IsStopped{get;set;}

        public virtual void StoreProducts<T>(T model)
        {
            this.Products = BinaryJsonStore<T>.From(model);
        }

        public virtual T RestoreProducts<T>()
        {
            return BinaryJsonStore<T>.From(this.Products);
        }
    }
}
